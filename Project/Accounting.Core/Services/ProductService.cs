using Accounting.Core.Models;
using Accounting.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Accounting.Core.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repo;
        private string _connectionString;

        public ProductService(string connectionString)
        {
            _connectionString = connectionString;
            _repo = new ProductRepository(connectionString);

        }

        public List<Product> GetAllProducts()
        {
            var products = _repo.GetAllProducts();

            InventoryService invService = new InventoryService(_connectionString);

            foreach (var p in products)
            {
                p.Balance = invService.GetProductBalance(p.ProductId);
            }

            return products;
        }
        public Product GetProductById(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT
    ProductId,
    Name,
    Unit,
    Price,
    TaxRate
FROM Products
WHERE ProductId = @Id
";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new Product
                    {
                        ProductId = (int)dr["ProductId"],
                        Name = dr["Name"].ToString(),
                        Unit = dr["Unit"].ToString(),
                        Price = (decimal)dr["Price"],
                        TaxRate = (decimal)dr["TaxRate"]
                    };
                }
            }

            return null;
        }

        public int AddProduct(Product product)
        {
            //            using (SqlConnection con = new SqlConnection(_connectionString))
            //            {
            //                con.Open();

            //                string sql = @"
            //INSERT INTO Products (Name, Unit, Price, TaxRate, CostPrice)
            //VALUES (@Name, @Unit, @Price, @Tax, @Cost)";

            //                using (SqlCommand cmd = new SqlCommand(sql, con))
            //                {
            //                    cmd.Parameters.AddWithValue("@Name", product.Name);
            //                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
            //                    cmd.Parameters.AddWithValue("@Price", product.Price);
            //                    cmd.Parameters.AddWithValue("@Tax", product.TaxRate);
            //                    cmd.Parameters.AddWithValue("@Cost", product.CostPrice);
            //                    cmd.ExecuteNonQuery();
            //                }
            //            }

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string sql = @"
INSERT INTO Products (Name, Unit, Price, TaxRate, CostPrice, IsActive)
VALUES (@Name, @Unit, @Price, @TaxRate, @CostPrice, 1);
SELECT SCOPE_IDENTITY();";   // إرجاع آخر معرف تم إدراجه

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@TaxRate", product.TaxRate);
                    cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice);

                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    product.ProductId = newId;
                    return newId;
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
    TaxRate=@Tax,
    CostPrice=@Cost
WHERE ProductId=@Id";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", product.ProductId);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Tax", product.TaxRate);
                    cmd.Parameters.AddWithValue("@Cost", product.CostPrice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            _repo.DeleteProduct(productId);
        }
    }
}

