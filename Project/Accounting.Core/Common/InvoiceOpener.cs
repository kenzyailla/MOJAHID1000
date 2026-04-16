using Accounting.Core.Forms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Accounting.Core.Common
{
    public static class InvoiceOpener
    {
        public static void OpenBuyById(int id)
        {
            frm_BuyInvoiceEditor frm = new frm_BuyInvoiceEditor(id);
            frm.ShowDialog();
        }

        public static void OpenSaleById(int id)
        {
            frm_NewInvoice frm = new frm_NewInvoice(id);
            frm.ShowDialog();
        }

        // 🔥 الجديد
        public static void OpenReturnById(int id)
        {
            frm_SalesReturn frm = new frm_SalesReturn(id);
            frm.ShowDialog();
        }
    }
}
