using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Core.EInvoice
{
    public static class EInvoiceConfig
    {
        public static string ClientId = Properties.Settings.Default.clientId;


        public static string SecretKey = Properties.Settings.Default.secretKey;

        public static string ApiUrl =
        "https://backend.jofotara.gov.jo/core/invoices/";
    }
}
