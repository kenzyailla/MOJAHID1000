using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public decimal Price { get; set; }

        public decimal TaxRate { get; set; }

        public decimal Balance { get; set; }

        public decimal CostPrice { get; set; }
        public decimal CurrentStock { get; set; }

    }
}
