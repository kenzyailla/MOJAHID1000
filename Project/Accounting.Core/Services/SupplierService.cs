using Accounting.Core.Models;
using Accounting.Core.Repositories;
using System;
using System.Data;

namespace Accounting.Core.Services
{
    public class SupplierService
    {
        private readonly SupplierRepository _repo;
        private readonly string _connectionString;

        public SupplierService(string connectionString)
        {
            _repo = new SupplierRepository(connectionString);

            _connectionString = connectionString;
          
        }

        public DataTable GetAllSuppliers()
        {
            return _repo.GetAllSuppliers();
        }

        public Supplier GetSupplierById(int id)
        {
            return _repo.GetSupplierById(id);
        }

        public int AddSupplier(Supplier supplier)
        {
            return _repo.AddSupplierAndCreateAccount(supplier);
        }


        public void UpdateSupplier(Supplier supplier)
        {
            _repo.UpdateSupplier(supplier);
        }

        public void DeleteSupplier(int id)
        {
            _repo.DeleteSupplier(id);
        }

        public void AddSupplierWithOpeningBalance(
  Supplier supplier,
  decimal openingBalance,
  string balanceType)
        {
            int accountId = _repo.AddSupplierAndCreateAccount(supplier);

            if (openingBalance == 0)
                return;

            JournalRepository journalRepo = new JournalRepository(_connectionString);

            JournalEntry entry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                ReferenceType = "Opening",
                ReferenceId = 0,
                Description = "رصيد افتتاحي مورد - " + supplier.Name
            };

            if (balanceType == "مدين")
            {
                entry.Lines.Add(new JournalLine
                {
                    AccountId = accountId,
                    Debit = openingBalance,
                    Credit = 0
                });

                entry.Lines.Add(new JournalLine
                {
                    AccountId = 11, // رصيد افتتاحي
                    Debit = 0,
                    Credit = openingBalance
                });
            }
            else
            {
                entry.Lines.Add(new JournalLine
                {
                    AccountId = 11,
                    Debit = openingBalance,
                    Credit = 0
                });

                entry.Lines.Add(new JournalLine
                {
                    AccountId = accountId,
                    Debit = 0,
                    Credit = openingBalance
                });
            }

            journalRepo.AddJournalEntry(entry);
        }
    }
}
