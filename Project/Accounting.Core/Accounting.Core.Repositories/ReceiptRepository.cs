using System;
using System.Data;
using System.Data.SqlClient;
using Accounting.Core.Models;
using System.Collections.Generic;
using Accounting.Core.Enums;

namespace Accounting.Core.Repositories
{
    public class ReceiptRepository
    {
        private readonly string _connectionString;

        public ReceiptRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int AddReceipt(Receipt receipt)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // =====================
                        // Insert Receipt
                        // =====================

                        string insertReceipt = @"
INSERT INTO Receipts
(ReceiptNumber,ReceiptDate,PartyType,PartyId,TotalAmount,Notes)
VALUES
(@Number,@Date,@PartyType,@PartyId,@Total,@Notes);

SELECT SCOPE_IDENTITY();
";

                        int receiptId;

                        using (SqlCommand cmd = new SqlCommand(insertReceipt, con, trans))
                        {
                            cmd.Parameters.Add("@Number", SqlDbType.NVarChar, 50).Value = receipt.ReceiptNumber;
                            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = receipt.ReceiptDate;
                            cmd.Parameters.Add("@PartyType", SqlDbType.Int).Value = (int)receipt.PartyType;
                            cmd.Parameters.Add("@PartyId", SqlDbType.Int).Value = receipt.PartyId;

                            var totalParam = cmd.Parameters.Add("@Total", SqlDbType.Decimal);
                            totalParam.Precision = 18;
                            totalParam.Scale = 5;
                            totalParam.Value = receipt.TotalAmount;

                            cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 500)
                                .Value = (object)receipt.Notes ?? DBNull.Value;

                            receiptId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // =====================
                        // Insert Details
                        // =====================

