using System.Collections.Generic;
using Accounting.Core.Enums;

namespace Accounting.Core.Models
{
    public class ReceiptDetail
    {
        public int DetailId { get; set; }

        public int ReceiptId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string BankAccount { get; set; }

        public List<Cheque> Cheques { get; set; }
            = new List<Cheque>();
    }
}

