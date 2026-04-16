using System;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Repositories
{
    public class VatSummaryRepository
    {
        private readonly string _connectionString;

        public VatSummaryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetVatSummary(DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"

SELECT 
    TaxRate,

    -- مبيعات
    SUM(SalesBeforeTax) AS SalesBeforeTax,
    SUM(SalesTax) AS SalesTax,

    -- مشتريات
    SUM(BuyBeforeTax) AS BuyBeforeTax,
    SUM(BuyTax) AS BuyTax,

    -- صافي الضريبة
    SUM(SalesTax - BuyTax) AS NetVat

FROM
(
    -------------------------------------------------
    -- 🔵 المبيعات
    -------------------------------------------------
    SELECT 
        l.TaxRate,
        l.TotalBeforeTax AS SalesBeforeTax,
        l.TotalTax AS SalesTax,
        0 AS BuyBeforeTax,
        0 AS BuyTax
    FROM Invoices i
    INNER JOIN InvoiceLines l ON i.InvoiceId = l.InvoiceId
    WHERE i.InvoiceType = 1
      AND i.InvoiceDate >= @FromDate
      AND i.InvoiceDate < DATEADD(DAY,1,@ToDate)

    UNION ALL

    -------------------------------------------------
    -- 🔴 مرتجع المبيعات (سالب)
    -------------------------------------------------
    SELECT 
        rl.TaxRate,
        -rl.LineBeforeTax,
        -rl.LineTax,
        0,
        0
    FROM SalesReturns r
    INNER JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
    WHERE r.ReturnDate >= @FromDate
      AND r.ReturnDate < DATEADD(DAY,1,@ToDate)

    UNION ALL

    -------------------------------------------------
    -- 🟢 المشتريات
    -------------------------------------------------
    SELECT 
        l.TaxRate,
        0,
        0,
        l.LineSubTotal,
        l.TaxAmount
    FROM BuyInvoices b
    INNER JOIN BuyInvoiceLines l ON b.BuyInvoiceId = l.BuyInvoiceId
    WHERE b.InvoiceDate >= @FromDate
      AND b.InvoiceDate < DATEADD(DAY,1,@ToDate)

    UNION ALL

    -------------------------------------------------
    -- 🔴 مرتجع المشتريات (سالب)
    -------------------------------------------------
    SELECT 
        rl.TaxRate,
        0,
        0,
        -rl.LineBeforeTax,
        -rl.LineTax
    FROM BuyReturns r
    INNER JOIN BuyReturnLines rl ON r.BuyReturnId = rl.BuyReturnId
    WHERE r.ReturnDate >= @FromDate
      AND r.ReturnDate < DATEADD(DAY,1,@ToDate)

) AS VATDATA

GROUP BY TaxRate
ORDER BY TaxRate
";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
    }
}

