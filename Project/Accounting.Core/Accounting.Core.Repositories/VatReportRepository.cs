using System;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Repositories
{
    public class VatReportRepository
    {
        private readonly string _connectionString;

        public VatReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetVatSummary(DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
;WITH Rates AS (
    SELECT CAST(0.00 AS decimal(5,2)) AS TaxRate UNION ALL
    SELECT CAST(1.00 AS decimal(5,2)) UNION ALL
    SELECT CAST(4.00 AS decimal(5,2)) UNION ALL
    SELECT CAST(16.00 AS decimal(5,2))
),

-------------------------------------------------
-- 🔵 المبيعات
-------------------------------------------------
SalesAgg AS (
    SELECT 
        CAST(l.TaxRate AS decimal(5,2)) AS TaxRate,
        SUM(ISNULL(l.TotalBeforeTax, 0)) AS SalesBeforeTax,
        SUM(ISNULL(l.TotalTax, 0))       AS SalesTax
    FROM InvoiceLines l
    INNER JOIN Invoices i ON i.InvoiceId = l.InvoiceId
    WHERE i.InvoiceType = 1
      AND i.InvoiceDate >= @FromDate
      AND i.InvoiceDate < DATEADD(DAY, 1, @ToDate)
    GROUP BY CAST(l.TaxRate AS decimal(5,2))
),

-------------------------------------------------
-- 🔴 مرتجع المبيعات
-------------------------------------------------
SalesReturnAgg AS (
    SELECT
        CAST(rl.TaxRate AS decimal(5,2)) AS TaxRate,
        SUM(ISNULL(rl.LineBeforeTax,0))  AS ReturnBeforeTax,
        SUM(ISNULL(rl.LineTax,0))        AS ReturnTax
    FROM SalesReturns r
    INNER JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
    WHERE r.ReturnDate >= @FromDate
      AND r.ReturnDate < DATEADD(DAY,1,@ToDate)
    GROUP BY CAST(rl.TaxRate AS decimal(5,2))
),

-------------------------------------------------
-- 🟢 المشتريات
-------------------------------------------------
BuyAgg AS (
    SELECT
        CAST(l.TaxRate AS decimal(5,2)) AS TaxRate,
        SUM(ISNULL(l.LineSubTotal, 0))  AS BuyBeforeTax,
        SUM(ISNULL(l.TaxAmount, 0))     AS BuyTax
    FROM BuyInvoiceLines l
    INNER JOIN BuyInvoices b ON b.BuyInvoiceId = l.BuyInvoiceId
    WHERE b.InvoiceDate >= @FromDate
      AND b.InvoiceDate < DATEADD(DAY, 1, @ToDate)
    GROUP BY CAST(l.TaxRate AS decimal(5,2))
),

-------------------------------------------------
-- 🔴 مرتجع المشتريات
-------------------------------------------------
BuyReturnAgg AS (
    SELECT
        CAST(rl.TaxRate AS decimal(5,2)) AS TaxRate,
        SUM(ISNULL(rl.LineBeforeTax,0))  AS ReturnBeforeTax,
        SUM(ISNULL(rl.LineTax,0))        AS ReturnTax
    FROM BuyReturns r
    INNER JOIN BuyReturnLines rl ON r.BuyReturnId = rl.BuyReturnId
    WHERE r.ReturnDate >= @FromDate
      AND r.ReturnDate < DATEADD(DAY,1,@ToDate)
    GROUP BY CAST(rl.TaxRate AS decimal(5,2))
)

-------------------------------------------------
-- ✅ النتيجة النهائية
-------------------------------------------------
SELECT
    r.TaxRate,

    -- المبيعات الصافية
    ISNULL(s.SalesBeforeTax,0) 
      - ISNULL(sr.ReturnBeforeTax,0) AS SalesBeforeTax,

    ISNULL(s.SalesTax,0)
      - ISNULL(sr.ReturnTax,0) AS SalesTax,

    -- المشتريات الصافية
    ISNULL(b.BuyBeforeTax,0)
      - ISNULL(br.ReturnBeforeTax,0) AS BuyBeforeTax,

    ISNULL(b.BuyTax,0)
      - ISNULL(br.ReturnTax,0) AS BuyTax,

    -- صافي الضريبة
    (
        (ISNULL(s.SalesTax,0) - ISNULL(sr.ReturnTax,0))
        -
        (ISNULL(b.BuyTax,0) - ISNULL(br.ReturnTax,0))
    ) AS NetTax

FROM Rates r
LEFT JOIN SalesAgg s        ON s.TaxRate = r.TaxRate
LEFT JOIN SalesReturnAgg sr ON sr.TaxRate = r.TaxRate
LEFT JOIN BuyAgg b          ON b.TaxRate = r.TaxRate
LEFT JOIN BuyReturnAgg br   ON br.TaxRate = r.TaxRate

ORDER BY r.TaxRate;
";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    return dt;
                }
            }
        }
    }
}

