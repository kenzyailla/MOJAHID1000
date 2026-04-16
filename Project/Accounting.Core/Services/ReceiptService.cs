using Accounting.Core.Models;
using Accounting.Core.Repositories;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Accounting.Core.Enums;

namespace Accounting.Core.Services
{
    public class ReceiptService
    {
        private readonly ReceiptRepository _repo;
        private string _connectionString;

        public ReceiptService(string connectionString)
        {
            _connectionString = connectionString;
            _repo = new ReceiptRepository(connectionString);
        }

        // ===============================
        // إضافة سند قبض
        // ===============================
        public int AddReceipt(Receipt receipt)
        {
            // حفظ السند
            int receiptId = _repo.AddReceipt(receipt);

            // إنشاء القيد المحاسبي
            CreateReceiptJournal(receipt, receiptId);

            return receiptId;
        }


        // ===============================
        // جلب السندات
        // ===============================
        public List<Receipt> GetAllReceipts()
        {
            return _repo.GetAllReceipts();
        }

        // ===============================
        // حذف سند
        // ===============================
        public void DeleteReceipt(int receiptId)
        {
            _repo.DeleteReceipt(receiptId);
        }

        // ===============================
        // إضافة الشيكات
        // ===============================
        public void AddCheques(int detailId, List<Cheque> cheques)
        {
            _repo.AddCheques(detailId, cheques);
        }
        public Receipt GetReceiptById(int id)
        {
            return _repo.GetReceiptById(id);
        }
        private void CreateReceiptJournal(Receipt receipt, int receiptId)
        {
            JournalService journalService = new JournalService(_connectionString);

            JournalEntry entry = new JournalEntry
            {
                EntryDate = receipt.ReceiptDate,
                ReferenceType = "Receipt",
                ReferenceId = receiptId,
                Description = "سند قبض رقم " + receipt.ReceiptNumber
            };

            //-------------------------------------------------
            // 💵 النقد
            //-------------------------------------------------
            decimal cashAmount = receipt.Details
        .Where(d => d.PaymentMethod == PaymentMethod.Cash)
        .Sum(d => d.Amount);

            decimal chequesAmount = receipt.Details
                .Where(d => d.PaymentMethod == PaymentMethod.Cheque)
                .Sum(d => d.Amount);

            decimal totalAmount = cashAmount + chequesAmount;


            // النقد
            if (cashAmount > 0)
            {
                entry.Lines.Add(new JournalLine
                {
                    AccountId = 1,
                    Debit = cashAmount,
                    Credit = 0
                });
            }

            // الشيكات
            if (chequesAmount > 0)
            {
                entry.Lines.Add(new JournalLine
                {
                    AccountId = 9,
                    Debit = chequesAmount,
                    Credit = 0
                });
            }

            // ذمم العملاء
            entry.Lines.Add(new JournalLine
            {
                AccountId = 4,
                Debit = 0,
                Credit = totalAmount
            });


            // ⭐ تحقق توازن القيد
            if (entry.Lines.Sum(x => x.Debit) != entry.Lines.Sum(x => x.Credit))
                throw new Exception("القيد غير متوازن");

            journalService.AddJournalEntry(entry);

        }
        public int GetNextReceiptNumber()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(MAX(ReceiptNumber),0) + 1 FROM Receipts", con);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public bool ExistsReceiptNumber(string receiptNumber)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Receipts WHERE ReceiptNumber = @No", con);

                cmd.Parameters.AddWithValue("@No", receiptNumber);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
       
    }
}
