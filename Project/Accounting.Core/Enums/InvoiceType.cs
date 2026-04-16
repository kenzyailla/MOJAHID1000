using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.Enums
{
    public enum InvoiceType
    {
        Sales = 1,
        Purchase = 2,
        SalesReturn = 3,
        PurchaseReturn = 4,
       ExemptSales = 5
    }
}

