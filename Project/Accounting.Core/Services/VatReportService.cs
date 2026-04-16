using System;
using System.Data;
using Accounting.Core.Repositories;

namespace Accounting.Core.Services
{
    public class VatReportService
    {
        private readonly VatReportRepository _repo;

        public VatReportService(string connectionString)
        {
            _repo = new VatReportRepository(connectionString);
        }

        public DataTable GetVatSummary(DateTime fromDate, DateTime toDate)
        {
            return _repo.GetVatSummary(fromDate, toDate);
        }
    }
}
