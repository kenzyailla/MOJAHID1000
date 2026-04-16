using Accounting.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Accounting.Core.Models
{
    public class BuyInvoice
    {
        public int BuyInvoiceId { get; set; }

        public int InvoiceNumber { get; set; }
        public int SupplierId { get; set; }

        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int PaymentType { get; set; } // 🔥 أضف هذا
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal TotalAfterTax { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string Notes { get; set; }

        public List<BuyInvoiceLine> Lines { get; set; } = new List<BuyInvoiceLine>();
    }
}
