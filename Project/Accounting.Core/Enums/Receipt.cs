using System;
using System.Collections.Generic;
using Accounting.Core.Enums;

namespace Accounting.Core.Models
{
    public class Receipt
    {
        public int ReceiptId { get; set; }

        public string ReceiptNumber { get; set; }

        public DateTime ReceiptDate { get; set; }

        public PartyType PartyType { get; set; }

        public int PartyId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ReceiptDetail> Details { get; set; }
            = new List<ReceiptDetail>();
    }
}
