using System;
using System.Data;
using System.Data.SqlClient;

public class BuyReturnRepository
{
    private readonly string _connectionString;

    public BuyReturnRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable GetBuyReturnsReport(
        DateTime fromDate,
        DateTime toDate,
        int? supplierId)
    {
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            con.Open();

            string sql = @"

SELECT
    r.BuyReturnId,
    r.ReturnDate,
    r.OriginalBuyInvoiceId,
    s.Name AS SupplierName,

    p.Name AS ProductName,
    rl.Quantity,
    rl.UnitPrice,
    rl.TaxRate,
    rl.LineBeforeTax,
    rl.LineTax,
    rl.LineAfterTax

FROM BuyReturns r

INNER JOIN Suppliers s
ON r.SupplierId = s.SupplierId

INNER JOIN BuyReturnLines rl
ON r.BuyReturnId = rl.BuyReturnId

INNER JOIN Products p
ON rl.ProductId = p.ProductId

WHERE
r.ReturnDate >= @FromDate
AND r.ReturnDate < DATEADD(DAY,1,@ToDate)
AND (@SupplierId IS NULL OR r.SupplierId=@SupplierId)

ORDER BY r.ReturnDate DESC
";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@SupplierId", (object)supplierId ?? DBNull.Value);

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            return dt;
        }
    }
}