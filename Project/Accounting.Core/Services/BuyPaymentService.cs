using Accounting.Core.Models;
using Accounting.Core.Repositories;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Services
{
    public class BuyPaymentService
    {
        private readonly string _connectionString;
        int paymentId = 0;
        public BuyPaymentService(string connectionString)
        {
            _connectionString = connectionString;
        }

        //-----------------------------------------------------
        // جلب الفواتير غير المسددة
        //-----------------------------------------------------
        public DataTable GetUnpaidInvoices(int supplierId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT b.BuyInvoiceId,
       b.InvoiceNumber,
       b.InvoiceDate,
       b.TotalAfterTax,
       ISNULL(SUM(p.PaidAmount),0) AS Paid,
       b.TotalAfterTax - ISNULL(SUM(p.PaidAmount),0) AS Remaining
FROM BuyInvoices b
LEFT JOIN BuyPaymentInvoices p 
    ON b.BuyInvoiceId = p.BuyInvoiceId
WHERE b.SupplierId=@Supplier
GROUP BY b.BuyInvoiceId,
         b.InvoiceNumber,
         b.InvoiceDate,
         b.TotalAfterTax
HAVING b.TotalAfterTax - ISNULL(SUM(p.PaidAmount),0) > 0";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Supplier", supplierId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        public void SavePayment(int supplierId, string paymentMethod, DataTable dt)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    //----------------------------------
                    // 1️⃣ حساب إجمالي السداد
                    //----------------------------------

                    decimal totalPaid = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["AmountToPay"] == DBNull.Value)
                            continue;

                        decimal amount = Convert.ToDecimal(row["AmountToPay"]);
                        if (amount > 0)
                            totalPaid += amount;
                    }

                    if (totalPaid <= 0)
                        throw new Exception("لا يوجد مبلغ للسداد");

                    //----------------------------------
                    // 2️⃣ إدخال رأس سند الصرف
                    //----------------------------------

                    string insertPayment = @"
INSERT INTO BuyPayments
(SupplierId, PaymentDate, Amount, PaymentMethod, CreatedAt)
VALUES
(@Supplier, GETDATE(), @Amount, @Method, GETDATE());
SELECT SCOPE_IDENTITY();";

                    int paymentId;

                    using (SqlCommand cmd = new SqlCommand(insertPayment, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Supplier", supplierId);
                        cmd.Parameters.AddWithValue("@Amount", totalPaid);
                        cmd.Parameters.AddWithValue("@Method", paymentMethod);

                        paymentId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //----------------------------------
                    // 3️⃣ إدخال توزيع الدفع على الفواتير
                    //----------------------------------

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["AmountToPay"] == DBNull.Value)
                            continue;

                        decimal amount = Convert.ToDecimal(row["AmountToPay"]);
                        if (amount <= 0)
                            continue;

                        int invoiceId = Convert.ToInt32(row["BuyInvoiceId"]);

                        string insertDetail = @"
INSERT INTO BuyPaymentInvoices
(BuyPaymentId, BuyInvoiceId, PaidAmount)
VALUES
(@Payment, @Invoice, @Amount)";

                        using (SqlCommand cmd = new SqlCommand(insertDetail, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Payment", paymentId);
                            cmd.Parameters.AddWithValue("@Invoice", invoiceId);
                            cmd.Parameters.AddWithValue("@Amount", amount);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    //----------------------------------
                    // 4️⃣ إنشاء القيد المحاسبي
                    //----------------------------------

                    int supplierAccountId = GetSupplierAccountId(con, trans, supplierId);
                    int cashAccountId = paymentMethod == "Cash" ? 1 : 10;

                    JournalEntry entry = new JournalEntry
                    {
                        EntryDate = DateTime.Now,
                        ReferenceType = "BuyPayment",
                        ReferenceId = paymentId,
                        Description = "سداد مورد"
                    };

                    // مدين ذمم الموردين
                    entry.Lines.Add(new JournalLine
                    {
                        AccountId = supplierAccountId,
                        Debit = totalPaid,
                        Credit = 0
                    });

                    // دائن الصندوق / البنك
                    entry.Lines.Add(new JournalLine
                    {
                        AccountId = cashAccountId,
                        Debit = 0,
                        Credit = totalPaid
                    });

                    JournalRepository repo = new JournalRepository(_connectionString);
                    repo.AddJournalEntry(con, trans, entry);

                    //----------------------------------
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        private int GetSupplierAccountId(
    SqlConnection con,
    SqlTransaction trans,
    int supplierId)
        {
            string sql = "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", supplierId);
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception("المورد غير مرتبط بحساب محاسبي");

                return Convert.ToInt32(result);
            }
        }
      
        public  int SavePaymentMixed(
    int supplierId,
    string paymentMethod,   // Mixed / Cheque / Cash / Bank
    string cashMethod,      // Cash / Bank (فقط عندما cashAmount > 0)
    decimal cashAmount,
    DataTable dtInvoices,
    DataTable dtCheques)
        {
        

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 1) إجمالي توزيع السداد على الفواتير
                    decimal totalPaid = 0m;
                    foreach (DataRow row in dtInvoices.Rows)
                    {
                        if (row["AmountToPay"] == DBNull.Value) continue;
                        decimal amount = Convert.ToDecimal(row["AmountToPay"]);
                        if (amount > 0) totalPaid += amount;
                    }

                    if (totalPaid <= 0)
                        throw new Exception("لا يوجد مبلغ للسداد");

                    // 2) اجمالي الشيكات من الجريد
                    decimal chequesTotal = 0m;

                    foreach (DataRow r in dtCheques.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        if (r["Amount"] == DBNull.Value) continue;

                        decimal amt = Convert.ToDecimal(r["Amount"]);
                        if (amt > 0) chequesTotal += amt;
                    }

                    // 3) تحقق التوازن (كاش + شيكات = totalPaid)
                    if ((cashAmount + chequesTotal) != totalPaid)
                        throw new Exception($"مجموع الدفع لا يساوي الموزع على الفواتير. الموزع={totalPaid}، كاش+شيكات={cashAmount + chequesTotal}");

                    // 4) إدخال رأس السند
                    string insertPayment = @"
INSERT INTO BuyPayments
(SupplierId, PaymentDate, Amount, PaymentMethod, Notes, CreatedAt)
VALUES
(@Supplier, CAST(GETDATE() AS DATE), @Amount, @Method, @Notes, GETDATE());
SELECT SCOPE_IDENTITY();";

                    int paymentId;
                    using (SqlCommand cmd = new SqlCommand(insertPayment, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Supplier", supplierId);
                        cmd.Parameters.AddWithValue("@Amount", totalPaid);
                        cmd.Parameters.AddWithValue("@Method", paymentMethod);
                        cmd.Parameters.AddWithValue("@Notes", $"CashMethod={cashMethod}, Cash={cashAmount}, Cheques={chequesTotal}");

                        paymentId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 5) تفاصيل توزيع الدفع على الفواتير
                    foreach (DataRow row in dtInvoices.Rows)
                    {
                        if (row["AmountToPay"] == DBNull.Value) continue;

                        decimal amount = Convert.ToDecimal(row["AmountToPay"]);
                        if (amount <= 0) continue;

                        int invoiceId = Convert.ToInt32(row["BuyInvoiceId"]);

                        string insertDetail = @"
INSERT INTO BuyPaymentInvoices
(BuyPaymentId, BuyInvoiceId, PaidAmount)
VALUES
(@Payment, @Invoice, @Amount);";

                        using (SqlCommand cmd = new SqlCommand(insertDetail, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Payment", paymentId);
                            cmd.Parameters.AddWithValue("@Invoice", invoiceId);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 6) حفظ الشيكات (إن وجدت)
                    foreach (DataRow r in dtCheques.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;

                        string chequeNumber = (r["ChequeNumber"] ?? "").ToString().Trim();
                        string bankName = (r["BankName"] ?? "").ToString().Trim();

                        if (r["DueDate"] == DBNull.Value) continue; // لو صف فاضي
                        DateTime dueDate = Convert.ToDateTime(r["DueDate"]);

                        decimal amt = 0m;
                        if (r["Amount"] != DBNull.Value)
                            amt = Convert.ToDecimal(r["Amount"]);

                        // تجاهل الصفوف الفاضية تمامًا
                        if (string.IsNullOrWhiteSpace(chequeNumber) && amt == 0m)
                            continue;

                        if (string.IsNullOrWhiteSpace(chequeNumber))
                            throw new Exception("رقم الشيك مطلوب");

                        if (amt <= 0)
                            throw new Exception("قيمة الشيك يجب أن تكون أكبر من صفر");

                        string insertCheque = @"
INSERT INTO OutgoingCheques
(BuyPaymentId, SupplierId, ChequeNumber, BankName, IssueDate, DueDate, Amount, Status, CreatedAt)
VALUES
(@PayId, @Supplier, @No, @Bank, CAST(GETDATE() AS DATE), @Due, @Amt, 1, GETDATE());";

                        using (SqlCommand cmd = new SqlCommand(insertCheque, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@PayId", paymentId);
                            cmd.Parameters.AddWithValue("@Supplier", supplierId);
                            cmd.Parameters.AddWithValue("@No", chequeNumber);
                            cmd.Parameters.AddWithValue("@Bank", bankName);
                            cmd.Parameters.AddWithValue("@Due", dueDate);
                            cmd.Parameters.AddWithValue("@Amt", amt);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 7) القيد المحاسبي (احترافي)
                    int supplierAccountId = GetSupplierAccountId(con, trans, supplierId);
                    int cashId = GetAccountIdByCode(con, trans, "101");        // الصندوق
                    int bankId = GetAccountIdByCode(con, trans, "102");        // البنك
                    int outgoingChequesId = GetAccountIdByCode(con, trans, "205"); // شيكات صادرة

                    int cashOrBankId = (cashMethod == "Bank") ? bankId : cashId;

                    JournalEntry entry = new JournalEntry
                    {
                        EntryDate = DateTime.Today,
                        ReferenceType = "BuyPayment",
                        ReferenceId = paymentId,
                        Description = $"سند صرف مورد رقم {supplierId}"
                    };

                    // مدين: ذمم موردين (تسديد/تخفيض الالتزام)
                    entry.Lines.Add(new JournalLine
                    {
                        AccountId = supplierAccountId,
                        Debit = totalPaid,
                        Credit = 0m
                    });

                    // دائن: صندوق/بنك (جزء الكاش)
                    if (cashAmount > 0m)
                    {
                        entry.Lines.Add(new JournalLine
                        {
                            AccountId = cashOrBankId,
                            Debit = 0m,
                            Credit = cashAmount
                        });
                    }

                    // دائن: شيكات صادرة
                    if (chequesTotal > 0m)
                    {
                        entry.Lines.Add(new JournalLine
                        {
                            AccountId = outgoingChequesId,
                            Debit = 0m,
                            Credit = chequesTotal
                        });
                    }

                    JournalRepository repo = new JournalRepository(_connectionString);
                    repo.AddJournalEntry(con, trans, entry);

                    trans.Commit();
                    return paymentId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        private int GetAccountIdByCode(SqlConnection con, SqlTransaction trans, string code)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Accounts WHERE AccountCode=@Code AND IsActive=1", con, trans))
            {
                cmd.Parameters.AddWithValue("@Code", code);
                object r = cmd.ExecuteScalar();
                if (r == null || r == DBNull.Value)
                    throw new Exception("حساب غير موجود: " + code);
                return Convert.ToInt32(r);
            }
        }
        public void DeletePayment(int paymentId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // ======================================
                    // 1️⃣ تأكد أن السند موجود
                    // ======================================
                    int supplierId = 0;
                    decimal amount = 0;

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT SupplierId, Amount FROM BuyPayments WHERE BuyPaymentId=@Id",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", paymentId);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                                throw new Exception("سند الصرف غير موجود");

                            supplierId = Convert.ToInt32(dr["SupplierId"]);
                            amount = Convert.ToDecimal(dr["Amount"]);
                        }
                    }

                    // ======================================
                    // 2️⃣ منع حذف الشيك لو حالته غير نشط
                    // ======================================
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM OutgoingCheques WHERE BuyPaymentId=@Id AND Status<>1",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", paymentId);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            throw new Exception("لا يمكن حذف السند لأن بعض الشيكات تم صرفها أو إرجاعها");
                    }

                    // ======================================
                    // 3️⃣ حذف القيد المحاسبي
                    // ======================================
                    DeleteJournalByReference(con, trans, paymentId);

                    // ======================================
                    // 4️⃣ حذف الشيكات
                    // ======================================
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM OutgoingCheques WHERE BuyPaymentId=@Id",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", paymentId);
                        cmd.ExecuteNonQuery();
                    }

                    // ======================================
                    // 5️⃣ حذف توزيع الفواتير
                    // ======================================
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM BuyPaymentInvoices WHERE BuyPaymentId=@Id",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", paymentId);
                        cmd.ExecuteNonQuery();
                    }

                    // ======================================
                    // 6️⃣ حذف رأس السند
                    // ======================================
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM BuyPayments WHERE BuyPaymentId=@Id",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", paymentId);
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        private void DeleteJournalByReference(
    SqlConnection con,
    SqlTransaction trans,
    int paymentId)
        {
            int journalId = 0;

            using (SqlCommand cmd = new SqlCommand(
                "SELECT JournalId FROM JournalEntries WHERE ReferenceType='BuyPayment' AND ReferenceId=@Ref",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Ref", paymentId);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    journalId = Convert.ToInt32(result);
            }

            if (journalId > 0)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM JournalLines WHERE JournalId=@J",
                    con, trans))
                {
                    cmd.Parameters.AddWithValue("@J", journalId);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM JournalEntries WHERE JournalId=@J",
                    con, trans))
                {
                    cmd.Parameters.AddWithValue("@J", journalId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

