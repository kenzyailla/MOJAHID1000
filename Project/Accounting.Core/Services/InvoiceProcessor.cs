using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Accounting.Core.EInvoice;
using System.Drawing;
using System.Data;
using System.Windows;
using System.IO;
using QRCoder;

namespace Accounting.Core.Services
{
    public class InvoiceProcessor
    {
        private string _cs;

        public InvoiceProcessor(string connectionString)
        {
            _cs = connectionString;
        }

        public string SellerName { get; set; }
        public string SellerTaxNo { get; set; }
        public async Task<InvoiceResponse> SendInvoiceToTaxAsync(int invoiceId)
        {
            // توليد UUID
            string uuid = UUIDGenerator.GenerateUUID();

            // توليد ICV
            ICVGenerator icvGen = new ICVGenerator(_cs);
            int icv = icvGen.GetNextICV();

            // توليد PreviousHash
            string previousHash = HashGenerator.GenerateHash(uuid + icv.ToString());

            // حفظ البيانات
            SaveInvoiceMeta(invoiceId, uuid, icv, previousHash);

            // توليد XML
            UBLInvoiceGenerator gen = new UBLInvoiceGenerator(_cs);
            string xml = gen.GenerateInvoiceXml(invoiceId, SellerName, SellerTaxNo);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\invoice_test.xml";
            File.WriteAllText(path, xml);
            MessageBox.Show("تم حفظ XML على سطح المكتب");

            // إرسال الفاتورة
            InvoiceResponse result = await ERTApiClient.SendXmlToERTAsync(xml);

            MessageBox.Show(result.Status);

            // التحقق من النجاح
            if (result != null && !string.IsNullOrEmpty(result.Uuid) && !string.IsNullOrEmpty(result.InvoiceNumber))
            {
                MessageBox.Show("تم الإرسال بنجاح - سيتم إنشاء QR");

                // استخدام QR الذي أعادته ERT مباشرة
                if (!string.IsNullOrEmpty(result.QrCode))
                {
                    // تحويل نص QR إلى صورة باستخدام QRCoder
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(result.QrCode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrImage = qrCode.GetGraphic(20);

                    // حفظ الصورة
                    string qrPath = SaveQRImage(qrImage, invoiceId);
                    MessageBox.Show($"تم حفظ QR في: {qrPath}");

                    // حفظ المسار في قاعدة البيانات
                    SaveInvoicePosting(invoiceId, result.Uuid, qrPath);
                }
                else
                {
                    MessageBox.Show("لم يتم استلام QR من ERT");
                }
            }
            else
            {
                MessageBox.Show("الإرسال فشل: " + result?.Message);
            }

            return result;
        }

        private string SaveQRImage(Bitmap qrImage, int invoiceId)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "InvoicesQR");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"QR_{invoiceId}_{DateTime.Now:yyyyMMddHHmmss}.png";
            string filePath = Path.Combine(folderPath, fileName);
            qrImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            return filePath;
        }

        private void SaveInvoiceQrOnly(int invoiceId, string qrPath)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
UPDATE Invoices
SET QRCodePath=@QR
WHERE InvoiceId=@Id", con);

                cmd.Parameters.AddWithValue("@QR", qrPath ?? "");
                cmd.Parameters.AddWithValue("@Id", invoiceId);

                cmd.ExecuteNonQuery();
            }
        }
        private void SaveInvoiceMeta(int invoiceId, string uuid, int icv, string previousHash)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
UPDATE Invoices
SET UUID=@UUID,
ICV=@ICV,
PreviousHash=@Hash
WHERE InvoiceId=@Id", con);

                cmd.Parameters.AddWithValue("@UUID", uuid);
                cmd.Parameters.AddWithValue("@ICV", icv);
                cmd.Parameters.AddWithValue("@Hash", previousHash);
                cmd.Parameters.AddWithValue("@Id", invoiceId);

                cmd.ExecuteNonQuery();
            }
        }
        private void SaveInvoicePosting(int invoiceId, string uuid, string qrPath)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(@"
UPDATE Invoices
SET UUID = @UUID,
    PostedToTax = 1,
    PostedDate = GETDATE(),
    QRCodePath = @QR
WHERE InvoiceId = @Id", con))
                {
                    cmd.Parameters.Add("@UUID", SqlDbType.NVarChar, 200).Value = uuid ?? "";
                    cmd.Parameters.Add("@QR", SqlDbType.NVarChar, 500).Value = qrPath ?? "";
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = invoiceId;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private (decimal totalAfterTax, decimal totalTax, DateTime date)
 GetInvoiceTotals(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT TotalAfterTax, TotalTax, InvoiceDate
FROM Invoices
WHERE InvoiceId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", invoiceId);

                SqlDataReader rd = cmd.ExecuteReader();

                if (!rd.Read())
                    throw new Exception("Invoice not found");

                return (
                    Convert.ToDecimal(rd["TotalAfterTax"]),
                    Convert.ToDecimal(rd["TotalTax"]),
                    Convert.ToDateTime(rd["InvoiceDate"])
                );
            }
        }
    }
}