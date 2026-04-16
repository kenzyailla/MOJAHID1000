using Accounting.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Accounting.Core.Repositories
{
    public class ChequeRepository
    {
        private string _connectionString;

        public ChequeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ======================
        // Get All Cheques
        // ======================
      
        public DataTable GetAllCheques()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    c.ChequeId,
    c.ChequeNumber,
    c.BankName,
    c.DueDate,
    c.ChequeAmount,
    cs.StatusName,

    r.ReceiptNumber,

    CASE 
        WHEN r.PartyType = 1 THEN cust.Name
        WHEN r.PartyType = 2 THEN sup.Name
    END AS PartyName,

    CASE 
        WHEN r.PartyType = 1 THEN N'عميل'
        WHEN r.PartyType = 2 THEN N'مورد'
    END AS PartyTypeName

FROM Cheques c

INNER JOIN ReceiptDetails rd ON c.DetailId = rd.DetailId
INNER JOIN Receipts r ON rd.ReceiptId = r.ReceiptId

LEFT JOIN Customers cust 
    ON r.PartyType = 1 AND r.PartyId = cust.CustomerId

LEFT JOIN Suppliers sup 
    ON r.PartyType = 2 AND r.PartyId = sup.SupplierId

LEFT JOIN ChequeStatus cs 
    ON c.Status = cs.StatusId

ORDER BY c.DueDate
";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }


        // ======================
        // Update Cheque Status
        // ======================

        public void UpdateChequeStatus(int chequeId, int status)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
UPDATE Cheques
SET Status=@Status
WHERE ChequeId=@Id
";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", chequeId);

                cmd.ExecuteNonQuery();
            }
        }
        public ChequeInfo GetChequeInfo(int chequeId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    c.ChequeAmount,
    r.ReceiptId,
    r.PartyId
FROM Cheques c
JOIN ReceiptDetails rd ON c.DetailId = rd.DetailId
JOIN Receipts r ON rd.ReceiptId = r.ReceiptId
WHERE c.ChequeId=@Id
";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", chequeId);

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                    return null;

                return new ChequeInfo
                {
                    Amount = Convert.ToDecimal(dr["ChequeAmount"]),
                    ReceiptId = Convert.ToInt32(dr["ReceiptId"]),
                    PartyId = Convert.ToInt32(dr["PartyId"])
                };
            }
        }
        public class ChequeInfo
        {
            public decimal Amount { get; set; }
            public int ReceiptId { get; set; }
            public int PartyId { get; set; }
        }

    }

}
