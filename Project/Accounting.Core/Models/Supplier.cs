using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string TaxNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

      
    }
}