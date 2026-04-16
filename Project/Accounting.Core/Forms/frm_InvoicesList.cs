using Accounting.Core.EInvoice;
using Accounting.Core.Reportingcrystal;
using Accounting.Core.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_InvoicesList : Form
    {
        string connectionString =
        @"Data Source=.\SQLEXPRESS;
        Initial Catalog=AccountingCoreDB;
        Integrated Security=True";

        public frm_InvoicesList()
        {
            InitializeComponent();
            CustomizeDataGridView(dgvInvoices);
        }
        private void CustomizeDataGridView(DataGridView dgv)
        {

            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;

            // خطوط الصفوف
            dgv.RowTemplate.Height = 30;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f0f8ff");
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#ADD8E6");
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // محاذاة الصفوف والأعمدة
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // خيارات إضافية
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.GridColor = ColorTranslator.FromHtml("#e0e0e0");

            // بعد إضافة الأعمدة أو ملء البيانات، طبق الخط
            Font notoFont = new Font("Noto Kufi Arabic", 10.18868f);

            // رؤوس الأعمدة
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Noto Kufi Arabic", 10.18868f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // لكل الأعمدة
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.DefaultCellStyle.Font = notoFont;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
        private void frm_InvoicesList_Load(object sender, EventArgs e)
        {
            LoadInvoices();
            //dgvInvoices.CellDoubleClick += dgvInvoices_CellDoubleClick;
            SetupButtonsAppearance();
        }
        private void LoadInvoices()
        {
            InvoiceService service =
                new InvoiceService(connectionString);

            DataTable dt = service.GetInvoices();

            dgvInvoices.DataSource = dt;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frm_NewInvoice frm = new frm_NewInvoice();
            frm.ShowDialog();

            LoadInvoices();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.CurrentRow == null)
                return;

            int id = Convert.ToInt32(dgvInvoices.CurrentRow.Cells["InvoiceId"].Value);

                     frm_InvoiceEditor f =
                new frm_InvoiceEditor(id);

            f.ShowDialog();

            LoadInvoices();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
              
         
                try
                {
                    if (dgvInvoices.CurrentRow == null)
                    {
                        MessageBox.Show("اختر فاتورة");
                        return;
                    }

                    int invoiceId = Convert.ToInt32(
                        dgvInvoices.CurrentRow.Cells["InvoiceId"].Value
                    );

                    InvoiceProcessor processor = new InvoiceProcessor(connectionString);

                    processor.SellerName = "مؤسسة يزن للتجهيزات العلمية";
                    processor.SellerTaxNo = "1160788";

                    var result = await processor.SendInvoiceToTaxAsync(invoiceId);

                    if (!string.IsNullOrEmpty(result.Uuid) && !string.IsNullOrEmpty(result.InvoiceNumber))
                    {
                        MessageBox.Show(
                            $"تم إرسال الفاتورة بنجاح.\nرقم الفاتورة: {result.InvoiceNumber}\nUUID: {result.Uuid}",
                            "نتيجة الإرسال"
                        );

                        if (!string.IsNullOrEmpty(result.SignedInvoiceBase64))
                        {
                            string signedXml = ERTApiClient.DecodeBase64ToString(result.SignedInvoiceBase64);

                            string folderPath = Path.Combine(Application.StartupPath, "SignedInvoices");

                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            string fileName = $"signed_invoice_{result.InvoiceNumber}_{DateTime.Now:yyyyMMddHHmmss}.xml";
                            string filePath = Path.Combine(folderPath, fileName);

                            File.WriteAllText(filePath, signedXml, Encoding.UTF8);
                        }

                        // ⭐ الطباعة بعد نجاح الإرسال
                        PrintInvoice(invoiceId);
                    }
                    else
                    {
                        MessageBox.Show(
                            $"فشل الإرسال: {result.Message}",
                            "نتيجة الإرسال"
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        private void PrintInvoice(int invoiceId)
        {
            

            try
            {
                DataTable dt = new DataTable();

               
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("rpt_PrintInvoice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                  
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للطباعة");
                    return;
                }

                string qrPath = GetQRPathFromDatabase(invoiceId);

                rptInvoice rpt = new rptInvoice();
                rpt.SetDataSource(dt);

                rpt.SetParameterValue("QRCodeImagePath", qrPath);

                decimal total = 0;

                if (dt.Rows[0]["InvoiceTotal"] != DBNull.Value)
                    total = Convert.ToDecimal(dt.Rows[0]["InvoiceTotal"]);

                total = Math.Round(total, 3);

                string totalInWords = NumberConverter.ConvertToArabicWords(total);
                totalInWords = "فقط: " + totalInWords + " لا غير";

              

                rpt.SetParameterValue("pTotalInWords", totalInWords);

                frm_ReportViewer frm = new frm_ReportViewer(rpt);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static class NumberConverter
        {
            private static readonly string[] units =
            {
        "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة",
        "ستة", "سبعة", "ثمانية", "تسعة", "عشرة",
        "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر",
        "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
    };

            private static readonly string[] tens =
            {
        "", "", "عشرون", "ثلاثون", "أربعون",
        "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
    };

            private static readonly string[] hundreds =
            {
        "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة",
        "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
    };

            public static string ConvertToArabicWords(decimal number)
            {
                if (number == 0)
                    return "صفر دينار";

                long dinar = (long)Math.Floor(number);

                // 🔥 تعديل مهم: 1000 فلس (الأردن)
                int fils = (int)Math.Round((number - dinar) * 1000);

                string result = ConvertNumber(dinar) + " " + GetDinarWord(dinar);

                if (fils > 0)
                {
                    // 🔥 هنا الحل الحقيقي
                    result += " و " + ConvertNumberUnder1000(fils) + " " + GetFilsWord(fils);
                }

                return result;
            }

            // 🔥 دالة خاصة للفلس فقط (أقل من 1000)
            private static string ConvertNumberUnder1000(int number)
            {
                if (number == 0)
                    return "";

                if (number < 20)
                    return units[number];

                if (number < 100)
                {
                    if (number % 10 == 0)
                        return tens[number / 10];

                    return units[number % 10] + " و " + tens[number / 10];
                }

                if (number < 1000)
                {
                    if (number % 100 == 0)
                        return hundreds[number / 100];

                    return hundreds[number / 100] + " و " + ConvertNumberUnder1000(number % 100);
                }

                return number.ToString(); // احتياط
            }

            private static string GetDinarWord(long number)
            {
                if (number == 1) return "دينار";
                if (number == 2) return "ديناران";
                if (number >= 3 && number <= 10) return "دنانير";
                return "دينار";
            }

            private static string GetFilsWord(int number)
            {
                if (number == 1) return "فلس";
                if (number == 2) return "فلسان";
                if (number >= 3 && number <= 10) return "فلوس";
                return "فلسا";
            }

            private static string ConvertNumber(long number)
            {
                if (number == 0)
                    return "";

                if (number < 20)
                    return units[number];

                if (number < 100)
                {
                    if (number % 10 == 0)
                        return tens[number / 10];

                    return units[number % 10] + " و " + tens[number / 10];
                }

                if (number < 1000)
                {
                    if (number % 100 == 0)
                        return hundreds[number / 100];

                    return hundreds[number / 100] + " و " + ConvertNumber(number % 100);
                }

                if (number < 1000000)
                {
                    long thousands = number / 1000;
                    long remainder = number % 1000;

                    string thousandText = "";

                    if (thousands == 1)
                        thousandText = "ألف";
                    else if (thousands == 2)
                        thousandText = "ألفان";
                    else
                        thousandText = ConvertNumber(thousands) + " آلاف";

                    if (remainder == 0)
                        return thousandText;

                    return thousandText + " و " + ConvertNumber(remainder);
                }

                if (number < 1000000000)
                {
                    long millions = number / 1000000;
                    long remainder = number % 1000000;

                    string millionText = "";

                    if (millions == 1)
                        millionText = "مليون";
                    else if (millions == 2)
                        millionText = "مليونان";
                    else
                        millionText = ConvertNumber(millions) + " ملايين";

                    if (remainder == 0)
                        return millionText;

                    return millionText + " و " + ConvertNumber(remainder);
                }

                return number.ToString();
            }
        }
        private void btnShowQR_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.CurrentRow == null)
            {
                MessageBox.Show("اختر فاتورة أولاً");
                return;
            }

            int invoiceId = Convert.ToInt32(dgvInvoices.CurrentRow.Cells["InvoiceId"].Value);
            string qrPath = GetQRPathFromDatabase(invoiceId);

            if (!string.IsNullOrEmpty(qrPath) && File.Exists(qrPath))
            {
                Form qrForm = new Form();
                qrForm.Text = $"QR Code - فاتورة رقم {invoiceId}";
                qrForm.Size = new Size(300, 300);
                qrForm.StartPosition = FormStartPosition.CenterParent;

                PictureBox pb = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = Image.FromFile(qrPath),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                qrForm.Controls.Add(pb);
                qrForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("لا يوجد رمز QR لهذه الفاتورة أو الملف غير موجود");
            }
        }
        private string GetQRPathFromDatabase(int invoiceId)
        {
            string qrPath = null;

            using (SqlConnection con = new SqlConnection(
                @"Data Source=.\SQLEXPRESS;Initial Catalog=AccountingCoreDB;Integrated Security=True"))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT QRCodePath FROM Invoices WHERE InvoiceId=@Id",
                    con);

                cmd.Parameters.AddWithValue("@Id", invoiceId);

                qrPath = cmd.ExecuteScalar()?.ToString();
            }

            return qrPath;
        }

       

        private void btnCreditNote_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.CurrentRow == null)
            {
                MessageBox.Show("اختر فاتورة");
                return;
            }

            int invoiceId = Convert.ToInt32(dgvInvoices.CurrentRow.Cells["InvoiceId"].Value);
            bool posted = Convert.ToBoolean(dgvInvoices.CurrentRow.Cells["PostedToTax"].Value);
            if (!posted)
            {
                MessageBox.Show("لا يمكن إنشاء مرتجع لفاتورة لم ترسل بعد.");
                return;
            }

            frm_CreditNote frm = new frm_CreditNote(invoiceId);
            frm.ShowDialog();
            // يمكن تحديث القائمة إذا أضفنا عموداً للمرتجعات
        }

      

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.CurrentRow == null)
            {
                MessageBox.Show("الرجاء اختيار فاتورة أولاً");
                return;
            }

            int invoiceId = Convert.ToInt32(
                dgvInvoices.CurrentRow.Cells["InvoiceId"].Value
            );

            PrintInvoice(invoiceId);
        }

        private void dgvInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int invoiceId = Convert.ToInt32(
                dgvInvoices.Rows[e.RowIndex].Cells["InvoiceId"].Value
            );

            //frm_NewInvoice frm = new frm_NewInvoice(invoiceId);
            frm_InvoiceEditor frm = new frm_InvoiceEditor(invoiceId);
            frm.ShowDialog();

            LoadInvoices();
        }
        private void SetupButtonsAppearance()
        {
            // تخصيص زر جديد
            btnNew.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnNew.Appearance.ForeColor = Color.White;
            btnNew.Appearance.BackColor = Color.FromArgb(41, 128, 185); // أزرق غامق
            btnNew.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnNew.Cursor = Cursors.Hand;

            // زر تعديل
            btnEdit.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnEdit.Appearance.ForeColor = Color.White;
            btnEdit.Appearance.BackColor = Color.FromArgb(39, 174, 96); // أخضر
            btnEdit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnEdit.Cursor = Cursors.Hand;


            btnSend.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnSend.Appearance.ForeColor = Color.White;
            btnSend.Appearance.BackColor = Color.FromArgb(192, 57, 43); // أحمر
            btnSend.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnSend.Cursor = Cursors.Hand;

            // تخصيص زر جديد
            btnRefresh.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnRefresh.Appearance.ForeColor = Color.White;
            btnRefresh.Appearance.BackColor = Color.FromArgb(41, 128, 185); // أزرق غامق
            btnRefresh.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnRefresh.Cursor = Cursors.Hand;

            // زر تعديل
            btnCreditNote.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnCreditNote.Appearance.ForeColor = Color.White;
            btnCreditNote.Appearance.BackColor = Color.FromArgb(39, 174, 96); // أخضر
            btnCreditNote.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnCreditNote.Cursor = Cursors.Hand;


            btnShowQR.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnShowQR.Appearance.ForeColor = Color.White;
            btnShowQR.Appearance.BackColor = Color.FromArgb(192, 57, 43); // أحمر
            btnShowQR.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnShowQR.Cursor = Cursors.Hand;

            btnPrint.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnPrint.Appearance.ForeColor = Color.White;
            btnPrint.Appearance.BackColor = Color.SkyBlue; // أحمر
            btnPrint.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnPrint.Cursor = Cursors.Hand;
        }

    }
}
  

