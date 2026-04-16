using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Accounting.Core.Repositories
{
    public class SalesReportRepository
    {
        private readonly string _connectionString;

        public SalesReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetSalesReportAdvanced(
            int? productId,
            int? customerId,
            string invoiceNumber,
              decimal? taxRate,
            DateTime fromDate,
            DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {

            

                con.Open();

                string sql = @"

-- ======================= المبيعات =======================
SELECT 
    i.InvoiceId,
    i.InvoiceNumber,
    i.InvoiceDate,
    c.Name AS CustomerName,
    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TaxRate,
  (l.Quantity * l.UnitPrice) AS TotalBeforeTax,

(l.Quantity * l.UnitPrice * l.TaxRate / 100.0) AS TotalTax,

(l.Quantity * l.UnitPrice) +
(l.Quantity * l.UnitPrice * l.TaxRate / 100.0) AS TotalAfterTax
FROM Invoices i
INNER JOIN Customers c ON i.CustomerId = c.CustomerId
INNER JOIN InvoiceLines l ON i.InvoiceId = l.InvoiceId
INNER JOIN Products p ON l.ProductId = p.ProductId
WHERE
   i.InvoiceType IN (1, 5)
    AND i.InvoiceDate >= @FromDate
    AND i.InvoiceDate < DATEADD(DAY,1,@ToDate)
    AND (@ProductId IS NULL OR l.ProductId = @ProductId)
    AND (@CustomerId IS NULL OR i.CustomerId = @CustomerId)
    AND (@InvoiceNumber IS NULL OR i.InvoiceNumber = @InvoiceNumber)
    AND (@TaxRate IS NULL OR l.TaxRate = @TaxRate)

UNION ALL

-- ======================= مرتجع المبيعات =======================
SELECT 
    r.SalesReturnId,
    CAST(r.SalesReturnId AS NVARCHAR(50)),
    r.ReturnDate,
    c.Name,
    rl.ProductId,
    p.Name,
    -rl.Quantity,
    rl.UnitPrice,
    rl.TaxRate,
    -rl.LineBeforeTax,
    -rl.LineTax,
    -rl.LineAfterTax
FROM SalesReturns r
INNER JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
INNER JOIN Products p ON rl.ProductId = p.ProductId
INNER JOIN Customers c ON r.CustomerId = c.CustomerId
WHERE
    r.ReturnDate >= @FromDate
    AND r.ReturnDate < DATEADD(DAY,1,@ToDate)
    AND (@ProductId IS NULL OR rl.ProductId = @ProductId)
    AND (@CustomerId IS NULL OR r.CustomerId = @CustomerId)
    AND (@TaxRate IS NULL OR rl.TaxRate = @TaxRate)

ORDER BY 3 DESC
";
                        


                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                cmd.Parameters.AddWithValue("@ProductId", (object)productId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CustomerId", (object)customerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxRate", (object)taxRate ?? DBNull.Value);

                invoiceNumber = (invoiceNumber ?? "").Trim();
                //cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                cmd.Parameters.AddWithValue("@InvoiceNumber",
    string.IsNullOrWhiteSpace(invoiceNumber)
        ? (object)DBNull.Value
        : invoiceNumber);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
        }
    }
}

