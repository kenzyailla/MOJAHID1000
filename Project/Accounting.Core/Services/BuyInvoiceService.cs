using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accounting.Core.Models;
using Accounting.Core.Repositories;

namespace Accounting.Core.Services
{
   

    public class BuyInvoiceService
    {
        private readonly string _connectionString;
        private readonly BuyInvoiceRepository _repo;

        public BuyInvoiceService(string connectionString)
        {
            _repo = new BuyInvoiceRepository(connectionString);
            _connectionString = connectionString;
        }
        public DataTable GetBuyInvoiceLines(int invoiceId)
        {
            return _repo.GetInvoiceLines(invoiceId);
        }

        public int AddBuyInvoice(BuyInvoice invoice)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 1️⃣ حفظ الفاتورة
                    int invoiceId = _repo.AddBuyInvoice(invoice);

                    // 2️⃣ جلب حساب المورد
                    int supplierAccountId = GetSupplierAccountId(con, trans, invoice.SupplierId);

                    // 3️⃣ إنشاء القيد
                    CreateJournalEntry(
                        con,
                        trans,
                        invoiceId,
                        invoice,
                        supplierAccountId
                    );

                    trans.Commit();
                    return invoiceId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        private int GetSupplierAccountId(SqlConnection con, SqlTransaction trans, int supplierId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", supplierId);

                object result = cmd.ExecuteScalar();
                if (result == null)
                    throw new Exception("المورد غير مرتبط بحساب محاسبي");

                return Convert.ToInt32(result);
            }
        }


        private void CreateJournalEntry(
    SqlConnection con,
    SqlTransaction trans,
    int invoiceId,
    BuyInvoice invoice,
    int supplierAccountId)
        {
            //-----------------------------------
            // إنشاء كائن القيد
            //-----------------------------------

            JournalEntry entry = new JournalEntry
            {
                EntryDate = invoice.InvoiceDate,
                ReferenceType = "BuyInvoice",
                ReferenceId = invoiceId,
                Description = "فاتورة مشتريات رقم " + invoice.InvoiceNumber
            };

            //-----------------------------------
            // مدين: المخزون
            //-----------------------------------
            entry.Lines.Add(new JournalLine
            {
                AccountId = 8, // المخزون
                Debit = invoice.SubTotal,
                Credit = 0
            });

            //-----------------------------------
            // مدين: ضريبة مدخلات
            //-----------------------------------
            if (invoice.TaxTotal > 0)
            {
                entry.Lines.Add(new JournalLine
                {
                    AccountId = 5, // ضريبة مدخلات
                    Debit = invoice.TaxTotal,
                    Credit = 0
                });
            }

            //-----------------------------------
            // دائن: حساب المورد
            //-----------------------------------
            entry.Lines.Add(new JournalLine
            {
                AccountId = supplierAccountId,
                Debit = 0,
                Credit = invoice.TotalAfterTax
            });

            //-----------------------------------
            // إدخال القيد
            //-----------------------------------

            JournalRepository repo = new JournalRepository(_connectionString);

            repo.AddJournalEntry(con, trans, entry);
        }

        public DataTable GetAllBuyInvoices()
        {
            return _repo.GetAllBuyInvoices();
        }
        public DataRow GetBuyInvoiceHeader(int id)
        {
            return _repo.GetBuyInvoiceHeader(id);
        }

      

        public void UpdateBuyInvoice(int id, BuyInvoice invoice)
        {
            _repo.UpdateBuyInvoice(id, invoice);
        }
      
      

    }
}
