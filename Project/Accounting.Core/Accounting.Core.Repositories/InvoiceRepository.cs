using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Repositories
{
    public class InvoiceRepository
    {
        private readonly string _connectionString;

        public InvoiceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetInvoices()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT
i.InvoiceId,
i.InvoiceNumber,
i.InvoiceDate,
c.Name AS CustomerName,
i.TotalBeforeTax,
i.TotalTax,
i.TotalAfterTax,
i.PostedToTax,
i.PostedDate
FROM Invoices i
LEFT JOIN Customers c 
ON i.CustomerId = c.CustomerId
ORDER BY i.InvoiceId DESC

        ";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        public DataRow GetInvoiceHeader(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = "SELECT * FROM Invoices WHERE InvoiceId=@Id";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                    return dt.Rows[0];

                return null;
            }
        }
        //public DataTable GetInvoiceLines(int invoiceId)
        //{
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        con.Open();

        //        string sql = @"
        //SELECT
        //    ProductId,
        //    Quantity,
        //    UnitPrice,
        //    Discount,
        //    TaxRate,
        //    TotalBeforeTax,
        //    TotalTax,
        //    TotalAfterTax
        //FROM InvoiceLines
        //WHERE InvoiceId=@Id
        //";

        //        SqlDataAdapter da = new SqlDataAdapter(sql, con);
        //        da.SelectCommand.Parameters.AddWithValue("@Id", invoiceId);

        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        return dt;
        //    }
        //}

        public DataTable GetInvoiceLines(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
        SELECT
            l.ProductId,
            p.Name AS ProductName, -- 🔥 الجديد
            l.Quantity,
            l.UnitPrice,
            l.Discount,
            l.TaxRate,
            l.TotalBeforeTax,
            l.TotalTax,
            l.TotalAfterTax
        FROM InvoiceLines l
        LEFT JOIN Products p
            ON l.ProductId = p.ProductId
        WHERE l.InvoiceId=@Id
        ";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public void DeleteInvoice(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // حذف حركات المخزون
                    SqlCommand cmdStock = new SqlCommand(
                        "DELETE FROM InventoryTransactions WHERE ReferenceId=@Id",
                        con, trans);

                    cmdStock.Parameters.AddWithValue("@Id", invoiceId);
                    cmdStock.ExecuteNonQuery();


                    // حذف تفاصيل الفاتورة
                    SqlCommand cmdLines = new SqlCommand(
                        "DELETE FROM InvoiceLines WHERE InvoiceId=@Id",
                        con, trans);

                    cmdLines.Parameters.AddWithValue("@Id", invoiceId);
                    cmdLines.ExecuteNonQuery();


                    // حذف الفاتورة
                    SqlCommand cmdInv = new SqlCommand(
                        "DELETE FROM Invoices WHERE InvoiceId=@Id",
                        con, trans);

                    cmdInv.Parameters.AddWithValue("@Id", invoiceId);
                    cmdInv.ExecuteNonQuery();


                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        public DataTable GetSalesReturns()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    r.SalesReturnId,
    r.OriginalInvoiceId,
    c.Name AS CustomerName,
    r.ReturnDate,
    r.TotalAfterTax,
    r.Notes
FROM SalesReturns r
INNER JOIN Customers c ON r.CustomerId = c.CustomerId
ORDER BY r.SalesReturnId DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}

