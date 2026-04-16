using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Accounting.Core.Enums;
using Accounting.Core.Models;

namespace Accounting.Core.Services
{
    public class InventoryService
    {
        private readonly string _connectionString;

        public InventoryService(string connectionString)
        {
            _connectionString = connectionString;
        }
        private readonly string connectionString =
@"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";
        // =========================
        // Add Single Stock Movement
        // =========================
        public void AddStockTransaction(
       SqlConnection con,
    SqlTransaction trans,
    int productId,
    int referenceId,      // InvoiceId
    decimal signedQty,    // سالب للمبيعات - موجب للمشتريات
    int transactionType)  // نفس InvoiceType (1/2/3/4)
        {
            string sql = @"
INSERT INTO InventoryTransactions
(ProductId, TransactionDate, TransactionType, Quantity, ReferenceId)
VALUES
(@ProductId, GETDATE(), @Type, @Qty, @RefId)
;";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Qty", signedQty);
                cmd.Parameters.AddWithValue("@Type", transactionType);
                cmd.Parameters.AddWithValue("@RefId", referenceId);
                cmd.ExecuteNonQuery();
            }
        }


        // =========================
        // Add Multiple Movements
        // =========================
        public void AddStockAdjustments(
      SqlConnection con,
      SqlTransaction trans,
      List<StockAdjustment> adjustments,
      int referenceId,
      int transactionType)
        {
            string sql = @"
INSERT INTO InventoryTransactions
(ProductId, Quantity, TransactionType, ReferenceId, TransactionDate)
VALUES
(@ProductId, @Qty, @Type, @RefId, GETDATE());";

            foreach (var adj in adjustments)
            {
                using (SqlCommand cmd = new SqlCommand(sql, con, trans))
                {
                    cmd.Parameters.AddWithValue("@ProductId", adj.ProductId);
                    cmd.Parameters.AddWithValue("@Qty", adj.Quantity);
                    cmd.Parameters.AddWithValue("@Type", transactionType);
                    cmd.Parameters.AddWithValue("@RefId", referenceId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // =========================
        // Validate Stock Before Sale
        // =========================
        public void ValidateStockAvailability(
            SqlConnection con,
            SqlTransaction trans,
            List<StockAdjustment> adjustments)
        {
            foreach (var adj in adjustments)
            {
                if (adj.Quantity >= 0)
                    continue;

                decimal balance = GetProductBalance(con, trans, adj.ProductId);

                if (balance < Math.Abs(adj.Quantity))
                {
                    throw new Exception(
                        $"الرصيد غير كافي للمنتج رقم {adj.ProductId} — المتوفر {balance}"
                    );
                }
            }
        }

        // =========================
        // Remove Stock Movements (For Invoice Update)
        // =========================
        public void RemoveInvoiceStockTransactions(
    SqlConnection con,
    SqlTransaction trans,
    int invoiceId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "DELETE FROM InventoryTransactions WHERE ReferenceId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", invoiceId);
                cmd.ExecuteNonQuery();
            }
        }


        // =========================
        // Helper
        // =========================
        private int GetTransactionType(int invoiceType)
        {
            return invoiceType;
        }

        public decimal GetProductBalance(int productId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
            SELECT ISNULL(SUM(Quantity),0)
            FROM InventoryTransactions
            WHERE ProductId = @Product
        ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Product", productId);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }
        public void InsertInventoryTransaction(
         SqlConnection con,
         SqlTransaction trans,
         int productId,
         decimal qty,
         int transactionType,
         int referenceId,
         int? invoiceId = null,
         string notes = null)
        {
            string sql = @"
INSERT INTO InventoryTransactions
(ProductId, Quantity, TransactionType, ReferenceId, InvoiceId, TransactionDate, Notes)
VALUES
(@ProductId, @Qty, @Type, @RefId, @InvoiceId, GETDATE(), @Notes);";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@Type", transactionType);
                cmd.Parameters.AddWithValue("@RefId", referenceId);
                cmd.Parameters.AddWithValue("@InvoiceId", (object)invoiceId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
        public decimal GetProductBalance(
    SqlConnection con,
    SqlTransaction trans,
    int productId)
        {
            string sql = @"
SELECT ISNULL(SUM(Quantity),0)
FROM InventoryTransactions
WHERE ProductId = @Product";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@Product", productId);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
        public DataTable GetStockBalances()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    p.ProductId,
    p.Name AS ProductName,
    ISNULL(SUM(t.Quantity),0) AS Balance
FROM Products p
LEFT JOIN InventoryTransactions t 
    ON p.ProductId = t.ProductId
GROUP BY p.ProductId, p.Name
ORDER BY p.Name
";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        public DataTable GetInventoryMasterReport(DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"

SELECT p.ProductId, p.Name AS ProductName, ISNULL(SUM(CASE WHEN t.TransactionDate < @FromDate THEN t.Quantity ELSE 0 END),0) AS OpeningBalance, ISNULL(SUM(CASE WHEN t.TransactionType = 1 AND t.TransactionDate BETWEEN @FromDate AND @ToDate THEN t.Quantity ELSE 0 END),0) AS Purchases, ISNULL(SUM(CASE WHEN t.TransactionType = 2 AND t.TransactionDate BETWEEN @FromDate AND @ToDate THEN ABS(t.Quantity) ELSE 0 END),0) AS Sales, ISNULL(SUM(CASE WHEN t.TransactionType = 3 AND t.TransactionDate BETWEEN @FromDate AND @ToDate THEN t.Quantity ELSE 0 END),0) AS SalesReturns, ISNULL(SUM(CASE WHEN t.TransactionType = 4 AND t.TransactionDate BETWEEN @FromDate AND @ToDate THEN ABS(t.Quantity) ELSE 0 END),0) AS PurchaseReturns, ISNULL(SUM(t.Quantity),0) AS CurrentBalance FROM Products p LEFT JOIN InventoryTransactions t ON p.ProductId = t.ProductId GROUP BY p.ProductId, p.Name ORDER BY p.Name
";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public void AddOpeningStock(
    SqlConnection con,
    SqlTransaction trans,
    int productId,
    decimal openingQty,
    string notes = null)
        {
            // openingQty لازم تكون موجبة
            if (openingQty <= 0)
                throw new Exception("الكمية الافتتاحية يجب أن تكون أكبر من صفر");

            string sql = @"
INSERT INTO InventoryTransactions
(ProductId, Quantity, TransactionType, ReferenceId, TransactionDate, Notes)
VALUES
(@ProductId, @Qty, @Type, NULL, GETDATE(), @Notes);";

            using (SqlCommand cmd = new SqlCommand(sql, con, trans))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Qty", openingQty);
                cmd.Parameters.AddWithValue("@Type", 0); // Opening
                cmd.Parameters.AddWithValue("@Notes", (object)(notes ?? "رصيد افتتاحي") ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
        public void AddInventoryTransaction(int productId, decimal qty, int type, int refId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO InventoryTransactions
(ProductId, TransactionDate, TransactionType, Quantity, ReferenceId)
VALUES
(@ProductId, GETDATE(), @Type, @Qty, @RefId)";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@RefId", refId);

                cmd.ExecuteNonQuery();
            }
        }
        public void AddInventoryTransaction(int productId, decimal qty, decimal costPrice, int type, int? refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO InventoryTransactions
(ProductId, Quantity, CostPrice, TransactionType, ReferenceId, TransactionDate)
VALUES
(@p, @q, @c, @t, @r, GETDATE())";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@p", productId);
                cmd.Parameters.AddWithValue("@q", qty);
                cmd.Parameters.AddWithValue("@c", costPrice);
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@r", (object)refId ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
        private decimal GetOpeningCostPrice(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT TOP 1 CostPrice
FROM InventoryTransactions
WHERE ProductId = @p
  AND TransactionType = 0
  AND CostPrice IS NOT NULL
ORDER BY TransactionDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@p", productId);

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return 0;

                    return Convert.ToDecimal(result);
                }
            }
        }
    }
}
