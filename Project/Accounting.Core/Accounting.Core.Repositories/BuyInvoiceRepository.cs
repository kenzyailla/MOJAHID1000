using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accounting.Core.Models;

using System.Data.SqlClient;
using System.Data;

using Accounting.Core.Repositories; // إذا JournalRepository هنا
namespace Accounting.Core.Repositories
{
    public class BuyInvoiceRepository
    {
        private readonly string _connectionString;

        public BuyInvoiceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddBuyInvoice(BuyInvoice invoice)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        //----------------------------------
                        // إضافة الرأس
                        //----------------------------------

                        string sqlInvoice = @"
INSERT INTO BuyInvoices
(InvoiceNumber, SupplierId, InvoiceDate, DueDate,PaymentType, 
 SubTotal, TaxTotal, TotalAfterTax,
 PaymentStatus, Notes)
VALUES
(@Number,@Supplier,@Date,@Due,@PaymentType,
 @Sub,@Tax,@Total,
 @Status,@Notes);
SELECT SCOPE_IDENTITY();
";

                        int invoiceId;

                        using (SqlCommand cmd = new SqlCommand(sqlInvoice, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Number", invoice.InvoiceNumber);
                            cmd.Parameters.AddWithValue("@Supplier", invoice.SupplierId);
                            cmd.Parameters.AddWithValue("@Date", invoice.InvoiceDate);
                            cmd.Parameters.AddWithValue("@Due", (object)invoice.DueDate ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentType", invoice.PaymentType);
                            cmd.Parameters.AddWithValue("@Sub", invoice.SubTotal);
                            cmd.Parameters.AddWithValue("@Tax", invoice.TaxTotal);
                            cmd.Parameters.AddWithValue("@Total", invoice.TotalAfterTax);
                            cmd.Parameters.AddWithValue("@Status", (int)invoice.PaymentStatus);
                            cmd.Parameters.AddWithValue("@Notes", invoice.Notes ?? "");

                            invoiceId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        //----------------------------------
                        // إضافة التفاصيل
                        //----------------------------------

                        foreach (var line in invoice.Lines)
                        {
                            string sqlLine = @"
INSERT INTO BuyInvoiceLines
(BuyInvoiceId, ProductId, Quantity, UnitPrice,
 LineSubTotal, TaxRate, TaxAmount, LineTotal)
VALUES
(@Invoice,@Product,@Qty,@Price,
 @Sub,@Rate,@Tax,@Total)";

                            using (SqlCommand cmd = new SqlCommand(sqlLine, con, trans))
                            {
                                cmd.Parameters.AddWithValue("@Invoice", invoiceId);
                                cmd.Parameters.AddWithValue("@Product", line.ProductId);
                                cmd.Parameters.AddWithValue("@Qty", line.Quantity);
                                cmd.Parameters.AddWithValue("@Price", line.UnitPrice);
                                cmd.Parameters.AddWithValue("@Sub", line.LineSubTotal);
                                cmd.Parameters.AddWithValue("@Rate", line.TaxRate);
                                cmd.Parameters.AddWithValue("@Tax", line.TaxAmount);
                                cmd.Parameters.AddWithValue("@Total", line.LineTotal);

                                cmd.ExecuteNonQuery();
                            }
                        }
                

                        foreach (var line in invoice.Lines)
                        {
                            InsertInventoryTransaction(
                                con,
                                trans,
                                line.ProductId,
                                line.Quantity,
                                invoiceId);
                        }
                        // ================== Journal Entry (قيد فاتورة مشتريات) ==================
                        int supplierAccountId;
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id",
                            con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Id", invoice.SupplierId);

                            object result = cmd.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                                throw new Exception("هذا المورد غير مرتبط بحساب محاسبي (AccountId)");

                            supplierAccountId = Convert.ToInt32(result);
                        }

                       
                        trans.Commit();
                        return invoiceId;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        public DataTable GetAllBuyInvoices()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    b.BuyInvoiceId,
    b.InvoiceNumber,
    s.Name AS SupplierName,
    b.InvoiceDate,
    b.TotalAfterTax,
    CASE 
        WHEN b.PaymentStatus = 1 THEN N'غير مدفوع'
        WHEN b.PaymentStatus = 2 THEN N'مدفوع جزئي'
        WHEN b.PaymentStatus = 3 THEN N'مدفوع بالكامل'
        ELSE N'غير معروف'
    END AS PaymentStatusName
FROM BuyInvoices b
INNER JOIN Suppliers s ON b.SupplierId = s.SupplierId
ORDER BY b.BuyInvoiceId DESC;
";

                SqlCommand cmd = new SqlCommand(sql, con);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }

      
     
        public void UpdateBuyInvoice(int id, BuyInvoice invoice)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    //----------------------------------
                    // تحديث الهيدر
                    //----------------------------------
                    string updateHeader = @"
UPDATE BuyInvoices
SET 
    SupplierId = @Supplier,
    InvoiceDate = @Date,
    DueDate = @DueDate,
    SubTotal = @Sub,
    TaxTotal = @Tax,
    TotalAfterTax = @Total,
    Notes = @Notes,
    PaymentStatus = @Status
WHERE BuyInvoiceId = @Id";

                    SqlCommand cmdHeader = new SqlCommand(updateHeader, con, trans);

                    cmdHeader.Parameters.AddWithValue("@Supplier", invoice.SupplierId);
                    cmdHeader.Parameters.AddWithValue("@Date", invoice.InvoiceDate);
                    cmdHeader.Parameters.AddWithValue("@DueDate", invoice.DueDate);
                    cmdHeader.Parameters.AddWithValue("@Sub", invoice.SubTotal);
                    cmdHeader.Parameters.AddWithValue("@Tax", invoice.TaxTotal);
                    cmdHeader.Parameters.AddWithValue("@Total", invoice.TotalAfterTax);
                    cmdHeader.Parameters.AddWithValue("@Notes", invoice.Notes ?? "");
                    cmdHeader.Parameters.AddWithValue("@Status", (int)invoice.PaymentStatus);
                    cmdHeader.Parameters.AddWithValue("@Id", id);

                    cmdHeader.ExecuteNonQuery();
                    // حذف حركات المخزون القديمة
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM InventoryTransactions WHERE TransactionType = 1 AND ReferenceId = @Id",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                    //----------------------------------
                    // حذف الأصناف القديمة
                    //----------------------------------
                    SqlCommand cmdDelete = new SqlCommand(
                        "DELETE FROM BuyInvoiceLines WHERE BuyInvoiceId=@Id",
                        con, trans);

                    cmdDelete.Parameters.AddWithValue("@Id", id);
                    cmdDelete.ExecuteNonQuery();

                    //----------------------------------
                    // إدخال الأصناف الجديدة
                    //----------------------------------
                    foreach (var line in invoice.Lines)
                    {
                        string insertLine = @"
INSERT INTO BuyInvoiceLines
(BuyInvoiceId, ProductId, Quantity, UnitPrice,
 LineSubTotal, TaxRate, TaxAmount, LineTotal)
VALUES
(@Inv, @Prod, @Qty, @Price,
 @Sub, @TaxRate, @TaxAmount, @Total)";

                        SqlCommand cmdLine = new SqlCommand(insertLine, con, trans);

                        cmdLine.Parameters.AddWithValue("@Inv", id);
                        cmdLine.Parameters.AddWithValue("@Prod", line.ProductId);
                        cmdLine.Parameters.AddWithValue("@Qty", line.Quantity);
                        cmdLine.Parameters.AddWithValue("@Price", line.UnitPrice);
                        cmdLine.Parameters.AddWithValue("@Sub", line.LineSubTotal);
                        cmdLine.Parameters.AddWithValue("@TaxRate", line.TaxRate);
                        cmdLine.Parameters.AddWithValue("@TaxAmount", line.TaxAmount);
                        cmdLine.Parameters.AddWithValue("@Total", line.LineTotal);

                        cmdLine.ExecuteNonQuery();
                    }
                    foreach (var line in invoice.Lines)
                    {
                        InsertInventoryTransaction(
                            con,
                            trans,
                            line.ProductId,
                            line.Quantity,
                            id);
                    }
                    // ================== حذف القيد القديم ==================
                    using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM JournalLines
WHERE JournalId IN (
    SELECT JournalId
    FROM JournalEntries
    WHERE ReferenceType = 'BuyInvoice'
    AND ReferenceId = @Id
)", con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM JournalEntries
WHERE ReferenceType = 'BuyInvoice'
AND ReferenceId = @Id
", con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // ================== إنشاء القيد الجديد ==================

                    int supplierAccountId;

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id",
                        con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", invoice.SupplierId);
                        supplierAccountId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    JournalEntry entry = new JournalEntry
                    {
                        EntryDate = invoice.InvoiceDate,
                        ReferenceType = "BuyInvoice",
                        ReferenceId = id,
                        Description = "فاتورة مشتريات رقم " + id
                    };

                    entry.Lines.Add(new JournalLine
                    {
                        AccountId = 8, // المخزون
                        Debit = invoice.SubTotal,
                        Credit = 0
                    });

                    if (invoice.TaxTotal > 0)
                    {
                        entry.Lines.Add(new JournalLine
                        {
                            AccountId = 5, // ضريبة مدخلات
                            Debit = invoice.TaxTotal,
                            Credit = 0
                        });
                    }

                    entry.Lines.Add(new JournalLine
                    {
                        AccountId = supplierAccountId,
                        Debit = 0,
                        Credit = invoice.TotalAfterTax
                    });

                    JournalRepository repo = new JournalRepository(_connectionString);
                    repo.AddJournalEntry(con, trans, entry);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        public DataTable GetInvoiceLines(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
        SELECT 
            l.ProductId,
            p.Name AS ProductName,
            l.Quantity,
            l.UnitPrice,
            l.TaxRate,
            l.LineSubTotal AS SubTotal,
            l.TaxAmount,
            l.LineTotal AS Total
        FROM BuyInvoiceLines l
        INNER JOIN Products p ON l.ProductId = p.ProductId
        WHERE l.BuyInvoiceId = @Id
        ";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", invoiceId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        private void InsertInventoryTransaction(
        SqlConnection con,
        SqlTransaction trans,
        int productId,
        decimal qty,
        int invoiceId)
        {
            string sql = @"
INSERT INTO InventoryTransactions
(ProductId, Quantity, TransactionType, ReferenceId, TransactionDate)
VALUES
(@ProductId, @Qty, 1, @RefId, GETDATE());";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@RefId", invoiceId);

                cmd.ExecuteNonQuery();
            }
        }
        public DataRow GetBuyInvoiceHeader(int buyInvoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT *
FROM BuyInvoices
WHERE BuyInvoiceId = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", buyInvoiceId);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];

                    return null;
                }
            }
        }
        public DataTable GetBuyInvoiceLines(int buyInvoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    ProductId,
    Quantity,
    UnitPrice,
    TaxRate,
    LineSubTotal,
    TaxAmount,
    LineTotal
FROM BuyInvoiceLines
WHERE BuyInvoiceId = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", buyInvoiceId);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    return dt;
                }
            }
        }
    }
}

