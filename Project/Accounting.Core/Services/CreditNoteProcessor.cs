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
    public class CreditNoteProcessor
    {
        private string _cs;

        public CreditNoteProcessor(string connectionString)
        {
            _cs = connectionString;
        }

        public string SellerName { get; set; }
        public string SellerTaxNo { get; set; }
        public string ReasonNote { get; set; } = "مرتجع بضاعة"; // قيمة افتراضية
        public async Task<InvoiceResponse> SendCreditNoteToTaxAsync(int creditNoteId)
        {
            // توليد UUID
            string uuid = UUIDGenerator.GenerateUUID();

            // توليد ICV
            ICVGenerator icvGen = new ICVGenerator(_cs);
            int icv = icvGen.GetNextICV();

            // توليد PreviousHash (قد تحتاج إلى تعديله ليشمل معلومات المرتجع)
            string previousHash = HashGenerator.GenerateHash(uuid + icv.ToString());

            // حفظ البيانات في جدول CreditNotes
            SaveCreditNoteMeta(creditNoteId, uuid, icv, previousHash);

            // الحصول على originalInvoiceId من جدول CreditNotes
            int originalInvoiceId = GetOriginalInvoiceId(creditNoteId);

            // توليد XML
            UBLInvoiceGenerator gen = new UBLInvoiceGenerator(_cs);

            string xml = gen.GenerateCreditNoteXml(creditNoteId,  SellerName, SellerTaxNo, this.ReasonNote);
          
            // اختياري: حفظ XML للتشخيص
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\creditnote_test.xml";
            File.WriteAllText(path, xml);
            MessageBox.Show("تم حفظ XML على سطح المكتب");

            // إرسال XML
            InvoiceResponse result = await ERTApiClient.SendXmlToERTAsync(xml);

            MessageBox.Show(result.Status);

            if (result != null && !string.IsNullOrEmpty(result.Uuid) && !string.IsNullOrEmpty(result.InvoiceNumber))
            {
                MessageBox.Show("تم الإرسال بنجاح - سيتم إنشاء QR");

                if (!string.IsNullOrEmpty(result.QrCode))
                {
                    // تحويل نص QR إلى صورة
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(result.QrCode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrImage = qrCode.GetGraphic(6);

                    string qrPath = SaveQRImage(qrImage, creditNoteId);
                    MessageBox.Show($"تم حفظ QR في: {qrPath}");

                    // حفظ بيانات الإرسال في جدول CreditNotes (UUID, QR, Posted)
                    SaveCreditNotePosting(creditNoteId, result.Uuid, qrPath);
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

        private int GetOriginalInvoiceId(int creditNoteId)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT OriginalInvoiceId FROM CreditNotes WHERE CreditNoteId = @Id", con);
                cmd.Parameters.AddWithValue("@Id", creditNoteId);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void SaveCreditNoteMeta(int creditNoteId, string uuid, int icv, string previousHash)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE CreditNotes
                    SET UUID = @UUID,
                        ICV = @ICV,
                        PreviousHash = @Hash
                    WHERE CreditNoteId = @Id", con);

                cmd.Parameters.AddWithValue("@UUID", uuid);
                cmd.Parameters.AddWithValue("@ICV", icv);
                cmd.Parameters.AddWithValue("@Hash", previousHash);
                cmd.Parameters.AddWithValue("@Id", creditNoteId);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveCreditNotePosting(int creditNoteId, string uuid, string qrPath)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE CreditNotes
                    SET UUID = @UUID,
                        PostedToTax = 1,
                        PostedDate = GETDATE(),
                        QRCodePath = @QR
                    WHERE CreditNoteId = @Id", con))
                {
                    cmd.Parameters.Add("@UUID", SqlDbType.NVarChar, 200).Value = uuid ?? "";
                    cmd.Parameters.Add("@QR", SqlDbType.NVarChar, 500).Value = qrPath ?? "";
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = creditNoteId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string SaveQRImage(Bitmap qrImage, int creditNoteId)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CreditNotesQR");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"QR_CN_{creditNoteId}_{DateTime.Now:yyyyMMddHHmmss}.png";
            string filePath = Path.Combine(folderPath, fileName);
            qrImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            return filePath;
        }
    }
}