                        foreach (var detail in receipt.Details)
                        {
                            string insertDetail = @"
INSERT INTO ReceiptDetails
(ReceiptId, PaymentMethod, Amount, BankAccount)
VALUES
(@ReceiptId, @Method, @Amount, @Bank);
SELECT SCOPE_IDENTITY();
";

                            int detailId;

                            using (SqlCommand cmd = new SqlCommand(insertDetail, con, trans))
                            {
                                cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                                cmd.Parameters.AddWithValue("@Method", (int)detail.PaymentMethod);
                                cmd.Parameters.AddWithValue("@Amount", detail.Amount);
                                cmd.Parameters.AddWithValue("@Bank", detail.BankAccount ?? "");

                                detailId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            foreach (var cheque in detail.Cheques)
                            {
                                string insertCheque = @"
INSERT INTO Cheques
(DetailId, ChequeNumber, BankName, DueDate, ChequeAmount, Status)
VALUES
(@Detail,@Number,@Bank,@Due,@Amount,@Status)
";

                                using (SqlCommand cmd = new SqlCommand(insertCheque, con, trans))
                                {
                                    cmd.Parameters.AddWithValue("@Detail", detailId);
                                    cmd.Parameters.AddWithValue("@Number", cheque.ChequeNumber);
                                    cmd.Parameters.AddWithValue("@Bank", cheque.BankName);
                                    cmd.Parameters.AddWithValue("@Due", cheque.DueDate);
                                    cmd.Parameters.AddWithValue("@Amount", cheque.ChequeAmount);
                                    cmd.Parameters.AddWithValue("@Status", (int)cheque.Status);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }


                        trans.Commit();
                        return receiptId;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Receipt> GetAllReceipts()
        {
            List<Receipt> list = new List<Receipt>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
            SELECT *
            FROM Receipts
            ORDER BY ReceiptDate DESC
        ";

                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Receipt r = new Receipt();

                    r.ReceiptId = Convert.ToInt32(dr["ReceiptId"]);
                    r.ReceiptNumber = dr["ReceiptNumber"].ToString();
                    r.ReceiptDate = Convert.ToDateTime(dr["ReceiptDate"]);
                    r.PartyType = (PartyType)Convert.ToInt32(dr["PartyType"]);

                    r.PartyId = Convert.ToInt32(dr["PartyId"]);
                    r.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    r.Notes = dr["Notes"]?.ToString();

                    list.Add(r);
                }
            }

            return list;
        }
        public void DeleteReceipt(int receiptId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = "DELETE FROM Receipts WHERE ReceiptId=@Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", receiptId);

                cmd.ExecuteNonQuery();
            }
        }
        public void AddCheques(int detailId, List<Cheque> cheques)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                foreach (var ch in cheques)
                {
                    string sql = @"
                INSERT INTO Cheques
                (DetailId,ChequeNumber,BankName,DueDate,ChequeAmount,Status)
                VALUES
                (@Detail,@Number,@Bank,@Date,@Amount,@Status)
            ";

                    SqlCommand cmd = new SqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@Detail", detailId);
                    cmd.Parameters.AddWithValue("@Number", ch.ChequeNumber);
                    cmd.Parameters.AddWithValue("@Bank", ch.BankName);
                    cmd.Parameters.AddWithValue("@Date", ch.DueDate);
                    cmd.Parameters.AddWithValue("@Amount", ch.ChequeAmount);
                    cmd.Parameters.AddWithValue("@Status", ch.Status);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Receipt GetReceiptById(int receiptId)
        {
            Receipt receipt = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // ======================
                // Header
                // ======================

                string sqlReceipt = "SELECT * FROM Receipts WHERE ReceiptId=@Id";

                using (SqlCommand cmd = new SqlCommand(sqlReceipt, con))
                {
                    cmd.Parameters.AddWithValue("@Id", receiptId);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        receipt = new Receipt
                        {
                            ReceiptId = Convert.ToInt32(dr["ReceiptId"]),
                            ReceiptNumber = dr["ReceiptNumber"].ToString(),
                            ReceiptDate = Convert.ToDateTime(dr["ReceiptDate"]),
                            PartyType = (PartyType)Convert.ToInt32(dr["PartyType"]),
                            PartyId = Convert.ToInt32(dr["PartyId"]),
                            TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                            Notes = dr["Notes"]?.ToString()
                        };
                    }

                    dr.Close();
                }

                if (receipt == null)
                    return null;

                // ======================
                // Details
                // ======================

                string sqlDetails = "SELECT * FROM ReceiptDetails WHERE ReceiptId=@Id";

                using (SqlCommand cmd = new SqlCommand(sqlDetails, con))
                {
                    cmd.Parameters.AddWithValue("@Id", receiptId);

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        ReceiptDetail detail = new ReceiptDetail
                        {
                            DetailId = Convert.ToInt32(dr["DetailId"]),
                            ReceiptId = receiptId,
                            Amount = Convert.ToDecimal(dr["Amount"])
                        };

                        receipt.Details.Add(detail);
                    }

                    dr.Close();
                }

                // ======================
                // Cheques
                // ======================

                foreach (var detail in receipt.Details)
                {
                    string sqlCheques = "SELECT * FROM Cheques WHERE DetailId=@Id";

                    using (SqlCommand cmd = new SqlCommand(sqlCheques, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", detail.DetailId);

                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            Cheque cheque = new Cheque
                            {
                                ChequeId = Convert.ToInt32(dr["ChequeId"]),
                                DetailId = detail.DetailId,
                                ChequeNumber = dr["ChequeNumber"].ToString(),
                                BankName = dr["BankName"]?.ToString(),
                                DueDate = Convert.ToDateTime(dr["DueDate"]),
                                ChequeAmount = Convert.ToDecimal(dr["ChequeAmount"]),
                                Status = (ChequeStatus)Convert.ToInt32(dr["Status"])
                            };

                            detail.Cheques.Add(cheque);
                        }

                        dr.Close();
                    }
                }
            }

            return receipt;
        }


    }
}