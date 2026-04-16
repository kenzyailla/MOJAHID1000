using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting.Core.Models
{
   



    public class CustomerService
    {
        private string _connectionString;
        private CustomerRepository _repo;

        public CustomerService(string connectionString)
        {
            _connectionString = connectionString;
            _repo = new CustomerRepository(connectionString);
        }

        public DataTable GetAllCustomers()
        {
            return _repo.GetAllCustomers();
        }

        public int AddCustomer(Customer customer)
        {
            return _repo.AddCustomerAndCreateAccount(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            _repo.UpdateCustomer(customer);
        }

        public void DeleteCustomer(int id)
        {
            _repo.DeleteCustomer(id);
        }

        public DataTable GetCustomerStatement(int customerId, DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"

DECLARE @OpeningDebit DECIMAL(18,5) = 0;
DECLARE @OpeningCredit DECIMAL(18,5) = 0;

------------------------------------------------
-- الرصيد الافتتاحي
------------------------------------------------

SELECT 
@OpeningDebit =
ISNULL(SUM(CASE WHEN BalanceType = 1 THEN OpeningBalance ELSE 0 END),0),

@OpeningCredit =
ISNULL(SUM(CASE WHEN BalanceType = 2 THEN OpeningBalance ELSE 0 END),0)

FROM CustomerOpeningBalances
WHERE CustomerId = @CustomerId



------------------------------------------------
-- العمليات السابقة قبل الفترة
------------------------------------------------

SELECT 
@OpeningDebit = @OpeningDebit + 
ISNULL(SUM(TotalAfterTax),0)
FROM Invoices
WHERE CustomerId = @CustomerId
AND InvoiceType = 1
AND InvoiceDate < @FromDate;


SELECT 
@OpeningCredit = @OpeningCredit + 
ISNULL(SUM(TotalAfterTax),0)
FROM SalesReturns
WHERE CustomerId = @CustomerId
AND ReturnDate < @FromDate;


SELECT 
@OpeningCredit = @OpeningCredit +
ISNULL(SUM(rd.Amount),0)
FROM Receipts r
JOIN ReceiptDetails rd ON r.ReceiptId = rd.ReceiptId
WHERE r.PartyType = 1
AND r.PartyId = @CustomerId
AND r.ReceiptDate < @FromDate;


------------------------------------------------
-- كشف الحساب
------------------------------------------------

SELECT *
FROM
(

------------------------------------------------
-- رصيد سابق
------------------------------------------------

SELECT
0 AS SortOrder,
@FromDate AS InvoiceDate,
N'رصيد سابق' AS Description,
@OpeningDebit AS Debit,
@OpeningCredit AS Credit,
NULL AS InvoiceId,
NULL AS ReceiptId


UNION ALL


------------------------------------------------
-- فواتير المبيعات
------------------------------------------------

SELECT
1,
i.InvoiceDate,
N'فاتورة مبيعات رقم '
+ CAST(i.InvoiceNumber AS NVARCHAR)
+ N' بتاريخ '
+ CONVERT(NVARCHAR(10),i.InvoiceDate,23),

i.TotalAfterTax,
0,
i.InvoiceId,
NULL

FROM Invoices i
WHERE i.CustomerId = @CustomerId
AND i.InvoiceType = 1
AND i.PaymentType = 3   -- 🔥 هذا السطر المهم
AND i.InvoiceDate >= @FromDate
AND i.InvoiceDate < DATEADD(DAY,1,@ToDate)


UNION ALL


------------------------------------------------
-- مرتجع المبيعات
------------------------------------------------

SELECT
1,
r.ReturnDate,
N'مرتجع مبيعات رقم '
+ CAST(r.SalesReturnId AS NVARCHAR)
+ N' بتاريخ '
+ CONVERT(NVARCHAR(10),r.ReturnDate,23),

0,
r.TotalAfterTax,
r.SalesReturnId,
NULL

FROM SalesReturns r
WHERE r.CustomerId = @CustomerId
AND r.ReturnDate >= @FromDate
AND r.ReturnDate < DATEADD(DAY,1,@ToDate)


UNION ALL


------------------------------------------------
-- سندات القبض
------------------------------------------------

SELECT
1,
r.ReceiptDate,
N'سند قبض رقم '
+ CAST(r.ReceiptNumber AS NVARCHAR)
+ N' بتاريخ '
+ CONVERT(NVARCHAR(10),r.ReceiptDate,23),

0,
SUM(rd.Amount),
NULL,
r.ReceiptId

FROM Receipts r
JOIN ReceiptDetails rd ON r.ReceiptId = rd.ReceiptId

WHERE r.PartyType = 1
AND r.PartyId = @CustomerId
AND r.ReceiptDate >= @FromDate
AND r.ReceiptDate < DATEADD(DAY,1,@ToDate)

GROUP BY
r.ReceiptDate,
r.ReceiptNumber,
r.ReceiptId


) X

ORDER BY SortOrder, InvoiceDate

";
               
                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                da.SelectCommand.Parameters.AddWithValue("@CustomerId", customerId);
                da.SelectCommand.Parameters.AddWithValue("@FromDate", fromDate.Date);
                da.SelectCommand.Parameters.AddWithValue("@ToDate", toDate.Date);

                DataTable dt = new DataTable();
                da.Fill(dt);

                //------------------------------------------------
                // حساب الرصيد المتراكم
                //------------------------------------------------

                if (!dt.Columns.Contains("Balance"))
                    dt.Columns.Add("Balance", typeof(decimal));

                decimal runningBalance = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal debit = row["Debit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Debit"]);
                    decimal credit = row["Credit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Credit"]);

                    runningBalance += debit - credit;

                    row["Balance"] = runningBalance;
                }

                return dt;
            }
        }


        public void SaveOpeningBalance(int customerId, decimal balance, int type)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO CustomerOpeningBalances
