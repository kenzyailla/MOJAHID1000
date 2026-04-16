using System;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Repositories
{
    public class BuyReportRepository
    {
        private readonly string _connectionString;

        public BuyReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetBuyReportAdvanced(
            int? productId,
            int? supplierId,
            string invoiceNumber,
            decimal? taxRate,
            DateTime fromDate,
            DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT
    b.BuyInvoiceId,
    b.InvoiceNumber,
    b.InvoiceDate,
    s.Name AS SupplierName,

    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TaxRate,
    l.LineSubTotal,
    l.TaxAmount,
    l.LineTotal
FROM BuyInvoices b
INNER JOIN Suppliers s ON b.SupplierId = s.SupplierId
INNER JOIN BuyInvoiceLines l ON b.BuyInvoiceId = l.BuyInvoiceId
INNER JOIN Products p ON l.ProductId = p.ProductId
WHERE
    b.InvoiceDate >= @FromDate
    AND b.InvoiceDate < DATEADD(DAY, 1, @ToDate)

    AND (@ProductId IS NULL OR l.ProductId = @ProductId)
    AND (@SupplierId IS NULL OR b.SupplierId = @SupplierId)
    AND (@InvoiceNumber = '' OR b.InvoiceNumber = @InvoiceNumber)
    AND (@TaxRate IS NULL OR l.TaxRate = @TaxRate)

ORDER BY b.InvoiceDate DESC, b.BuyInvoiceId DESC
";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                cmd.Parameters.AddWithValue("@ProductId", (object)productId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SupplierId", (object)supplierId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxRate", (object)taxRate ?? DBNull.Value);

                invoiceNumber = (invoiceNumber ?? "").Trim();
                cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
    }
}
