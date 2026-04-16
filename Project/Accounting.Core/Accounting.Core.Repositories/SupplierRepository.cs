using Accounting.Core.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Repositories
{
    public class SupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetAllSuppliers()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM Suppliers ORDER BY SupplierId DESC";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public Supplier GetSupplierById(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM Suppliers WHERE SupplierId=@Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Supplier
                    {
                        SupplierId = (int)reader["SupplierId"],
                        Name = reader["Name"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        TaxNumber = reader["TaxNumber"].ToString(),
                        IsActive = (bool)reader["IsActive"],
                        CreatedAt = (DateTime)reader["CreatedAt"]
                    };
                }

                return null;
            }
        }

        public int AddSupplier(Supplier supplier)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO Suppliers
(Name, Phone, TaxNumber, IsActive, CreatedAt)
VALUES
(@Name, @Phone, @TaxNumber, @IsActive, @CreatedAt);
SELECT SCOPE_IDENTITY();
";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@Name", supplier.Name);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone ?? "");
                cmd.Parameters.AddWithValue("@TaxNumber", supplier.TaxNumber ?? "");
                cmd.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        public void UpdateSupplier(Supplier supplier)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
UPDATE Suppliers SET
Name=@Name,
Phone=@Phone,
TaxNumber=@TaxNumber,
IsActive=@IsActive
WHERE SupplierId=@Id";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@Name", supplier.Name);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone ?? "");
                cmd.Parameters.AddWithValue("@TaxNumber", supplier.TaxNumber ?? "");
                cmd.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                cmd.Parameters.AddWithValue("@Id", supplier.SupplierId);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSupplier(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string sql = "DELETE FROM Suppliers WHERE SupplierId=@Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
            }
        }
        public int CreateSupplierAccount(string supplierName)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                //-----------------------------------------
                // جلب أعلى حساب يبدأ بـ 201
                //-----------------------------------------
                string sqlMax = @"
SELECT ISNULL(MAX(CAST(AccountCode AS INT)), 201000)
FROM Accounts
WHERE LEFT(AccountCode,3) = '201'";

                int lastCode;

                using (SqlCommand cmd = new SqlCommand(sqlMax, con))
                {
                    lastCode = Convert.ToInt32(cmd.ExecuteScalar());
                }

                //-----------------------------------------
                // إنشاء الرقم الجديد
                //-----------------------------------------
                int newCode = lastCode + 1;

                //-----------------------------------------
                // إدخال الحساب
                //-----------------------------------------
                string sqlInsert = @"
INSERT INTO Accounts
(AccountCode, AccountName, AccountType, IsActive)
VALUES
(@Code, @Name, 'Liability', 1);

SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sqlInsert, con))
                {
                    cmd.Parameters.AddWithValue("@Code", newCode.ToString());
                    cmd.Parameters.AddWithValue("@Name", "مورد - " + supplierName);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }



        public int AddSupplierAndCreateAccount(Supplier supplier)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    //----------------------------------
                    // 1️⃣ إنشاء حساب جديد للمورد
                    //----------------------------------
                    string getMaxCode = @"
SELECT ISNULL(MAX(CAST(AccountCode AS INT)), 201)
FROM Accounts
WHERE CAST(AccountCode AS INT) > 201
AND CAST(AccountCode AS INT) < 300";

                   
                    int newCode;

                    using (SqlCommand cmd = new SqlCommand(getMaxCode, con, trans))
                    {
                        newCode = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                    }

                    string insertAccount = @"
INSERT INTO Accounts
(AccountCode,AccountName,AccountType,IsActive)
VALUES
(@Code,@Name,'Liability',1);

SELECT SCOPE_IDENTITY();";

                    int accountId;

                    using (SqlCommand cmd = new SqlCommand(insertAccount, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Code", newCode.ToString());
                        cmd.Parameters.AddWithValue("@Name", supplier.Name);

                        accountId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //----------------------------------
                    // 2️⃣ إدخال المورد مع ربط الحساب
                    //----------------------------------

                    string insertSupplier = @"
INSERT INTO Suppliers
(Name,Phone,TaxNumber,IsActive,CreatedAt,AccountId)
VALUES
(@Name,@Phone,@Tax,@Active,@Date,@AccountId)";

                    using (SqlCommand cmd = new SqlCommand(insertSupplier, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Name", supplier.Name);
                        cmd.Parameters.AddWithValue("@Phone", supplier.Phone ?? "");
                        cmd.Parameters.AddWithValue("@Tax", supplier.TaxNumber ?? "");
                        cmd.Parameters.AddWithValue("@Active", supplier.IsActive);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AccountId", accountId);

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return accountId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        private int GetNextAccountCode(SqlConnection con, SqlTransaction trans)
        {
            string sql = "SELECT ISNULL(MAX(CAST(AccountCode AS INT)), 200000) FROM Accounts";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                int maxCode = Convert.ToInt32(cmd.ExecuteScalar());
                return maxCode + 1;
            }
        }
      


    }
}



