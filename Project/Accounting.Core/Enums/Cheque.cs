using System;
using Accounting.Core.Enums;

namespace Accounting.Core.Models
{
    public class Cheque
    {
        public int ChequeId { get; set; }

        public int DetailId { get; set; }

        public string ChequeNumber { get; set; }

        public string BankName { get; set; }

        public DateTime DueDate { get; set; }

        public decimal ChequeAmount { get; set; }

        public ChequeStatus Status { get; set; }

        public DateTime? CollectionDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}

