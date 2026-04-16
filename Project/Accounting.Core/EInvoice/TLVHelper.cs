using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace Accounting.Core.EInvoice
{
    public static class TLVHelper
    {
        public static string GenerateTLV(
            string sellerName,
            string taxNumber,
            DateTime invoiceDate,
            decimal totalAmount,
            decimal vatAmount)
        {
            MemoryStream stream = new MemoryStream();

            WriteTLV(stream, 1, sellerName);
            WriteTLV(stream, 2, taxNumber);
            WriteTLV(stream, 3, invoiceDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            WriteTLV(stream, 4, totalAmount.ToString("0.00", CultureInfo.InvariantCulture));
            WriteTLV(stream, 5, vatAmount.ToString("0.00", CultureInfo.InvariantCulture));

            return Convert.ToBase64String(stream.ToArray());
        }

        private static void WriteTLV(Stream stream, byte tag, string value)
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);

            stream.WriteByte(tag);
            stream.WriteByte((byte)valueBytes.Length);
            stream.Write(valueBytes, 0, valueBytes.Length);
        }
    }
}