using System;
using System.Data;
using Accounting.Core.Repositories;

namespace Accounting.Core.Services
{
    public class VatSummaryService
    {
        private readonly VatSummaryRepository _repo;

        public VatSummaryService(string connectionString)
        {
            _repo = new VatSummaryRepository(connectionString);
        }

        public DataTable GetVatSummary(DateTime fromDate, DateTime toDate)
        {
            return _repo.GetVatSummary(fromDate, toDate);
        }
    }
}

