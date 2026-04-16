using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class SalesReturnLineTemp
    {
        public int ProductId { get; set; }
        public int? InvoiceLineId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal BeforeTax { get; set; }
        public decimal Tax { get; set; }
        public decimal AfterTax { get; set; }
    }
}
