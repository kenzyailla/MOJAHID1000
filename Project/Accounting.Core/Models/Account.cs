using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string AccountType { get; set; }

        public bool IsActive { get; set; }
    }

}
