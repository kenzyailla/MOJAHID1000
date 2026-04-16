using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class CustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetAllCustomers()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    CustomerId,
    Name,
    Phone,
    TaxNumber,
    IsActive,
    CreatedAt,
    AccountId
FROM Customers
WHERE IsActive = 1
ORDER BY CustomerId DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public int AddCustomer(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO Customers
(Name,Phone,TaxNumber,IsActive,CreatedAt)
VALUES
(@Name,@Phone,@Tax,1,GETDATE());

SELECT CAST(SCOPE_IDENTITY() AS INT);
";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? "");
                cmd.Parameters.AddWithValue("@Tax", customer.TaxNumber ?? "");

                return (int)cmd.ExecuteScalar();
            }
        }


        public void UpdateCustomer(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
UPDATE Customers
SET Name=@Name,
    Phone=@Phone,
    TaxNumber=@Tax
WHERE CustomerId=@Id
";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? "");
                    cmd.Parameters.AddWithValue("@Tax", customer.TaxNumber ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCustomer(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = "UPDATE Customers SET IsActive = 0 WHERE CustomerId=@Id";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int AddCustomerAndCreateAccount(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    //----------------------------------
                    // 1️⃣ إنشاء حساب جديد للعميل
                    //----------------------------------

                    string getMaxCode = @"
SELECT ISNULL(MAX(CAST(AccountCode AS INT)), 102000)
FROM Accounts
WHERE CAST(AccountCode AS INT) >= 102000
AND CAST(AccountCode AS INT) < 103000";

                    int newCode;

                    using (SqlCommand cmd = new SqlCommand(getMaxCode, con, trans))
                    {
                        newCode = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                    }

                    string insertAccount = @"
INSERT INTO Accounts
(AccountCode,AccountName,AccountType,IsActive)
VALUES
(@Code,@Name,'Asset',1);

SELECT SCOPE_IDENTITY();";

                    int accountId;

                    using (SqlCommand cmd = new SqlCommand(insertAccount, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Code", newCode.ToString());
                        cmd.Parameters.AddWithValue("@Name", "عميل - " + customer.Name);

                        accountId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //----------------------------------
                    // 2️⃣ إدخال العميل وربط الحساب
                    //----------------------------------

                    string insertCustomer = @"
INSERT INTO Customers
(Name,Phone,TaxNumber,IsActive,CreatedAt,AccountId)
VALUES
(@Name,@Phone,@Tax,@Active,@Date,@AccountId);

SELECT CAST(SCOPE_IDENTITY() AS INT);
";
                    int customerId;

                    using (SqlCommand cmd = new SqlCommand(insertCustomer, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Name", customer.Name);
                        cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? "");
                        cmd.Parameters.AddWithValue("@Tax", customer.TaxNumber ?? "");
                        cmd.Parameters.AddWithValue("@Active", true);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AccountId", accountId);

                        customerId = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                    trans.Commit();

                    return customerId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

}
