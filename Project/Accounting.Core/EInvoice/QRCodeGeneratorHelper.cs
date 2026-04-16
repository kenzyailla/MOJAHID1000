using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Accounting.Core.EInvoice
{
    public static class QRCodeGeneratorHelper
    {
        public static Bitmap GenerateQR(string text)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qr = new QRCode(data);

            return qr.GetGraphic(20);
        }

        public static string SaveQR(Bitmap image, int invoiceId)
        {
            string folder = @"C:\InvoicesQR";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, "INV_" + invoiceId + ".png");

            image.Save(path, ImageFormat.Png);

            return path;
        }
    }
}
