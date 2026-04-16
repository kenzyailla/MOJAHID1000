using System;
using System.Data;
using Accounting.Core.Repositories;

namespace Accounting.Core.Services
{
    public class SalesReportService
    {
        private readonly SalesReportRepository _repo;

        public SalesReportService(string connectionString)
        {
            _repo = new SalesReportRepository(connectionString);
        }

        public DataTable GetSalesReportAdvanced(
            int? productId,
            int? customerId,
            string invoiceNumber,
            decimal? taxRate,

            DateTime fromDate,
            DateTime toDate)
        {
            return _repo.GetSalesReportAdvanced(productId, customerId, invoiceNumber, taxRate, fromDate, toDate);
        }
    }
}

