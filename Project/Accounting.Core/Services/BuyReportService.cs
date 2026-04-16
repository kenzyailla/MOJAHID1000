using System;
using System.Data;
using Accounting.Core.Repositories;

namespace Accounting.Core.Services
{
    public class BuyReportService
    {
        private readonly BuyReportRepository _repo;

        public BuyReportService(string connectionString)
        {
            _repo = new BuyReportRepository(connectionString);
        }

        public DataTable GetBuyReportAdvanced(
            int? productId,
            int? supplierId,
            string invoiceNumber,
            decimal? taxRate,
            DateTime fromDate,
            DateTime toDate)
        {
            return _repo.GetBuyReportAdvanced(
                productId,
                supplierId,
                invoiceNumber,
                taxRate,
                fromDate,
                toDate);
        }
    }
}

