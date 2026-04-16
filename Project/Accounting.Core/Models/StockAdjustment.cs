using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Models
{
    public class StockAdjustment
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}