(CustomerId, OpeningBalance, BalanceType, CreatedAt)
VALUES
(@Customer, @Balance, @Type, @CreatedAt)";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@Customer", customerId);
                cmd.Parameters.AddWithValue("@Balance", balance);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }
        public DataTable GetCustomersBalances()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"

SELECT
c.CustomerId,
c.Name AS CustomerName,

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) <= 30
THEN i.TotalAfterTax
ELSE 0
END
) AS [0_30],

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) BETWEEN 31 AND 60
THEN i.TotalAfterTax
ELSE 0
END
) AS [31_60],

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) BETWEEN 61 AND 90
THEN i.TotalAfterTax
ELSE 0
END
) AS [61_90],

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) > 90
THEN i.TotalAfterTax
ELSE 0
END
) AS [90_Plus],

SUM(i.TotalAfterTax) AS TotalBalance

FROM Customers c

LEFT JOIN Invoices i
ON i.CustomerId = c.CustomerId
AND i.InvoiceType = 1

WHERE c.IsActive = 1

GROUP BY
c.CustomerId,
c.Name

HAVING SUM(i.TotalAfterTax) > 0

ORDER BY TotalBalance DESC
";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
       
        public DataTable GetCustomersAging()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT
c.CustomerId,
c.Name AS CustomerName,

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) <= 30
THEN i.TotalAfterTax
ELSE 0
END
) AS [0_30],

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) BETWEEN 31 AND 60
THEN i.TotalAfterTax
ELSE 0
END
) AS [31_60],

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) BETWEEN 61 AND 90
THEN i.TotalAfterTax
ELSE 0
END
) AS [61_90],

SUM(
CASE 
WHEN DATEDIFF(DAY,i.InvoiceDate,GETDATE()) > 90
THEN i.TotalAfterTax
ELSE 0
END
) AS [90_Plus],

SUM(i.TotalAfterTax) AS TotalBalance

FROM Customers c

LEFT JOIN Invoices i
ON i.CustomerId = c.CustomerId
AND i.InvoiceType = 1

WHERE c.IsActive = 1

GROUP BY
c.CustomerId,
c.Name

ORDER BY TotalBalance DESC
";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public DataTable SearchCustomers(string text)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT TOP 50 CustomerId, Name
FROM Customers
WHERE Name LIKE @txt
ORDER BY Name";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@txt", "%" + text + "%");

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public DataTable SearchByCustomerId(int customerId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT *
FROM YourTable
WHERE CustomerId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", customerId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }

    }

}
