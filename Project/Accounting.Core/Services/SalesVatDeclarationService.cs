using Accounting.Core.Accounting.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Services
{
    public class SalesVatDeclarationService
    {
        private readonly SalesVatDeclarationRepository _repo;

        public SalesVatDeclarationService(string connectionString)
        {
            _repo = new SalesVatDeclarationRepository(connectionString);
        }

        public DataTable GetSalesVatDeclaration(DateTime fromDate, DateTime toDate)
        {
            return _repo.GetSalesVatDeclaration(fromDate, toDate);
        }
    }
}
