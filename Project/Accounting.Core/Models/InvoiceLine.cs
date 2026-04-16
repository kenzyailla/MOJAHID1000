namespace Accounting.Core.Models
{
    public class InvoiceLine
    {
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TotalBeforeTax { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalAfterTax { get; set; }

        public decimal LineTotal { get; set; }   // ⭐ أضف هذا

    }


}

