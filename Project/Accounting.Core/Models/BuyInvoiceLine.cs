using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class BuyInvoiceLine
    {
        public int BuyInvoiceLineId { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal LineSubTotal { get; set; }

        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }

        public decimal LineTotal { get; set; }
    }
}
