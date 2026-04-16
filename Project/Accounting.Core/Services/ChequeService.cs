using Accounting.Core.Accounting.Core.Repositories;
using Accounting.Core.Enums;
using Accounting.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Services
{
   
    public class ChequeService
    {
        private string _connectionString;
        private ChequeRepository _repo;
        private JournalService _journalService;
        private readonly JournalService journalService;


        public ChequeService(string connectionString)
        {
            _repo = new ChequeRepository(connectionString);
            _journalService = new JournalService(connectionString);
            _connectionString = connectionString;
            journalService = new JournalService(connectionString);
            _repo = new ChequeRepository(connectionString);
        }

        public DataTable GetAllCheques()
        {
            return _repo.GetAllCheques();
        }

        public void UpdateChequeStatus(int chequeId, int status)
        {
            _repo.UpdateChequeStatus(chequeId, status);
        }
        public void CollectCheque(int chequeId, int bankAccountId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        //-------------------------------------------------
                        // جلب قيمة الشيك
                        //-------------------------------------------------

                        decimal amount = 0;

                        using (SqlCommand cmd = new SqlCommand(@"
SELECT ChequeAmount
FROM Cheques
WHERE ChequeId=@Id AND Status <> 2
", con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Id", chequeId);

                            var result = cmd.ExecuteScalar();

                            if (result == null)
                                throw new Exception("الشيك محصل مسبقاً");

                            amount = Convert.ToDecimal(result);
                        }

                        //-------------------------------------------------
                        // تحديث حالة الشيك
                        //-------------------------------------------------

                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE Cheques
SET Status = 2,
    CollectionDate = GETDATE()
WHERE ChequeId=@Id
", con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Id", chequeId);
                            cmd.ExecuteNonQuery();
                        }

                        //-------------------------------------------------
                        // إنشاء القيد المحاسبي
                        //-------------------------------------------------

                        JournalEntry entry = new JournalEntry
                        {
                            EntryDate = DateTime.Today,
                            ReferenceType = "ChequeCollection",
                            ReferenceId = chequeId,
                            Description = "تحصيل شيك رقم " + chequeId
                        };

                        // ⭐ الحساب المختار من المستخدم
                        entry.Lines.Add(new JournalLine
                        {
                            AccountId = bankAccountId,
                            Debit = amount,
                            Credit = 0
                        });

                        // ⭐ شيكات تحت التحصيل
                        entry.Lines.Add(new JournalLine
                        {
                            AccountId = 9,
                            Debit = 0,
                            Credit = amount
                        });

                        journalService.AddJournalEntry(con, trans, entry);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        public void ReturnCheque(int chequeId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
UPDATE Cheques
SET 
    Status = @Status,
    ReturnDate = GETDATE(),
    CollectionDate = NULL
WHERE ChequeId = @ChequeId
AND Status = 1
";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Status", (int)ChequeStatus.Returned);
                cmd.Parameters.AddWithValue("@ChequeId", chequeId);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    throw new Exception("لا يمكن إرجاع هذا الشيك");
            }
        }
        public DataTable SearchByChequeNumber(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT *
FROM Cheques
WHERE ChequeNumber LIKE @No";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@No", "%" + chequeNo + "%");

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public DataTable SearchByCustomerName(string name)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT c.*
FROM Cheques c
INNER JOIN Customers cu ON c.CustomerId = cu.CustomerId
WHERE cu.Name LIKE @Name";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Name", "%" + name + "%");

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public DataTable SearchOutgoingByChequeNo(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT *
FROM OutgoingCheques
WHERE ChequeNumber LIKE @No";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@No", "%" + chequeNo + "%");

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public DataTable SearchOutgoingBySupplierName(string name)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT c.*
FROM OutgoingCheques c
INNER JOIN Suppliers s ON c.SupplierId = s.SupplierId
WHERE s.Name LIKE @Name";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Name", "%" + name + "%");

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public DataTable GetChequesByCustomer(int customerId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT *
FROM Cheques
WHERE CustomerId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", customerId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public DataTable SearchChequesByCustomerName(string name)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT c.*
FROM Cheques c
INNER JOIN ReceiptDetails rd ON c.DetailId = rd.DetailId
INNER JOIN Receipts r ON rd.ReceiptId = r.ReceiptId
LEFT JOIN Customers cu ON r.PartyId = cu.CustomerId AND r.PartyType = 1
LEFT JOIN Suppliers s ON r.PartyId = s.SupplierId AND r.PartyType = 2
WHERE 
    (ISNULL(cu.Name,'') LIKE @Name 
     OR ISNULL(s.Name,'') LIKE @Name)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Name", "%" + name + "%");

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }

        public DataTable SearchOutgoingBySupplierId(int supplierId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT *
FROM OutgoingCheques
WHERE SupplierId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", supplierId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
    }

}
