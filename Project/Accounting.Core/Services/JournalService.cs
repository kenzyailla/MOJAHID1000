using Accounting.Core.Enums;
using Accounting.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace Accounting.Core.Services
{
    public class JournalService
    {
        //private readonly string _connectionString;
        private string _connectionString;
        public JournalService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // =========================
        // Sales (Cash): Debit Cash, Credit Sales, Credit VAT
        // =========================
        public void CreateCashSalesEntry(decimal netAmount, decimal vatAmount, decimal totalAmount, int referenceId)
        {
            CreateSalesEntryInternal(
                netAmount: netAmount,
                vatAmount: vatAmount,
                totalAmount: totalAmount,
                referenceId: referenceId,
                debitAccountCode: "101", // Cash
                description: "فاتورة مبيعات نقدية"
            );
        }

        // =========================
        // Sales (Credit): Debit Customers, Credit Sales, Credit VAT
        // =========================
        public void CreateCreditSalesEntry(decimal netAmount, decimal vatAmount, decimal totalAmount, int referenceId)
        {
            CreateSalesEntryInternal(
                netAmount: netAmount,
                vatAmount: vatAmount,
                totalAmount: totalAmount,
                referenceId: referenceId,
                debitAccountCode: "111", // Customers (A/R)
                description: "فاتورة مبيعات آجلة"
            );
        }

        // =========================
        // Shared internal method
        // =========================
        private void CreateSalesEntryInternal(decimal netAmount, decimal vatAmount, decimal totalAmount, int referenceId,
                                              string debitAccountCode, string description)
        {
            // حماية بسيطة
            if (totalAmount <= 0)
                throw new Exception("Invalid total amount.");

            if (netAmount + vatAmount != totalAmount)
                throw new Exception("Not balanced: net + vat must equal total.");

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 1) Header + get JournalId
                    string insertJournal = @"
INSERT INTO JournalEntries (EntryDate, ReferenceType, ReferenceId, Description)
VALUES (@Date, N'Invoice', @RefId, @Desc);
SELECT SCOPE_IDENTITY();
";
                    int journalId;
                    using (SqlCommand cmd = new SqlCommand(insertJournal, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RefId", referenceId);
                        cmd.Parameters.AddWithValue("@Desc", description);

                        journalId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2) Dynamic accounts
                    int debitAccId = GetAccountId(con, trans, debitAccountCode);
                    int salesAccId = GetAccountId(con, trans, "301"); // Sales
                    int vatAccId = GetAccountId(con, trans, "401");   // VAT Output

                    // 3) Lines
                    InsertLine(con, trans, journalId, debitAccId, totalAmount, 0m);
                    InsertLine(con, trans, journalId, salesAccId, 0m, netAmount);
                    InsertLine(con, trans, journalId, vatAccId, 0m, vatAmount);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        private void InsertLine(SqlConnection con, SqlTransaction trans, int journalId, int accountId, decimal debit, decimal credit)
        {
            string sql = @"
INSERT INTO JournalLines (JournalId, AccountId, Debit, Credit)
VALUES (@JId, @AccId, @Debit, @Credit);
";
            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@JId", journalId);
                cmd.Parameters.AddWithValue("@AccId", accountId);
                cmd.Parameters.AddWithValue("@Debit", debit);
                cmd.Parameters.AddWithValue("@Credit", credit);
                cmd.ExecuteNonQuery();
            }
        }

        private int GetAccountId(SqlConnection con, SqlTransaction trans, string accountCode)
        {
            string sql = @"
SELECT AccountId
FROM Accounts
WHERE AccountCode = @Code AND IsActive = 1;
";
            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@Code", accountCode);
                object result = cmd.ExecuteScalar();

                if (result == null)
                    throw new Exception("Account not found: " + accountCode);

                return Convert.ToInt32(result);
            }
        }
        public void CreateSalesEntry(
    SqlConnection con,
    SqlTransaction trans,
    decimal netAmount,
    decimal vatAmount,
    decimal totalAmount,
    int paymentType,
    int referenceId)
        {
            // ======================
            // 1️⃣ Insert Journal Header
            // ======================

            string insertJournal = @"
        INSERT INTO JournalEntries
        (EntryDate, ReferenceType, ReferenceId, Description)
        VALUES
        (@Date, N'Invoice', @RefId, N'فاتورة مبيعات');
        SELECT SCOPE_IDENTITY();
    ";

            int journalId;

            using (SqlCommand cmd = new SqlCommand(insertJournal, con, trans))
            {
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@RefId", referenceId);

                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // ======================
            // 2️⃣ تحديد الحسابات
            // ======================

            int cashId = GetAccountId(con, trans, "101");       // الصندوق
            int customerId = GetAccountId(con, trans, "102");   // ذمم عملاء
            int salesId = GetAccountId(con, trans, "301");      // المبيعات
            int vatId = GetAccountId(con, trans, "401");        // ضريبة مخرجات

            // ======================
            // 3️⃣ تحديد المدين حسب نوع الدفع
            // ======================

            if (paymentType == (int)PaymentType.Cash)
            {
                InsertLine(con, trans, journalId, cashId, totalAmount, 0);
            }
            else
            {
                InsertLine(con, trans, journalId, customerId, totalAmount, 0);
            }

            // ======================
            // 4️⃣ الدائن
            // ======================

            InsertLine(con, trans, journalId, salesId, 0, netAmount);
            InsertLine(con, trans, journalId, vatId, 0, vatAmount);
        }
        public void DeleteJournalByReference(SqlConnection con, SqlTransaction trans, string referenceType, int referenceId)
        {
            // احضر JournalId
            string getId = @"
        SELECT TOP 1 JournalId
        FROM JournalEntries
        WHERE ReferenceType = @Type AND ReferenceId = @RefId
        ORDER BY JournalId DESC
    ";

            int journalId;

            using (SqlCommand cmd = new SqlCommand(getId, con, trans))
            {
                cmd.Parameters.AddWithValue("@Type", referenceType);
                cmd.Parameters.AddWithValue("@RefId", referenceId);

                object o = cmd.ExecuteScalar();
                if (o == null) return;

                journalId = Convert.ToInt32(o);
            }

            // احذف التفاصيل ثم الرأس
            using (SqlCommand cmd = new SqlCommand("DELETE FROM JournalLines WHERE JournalId=@J", con, trans))
            {
                cmd.Parameters.AddWithValue("@J", journalId);
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = new SqlCommand("DELETE FROM JournalEntries WHERE JournalId=@J", con, trans))
            {
                cmd.Parameters.AddWithValue("@J", journalId);
                cmd.ExecuteNonQuery();
            }
        }
        public void CreateChequeCollectionEntry(decimal amount, int receiptId)
        {
            CreateEntry(
                "ChequeCollection",
                receiptId,
                amount,
                104, // البنك
                103  // شيكات تحت التحصيل
            );
        }
        public void CreateChequeReturnEntry(decimal amount, int receiptId, int customerId)
        {
            CreateEntry(
                "ChequeReturn",
                receiptId,
                amount,
                102, // ذمم العملاء
                103  // شيكات تحت التحصيل
            );
        }
        private void CreateEntry(
    string refType,
    int refId,
    decimal amount,
    int debitAccount,
    int creditAccount)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    string insertJournal = @"
INSERT INTO JournalEntries
(EntryDate,ReferenceType,ReferenceId,Description)
VALUES
(GETDATE(),@RefType,@RefId,@Desc);

SELECT SCOPE_IDENTITY();
";

                    int journalId;

                    using (SqlCommand cmd = new SqlCommand(insertJournal, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@RefType", refType);
                        cmd.Parameters.AddWithValue("@RefId", refId);
                        cmd.Parameters.AddWithValue("@Desc", refType);

                        journalId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    InsertJournalLine(con, trans, journalId, debitAccount, amount, 0);
                    InsertJournalLine(con, trans, journalId, creditAccount, 0, amount);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        private void InsertJournalLine(
    SqlConnection con,
    SqlTransaction trans,
    int journalId,
    int accountId,
    decimal debit,
    decimal credit)
        {
            string sql = @"
INSERT INTO JournalLines
(JournalId,AccountId,Debit,Credit)
VALUES
(@Journal,@Account,@Debit,@Credit)
";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@Journal", journalId);
                cmd.Parameters.AddWithValue("@Account", accountId);
                cmd.Parameters.AddWithValue("@Debit", debit);
                cmd.Parameters.AddWithValue("@Credit", credit);

                cmd.ExecuteNonQuery();
            }
        }
        public void AddJournalEntry(JournalEntry entry)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    //-------------------------------------
                    // Insert Journal Header
                    //-------------------------------------

                    string sqlHeader = @"
INSERT INTO JournalEntries
(EntryDate,ReferenceType,ReferenceId,Description)
VALUES
(@Date,@RefType,@RefId,@Desc);

SELECT SCOPE_IDENTITY();
";

                    int journalId;

                    using (SqlCommand cmd = new SqlCommand(sqlHeader, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Date", entry.EntryDate);
                        cmd.Parameters.AddWithValue("@RefType", entry.ReferenceType);
                        cmd.Parameters.AddWithValue("@RefId", entry.ReferenceId);
                        cmd.Parameters.AddWithValue("@Desc", entry.Description ?? "");

                        journalId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //-------------------------------------
                    // Insert Lines
                    //-------------------------------------

                    foreach (var line in entry.Lines)
                    {
                        string sqlLine = @"
INSERT INTO JournalLines
(JournalId,AccountId,Debit,Credit)
VALUES
(@JournalId,@Account,@Debit,@Credit)
";

                        using (SqlCommand cmd = new SqlCommand(sqlLine, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@JournalId", journalId);
                            cmd.Parameters.AddWithValue("@Account", line.AccountId);
                            cmd.Parameters.AddWithValue("@Debit", line.Debit);
                            cmd.Parameters.AddWithValue("@Credit", line.Credit);

                            cmd.ExecuteNonQuery();
                        }
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
        public void AddJournalEntry(
    SqlConnection con,
    SqlTransaction trans,
    JournalEntry entry)
        {
            string insertEntry = @"
INSERT INTO JournalEntries
(EntryDate,ReferenceType,ReferenceId,Description)
VALUES
(@Date,@RefType,@RefId,@Desc);
SELECT SCOPE_IDENTITY();
";

            int journalId;

            using (SqlCommand cmd = new SqlCommand(insertEntry, con, trans))
            {
                cmd.Parameters.AddWithValue("@Date", entry.EntryDate);
                cmd.Parameters.AddWithValue("@RefType", entry.ReferenceType);
                cmd.Parameters.AddWithValue("@RefId", entry.ReferenceId);
                cmd.Parameters.AddWithValue("@Desc", entry.Description ?? "");

                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }


            //-------------------------------------------------
            // إدخال السطور
            //-------------------------------------------------

            foreach (var line in entry.Lines)
            {
                string insertLine = @"
INSERT INTO JournalLines
(JournalId,AccountId,Debit,Credit)
VALUES
(@Journal,@Account,@Debit,@Credit)
";

                using (SqlCommand cmd = new SqlCommand(insertLine, con, trans))
                {
                    cmd.Parameters.AddWithValue("@Journal", journalId);
                    cmd.Parameters.AddWithValue("@Account", line.AccountId);
                    cmd.Parameters.AddWithValue("@Debit", line.Debit);
                    cmd.Parameters.AddWithValue("@Credit", line.Credit);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void CreateSalesReturnEntry(
    SqlConnection con,
    SqlTransaction trans,
    int salesReturnId,
    decimal totalBefore,
    decimal totalTax,
    decimal totalAfter,
    int paymentType,
    int customerId)
        {
            // 🔹 حساباتك حسب جدول Accounts عندك
            int salesAccountId = GetAccountIdByCode(con, trans, "301"); // المبيعات
            int taxOutAccountId = GetAccountIdByCode(con, trans, "401"); // ضريبة مخرجات
            int cashAccountId = GetAccountIdByCode(con, trans, "101"); // الصندوق
            int bankAccountId = GetAccountIdByCode(con, trans, "104"); // البنك
            int arAccountId = GetAccountIdByCode(con, trans, "102"); // ذمم عملاء

            int creditAccountId;

            switch (paymentType)
            {
                case 1: creditAccountId = cashAccountId; break;
                case 2: creditAccountId = bankAccountId; break;
                case 3: creditAccountId = arAccountId; break;
                default: creditAccountId = arAccountId; break;
            }

            int journalId;

            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalEntries
(EntryDate, ReferenceType, ReferenceId, Description)
VALUES
(GETDATE(), 'SalesReturn', @RefId, @Desc);
SELECT SCOPE_IDENTITY();", con, trans))
            {
                cmd.Parameters.AddWithValue("@RefId", salesReturnId);
                cmd.Parameters.AddWithValue("@Desc", "مرتجع مبيعات رقم " + salesReturnId);

                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 🔹 مدين: المبيعات (عكس الإيراد)
            InsertJournalLine(con, trans, journalId, salesAccountId, totalBefore, 0m);

            // 🔹 مدين: ضريبة المخرجات
            InsertJournalLine(con, trans, journalId, taxOutAccountId, totalTax, 0m);

            // 🔹 دائن: الصندوق / البنك / ذمم
            InsertJournalLine(con, trans, journalId, creditAccountId, 0m, totalAfter);
        }
        private int GetAccountIdByCode(SqlConnection con, SqlTransaction trans, string code)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Accounts WHERE AccountCode=@Code AND IsActive=1",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Code", code);
                object result = cmd.ExecuteScalar();

                if (result == null)
                    throw new Exception("الحساب غير موجود: " + code);

                return Convert.ToInt32(result);
            }
        }
        public void CreateBuyReturnEntry(
        SqlConnection con,
        SqlTransaction trans,
        int buyReturnId,
        decimal totalBefore,
        decimal totalTax,
        decimal totalAfter,
        int supplierId)
        {
            int purchasesAccount = GetAccountIdByCode(con, trans, "501");
            int taxInputAccount = GetAccountIdByCode(con, trans, "402");

            // 🔥 هذا هو التعديل المهم
            int supplierAccountId = GetSupplierAccountId(con, trans, supplierId);

            int journalId;

            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalEntries
(EntryDate, ReferenceType, ReferenceId, Description)
VALUES
(GETDATE(), 'BuyReturn', @RefId, @Desc);
SELECT SCOPE_IDENTITY();", con, trans))
            {
                cmd.Parameters.AddWithValue("@RefId", buyReturnId);
                cmd.Parameters.AddWithValue("@Desc", "مرتجع مشتريات رقم " + buyReturnId);

                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // دائن: المشتريات
            InsertJournalLine(con, trans, journalId, purchasesAccount, 0m, totalBefore);

            // دائن: ضريبة المدخلات
            InsertJournalLine(con, trans, journalId, taxInputAccount, 0m, totalTax);

            // 🔥 مدين: حساب المورد الصحيح
            InsertJournalLine(con, trans, journalId, supplierAccountId, totalAfter, 0m);
        }
            public void CreateBuyInvoiceEntry(
    SqlConnection con,
    SqlTransaction trans,
    int buyInvoiceId,
    decimal totalBefore,
    decimal totalTax,
    decimal totalAfter,
    int supplierId)
        {
            // 1️⃣ جلب AccountId الخاص بالمورد
            int supplierAccountId;

            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", supplierId);
                supplierAccountId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 2️⃣ إنشاء JournalEntry
            int journalId;

            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalEntries
(EntryDate, ReferenceType, ReferenceId, Description, CreatedAt)
VALUES
(GETDATE(), 'BuyInvoice', @RefId, @Desc, GETDATE());
SELECT SCOPE_IDENTITY();", con, trans))
            {
                cmd.Parameters.AddWithValue("@RefId", buyInvoiceId);
                cmd.Parameters.AddWithValue("@Desc", "فاتورة مشتريات رقم " + buyInvoiceId);

                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 3️⃣ إضافة القيود

            // حساب المشتريات (مدين)
            int purchasesAccountId = GetAccountIdByCode(con, trans, "500"); // عدل حسب شجرة حساباتك

            InsertJournalLine(con, trans, journalId, purchasesAccountId, totalBefore, 0);

            // حساب ضريبة المدخلات (مدين)
            if (totalTax > 0)
            {
                int vatAccountId = GetAccountIdByCode(con, trans, "142"); // عدل حسب حساب الضريبة عندك
                InsertJournalLine(con, trans, journalId, vatAccountId, totalTax, 0);
            }

            // حساب المورد (دائن)
            InsertJournalLine(con, trans, journalId, supplierAccountId, 0, totalAfter);
        }
        private int GetSupplierAccountId(SqlConnection con, SqlTransaction trans, int supplierId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", supplierId);

                object result = cmd.ExecuteScalar();
                if (result == null)
                    throw new Exception("المورد غير مرتبط بحساب محاسبي");

                return Convert.ToInt32(result);
            }
        }
        public void CreateReceiptEntry(int receiptId, int partyId, decimal amount, PartyType partyType)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    //-------------------------------------------------
                    // 1️⃣ إنشاء القيد
                    //-------------------------------------------------
                    string insertJournal = @"
INSERT INTO JournalEntries
(EntryDate, ReferenceType, ReferenceId, Description)
VALUES
(GETDATE(), 'Receipt', @RefId, @Desc);
SELECT SCOPE_IDENTITY();
";

                    int journalId;

                    using (SqlCommand cmd = new SqlCommand(insertJournal, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@RefId", receiptId);
                        cmd.Parameters.AddWithValue("@Desc", "سند قبض");

                        journalId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //-------------------------------------------------
                    // 2️⃣ تحديد الحسابات
                    //-------------------------------------------------

                    int cashAccount = GetAccountIdByCode(con, trans, "101"); // الصندوق

                    int partyAccount;

                    if (partyType == PartyType.Customer)
                    {
                        partyAccount = GetCustomerAccountId(con, trans, partyId);
                    }
                    else
                    {
                        partyAccount = GetSupplierAccountId(con, trans, partyId);
                    }
                  
                    //-------------------------------------------------
                    // 3️⃣ القيود
                    //-------------------------------------------------

                    // مدين: الصندوق
                    InsertJournalLine(con, trans, journalId, cashAccount, amount, 0);

                    // دائن: العميل / المورد
                    InsertJournalLine(con, trans, journalId, partyAccount, 0, amount);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        private int GetCustomerAccountId(SqlConnection con, SqlTransaction trans, int customerId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Customers WHERE CustomerId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", customerId);

                object result = cmd.ExecuteScalar();

                if (result == null)
                    throw new Exception("العميل غير مرتبط بحساب");

                return Convert.ToInt32(result);
            }
        }
    }
}
