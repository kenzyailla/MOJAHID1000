
using System.Collections.Generic;

namespace Accounting.Core.EInvoice
{
    public class InvoiceResponse
    {
        public string Status { get; set; }
        public string Uuid { get; set; }
        public string InvoiceNumber { get; set; }
        public string QrCode { get; set; }
        public string SignedInvoiceBase64 { get; set; }
        public string Message { get; set; }
        public string FullResponse { get; set; }
    }

    public class ValidationMessage
    {
        public string field { get; set; }
        public string message { get; set; }
    }
}