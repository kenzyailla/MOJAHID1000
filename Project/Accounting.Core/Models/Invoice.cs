using System;
using System.Collections.Generic;

namespace Accounting.Core.Models
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int CustomerId { get; set; }

        public decimal TotalBeforeTax { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAfterTax { get; set; }

        public int InvoiceType { get; set; }
        public int PaymentType { get; set; }

        // ⭐ مهم جداً
        public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    }
}

