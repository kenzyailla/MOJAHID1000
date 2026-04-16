using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Accounting.Core.Repositories
{
    public class SalesVatDeclarationRepository
    {
        private readonly string _connectionString;

        public SalesVatDeclarationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetSalesVatDeclaration(DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"

;WITH SalesData AS
(
    -- المبيعات
    SELECT 
        l.TaxRate,
        SUM(l.TotalBeforeTax) AS SalesBefore,
        SUM(l.TotalTax) AS SalesTax,
        0 AS ReturnBefore,
        0 AS ReturnTax
    FROM Invoices i
    INNER JOIN InvoiceLines l ON i.InvoiceId = l.InvoiceId
    WHERE i.InvoiceType = 1
      AND i.InvoiceDate >= @FromDate
      AND i.InvoiceDate < DATEADD(DAY,1,@ToDate)
    GROUP BY l.TaxRate

    UNION ALL

    -- مرتجع المبيعات
    SELECT
        rl.TaxRate,
        0,
        0,
        SUM(rl.LineBeforeTax),
        SUM(rl.LineTax)
    FROM SalesReturns r
    INNER JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
    WHERE r.ReturnDate >= @FromDate
      AND r.ReturnDate < DATEADD(DAY,1,@ToDate)
    GROUP BY rl.TaxRate
)

SELECT
    TaxRate,
    SUM(SalesBefore) AS SalesBeforeTax,
    SUM(SalesTax) AS SalesTax,
    SUM(ReturnBefore) AS ReturnBeforeTax,
    SUM(ReturnTax) AS ReturnTax,
    SUM(SalesTax) - SUM(ReturnTax) AS NetTax
FROM SalesData
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
