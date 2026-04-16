using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class ReturnLineDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }   // الكمية المرتجعة
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Before { get; set; }
        public decimal Tax { get; set; }
        public decimal After { get; set; }
    }
}
