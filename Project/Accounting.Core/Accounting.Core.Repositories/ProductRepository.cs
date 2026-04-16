using Accounting.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace Accounting.Core.Repositories
{
    public class ProductRepository
    {
        private readonly string _connectionString;
        private readonly object txtCostPrice;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        //        public List<Product> GetAllProducts()
        //        {
        //            List<Product> list = new List<Product>();

        //            using (SqlConnection con = new SqlConnection(_connectionString))
        //            {
        //                con.Open();

        //                string sql = @"
        //              SELECT
        //    p.ProductId,
        //    p.Name,
        //    p.Unit,
        //    p.Price,
        //    p.TaxRate,
        // p.CostPrice, 
        //    ISNULL(SUM(t.Quantity),0) AS Balance
        //FROM Products p
        //LEFT JOIN InventoryTransactions t
        //    ON p.ProductId = t.ProductId
        //WHERE p.IsActive = 1
        //GROUP BY
        //    p.ProductId,
        //    p.Name,
        //    p.Unit,
        //    p.Price,
        //    p.TaxRate

        //ORDER BY p.ProductId DESC
        //                ";

        //                SqlCommand cmd = new SqlCommand(sql, con);

        //                SqlDataReader dr = cmd.ExecuteReader();

        //                while (dr.Read())
        //                {
        //                    list.Add(new Product
        //                    {
        //                        ProductId = (int)dr["ProductId"],
        //                        Name = dr["Name"].ToString(),
        //                        Unit = dr["Unit"].ToString(),
        //                        Price = (decimal)dr["Price"],
        //                        TaxRate = (decimal)dr["TaxRate"],

        //                        Balance = (decimal)dr["Balance"]
        //                    });
        //                }
        //            }

        //            return list;
        //        }

        public List<Product> GetAllProducts()
        {
            List<Product> list = new List<Product>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
            SELECT
                p.ProductId,
                p.Name,
                p.Unit,
                p.Price,
                p.TaxRate,
                p.CostPrice,   -- اسم العمود الصحيح بعد التأكد
                ISNULL(SUM(t.Quantity), 0) AS Balance
            FROM Products p
            LEFT JOIN InventoryTransactions t ON p.ProductId = t.ProductId
            WHERE p.IsActive = 1
            GROUP BY
                p.ProductId,
                p.Name,
                p.Unit,
                p.Price,
                p.TaxRate,
                p.CostPrice
            ORDER BY p.ProductId DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Product
                    {
                        ProductId = (int)dr["ProductId"],
                        Name = dr["Name"].ToString(),
                        Unit = dr["Unit"].ToString(),
                        Price = (decimal)dr["Price"],
                        TaxRate = (decimal)dr["TaxRate"],
                        CostPrice = dr["CostPrice"] != DBNull.Value ? (decimal)dr["CostPrice"] : 0,
                        Balance = (decimal)dr["Balance"]
                    });
                }
            }

            return list;
        }

        public void AddProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO Products
(Name, Unit, Price, TaxRate, CostPrice, IsActive, CreatedAt)
VALUES
(@Name, @Unit, @Price, @Tax, @Cost, 1, GETDATE())
";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Tax", product.TaxRate);
                    cmd.Parameters.AddWithValue("@Cost", product.CostPrice);
                    cmd.ExecuteNonQuery();
                   
                }
            }
        }
        public void UpdateProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
            UPDATE Products
            SET Name=@Name,
                Unit=@Unit,
                Price=@Price,
                TaxRate=@Tax
            WHERE ProductId=@Id
        ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", product.ProductId);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Tax", product.TaxRate);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
            UPDATE Products
            SET IsActive = 0
            WHERE ProductId = @Id
        ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Product GetProductById(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = "SELECT * FROM Products WHERE Pro_ID=@Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    //return new Product
                    //{
                    //    ProductId = Convert.ToInt32(dr["Pro_ID"]),
                    //    Name = dr["Pro_Name"].ToString(),
                    //    Price = Convert.ToDecimal(dr["Buy_Price"]),
                    //    TaxRate = Convert.ToDecimal(dr["Tax_Value"])
                    //};

                    return new Product
                    {
                        ProductId = Convert.ToInt32(dr["Pro_ID"]),
                        Name = dr["Pro_Name"].ToString(),
                        Price = dr["Buy_Price"] == DBNull.Value ? 0 :
                Convert.ToDecimal(dr["Buy_Price"]),

                        TaxRate = dr["Tax_Value"] == DBNull.Value ? 0 :
                  Convert.ToDecimal(dr["Tax_Value"])
                    };

                }
            }

            return null;
        }

    }
}

