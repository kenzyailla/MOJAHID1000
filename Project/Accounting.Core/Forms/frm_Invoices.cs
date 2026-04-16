using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Accounting.Core.Services;
using Accounting.Core.Forms;

namespace Accounting.Core.Forms
{
   
    public partial class frm_Invoices : Form
    {
        private int? _invoiceId;
        public frm_Invoices()
        {
            InitializeComponent();
         
            service = new InvoiceService(
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True");
        }
        string connectionString =
  @"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";
        public frm_Invoices(int invoiceId) : this()
        {
            _invoiceId = invoiceId;
        }
        bool isOpening = false;
      
        private void btnAdd_Click(object sender, EventArgs e)
        {
            frm_NewInvoice frm = new frm_NewInvoice();
            frm.ShowDialog();

            LoadInvoices();
        }
        InvoiceService service;

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRowCellValue("InvoiceId") == null)
                return;

            int invoiceId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("InvoiceId")
            );

            frm_InvoiceEditor frm = new frm_InvoiceEditor(invoiceId);
            frm.ShowDialog();

            LoadInvoices();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRowCellValue("InvoiceId") == null)
                return;

            int invoiceId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("InvoiceId"));

            if (MessageBox.Show("هل تريد حذف الفاتورة؟",
                "تأكيد",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                service.DeleteInvoice(invoiceId);

                LoadInvoices();
            }
        }
        private void LoadInvoices()
        {
            gridControl1.DataSource = service.GetInvoices();
            gridView1.PopulateColumns();

            if (gridView1.Columns["InvoiceId"] != null)
                gridView1.Columns["InvoiceId"].Visible = false;

            if (gridView1.Columns["UUID"] != null)
                gridView1.Columns["UUID"].Visible = false;

            if (gridView1.Columns["QRCodePath"] != null)
                gridView1.Columns["QRCodePath"].Visible = false;

            if (gridView1.Columns["InvoiceNumber"] != null)
                gridView1.Columns["InvoiceNumber"].Caption = "رقم الفاتورة";

            if (gridView1.Columns["InvoiceDate"] != null)
                gridView1.Columns["InvoiceDate"].Caption = "التاريخ";

            if (gridView1.Columns["CustomerName"] != null)
                gridView1.Columns["CustomerName"].Caption = "العميل";

            if (gridView1.Columns["TotalBeforeTax"] != null)
                gridView1.Columns["TotalBeforeTax"].Caption = "قبل الضريبة";

            if (gridView1.Columns["TotalTax"] != null)
                gridView1.Columns["TotalTax"].Caption = "الضريبة";

            if (gridView1.Columns["TotalAfterTax"] != null)
                gridView1.Columns["TotalAfterTax"].Caption = "الإجمالي";

            if (gridView1.Columns["PostedToTax"] != null)
                gridView1.Columns["PostedToTax"].Caption = "مرسلة للضريبة";

            if (gridView1.Columns["PostedDate"] != null)
                gridView1.Columns["PostedDate"].Caption = "تاريخ الإرسال";
        }


        private void frm_Invoices_Load(object sender, EventArgs e)
        {
            LoadInvoices();
           
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;

            // تعيين حجم النموذج
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Width = (int)(screenWidth * 0.7);
            this.Height = (int)(screenHeight * 0.75);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);

            // استدعاء الدالة الجديدة لضبط الأعمدة
            SetupGridColumns(gridView1);

            // ثم تطبيق التنسيقات الأخرى (الألوان، الخطوط) إذا أردت
            CustomizeGridView(gridView1);
            SetupButtonsAppearance();
          
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (isOpening)
                return;

            isOpening = true;

            if (gridView1.FocusedRowHandle < 0)
                return;

            int invoiceId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("InvoiceId"));

            frm_InvoiceEditor frm = new frm_InvoiceEditor(invoiceId);
            frm.ShowDialog();

            LoadInvoices();

            isOpening = false;
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            
        }

        private void gridControl1_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void gridControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {
            //if (e.Clicks != 2)
            //    return;

            //var hitInfo = gridView1.CalcHitInfo(e.Location);

            //if (!hitInfo.InRow && !hitInfo.InRowCell)
            //    return;

            //int invoiceId = Convert.ToInt32(
            //    gridView1.GetRowCellValue(hitInfo.RowHandle, "InvoiceId"));

            //frm_InvoiceEditor frm = new frm_InvoiceEditor(invoiceId);
            //frm.ShowDialog();

            //LoadInvoices();
        }
        private void CustomizeGridView(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            gv.Appearance.Row.BackColor = Color.White;
            gv.Appearance.Row.ForeColor = Color.Black;

            gv.Appearance.EvenRow.BackColor = ColorTranslator.FromHtml("#f0f8ff");
            gv.OptionsView.EnableAppearanceEvenRow = true;

            gv.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("#ADD8E6");
            gv.Appearance.FocusedRow.ForeColor = Color.Black;

            gv.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            gv.Appearance.Row.Font = new Font("Noto Kufi Arabic", 10);

            // محاذاة الرؤوس إلى الوسط لضمان ظهور النص كاملاً
            gv.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            // محاذاة الخلايا إلى اليمين (كما كانت)
            gv.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gv.OptionsView.ShowGroupPanel = false;
            gv.OptionsView.ShowIndicator = false;
            gv.RowHeight = 30;

            // ضبط عرض الأعمدة
            gv.BestFitColumns(); // ضبط أولي حسب المحتوى

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gv.Columns)
            {
                if (col.FieldName == "CustomerName" || col.FieldName == "ProductName")
                {
                    col.Width = Math.Min(col.Width, 200); // حد أقصى 200 بكسل للأسماء
                }
                else if (col.FieldName == "InvoiceNumber" || col.FieldName == "UUID" || col.FieldName == "ICV")
                {
                    col.Width = Math.Min(col.Width, 120); // حد أقصى 120 للأرقام المرجعية
                }
                else // للبيانات الرقمية (المبالغ، التواريخ، إلخ)
                {
                    col.Width = Math.Min(col.Width, 80);
                }
            }

            gv.OptionsView.ColumnAutoWidth = true; // توزيع المساحات الفارغة تلقائياً
        }
        private void SetupGridColumns(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            // إذا لم تكن الأعمدة قد أنشئت بعد، نقوم بإنشائها من مصدر البيانات
            if (gv.Columns.Count == 0)
                gv.PopulateColumns();

            // تعيين عنوان و عرض كل عمود بناءً على اسم الحقل
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gv.Columns)
            {
                switch (col.FieldName)
                {
                    case "InvoiceId":
                        col.Caption = "رقم الفاتورة";
                        col.Width = 80;
                        break;
                    case "InvoiceNumber":
                        col.Caption = "رقم الفاتورة";
                        col.Width = 100;
                        break;
                    case "InvoiceDate":
                        col.Caption = "التاريخ";
                        col.DisplayFormat.FormatString = "yyyy-MM-dd";
                        col.Width = 90;
                        break;
                    case "CustomerName":
                        col.Caption = "العميل";
                        col.Width = 150;
                        break;
                    case "TotalBeforeTax":
                        col.Caption = "قبل الضريبة";
                        col.DisplayFormat.FormatString = "n3";
                        col.Width = 90;
                        break;
                    case "TotalTax":
                        col.Caption = "الضريبة";
                        col.DisplayFormat.FormatString = "n3";
                        col.Width = 80;
                        break;
                    case "TotalAfterTax":
                        col.Caption = "الإجمالي";
                        col.DisplayFormat.FormatString = "n3";
                        col.Width = 90;
                        break;
                    case "UUID":
                        col.Caption = "UUID";
                        col.Width = 200;
                        break;
                    case "ICV":
                        col.Caption = "ICV";
                        col.Width = 60;
                        break;
                    case "PostedToTax":
                        col.Caption = "مرسل؟";
                        col.Width = 60;
                        break;
                    default:
                        // إذا لم نعرف الحقل، نضع اسم الحقل كعنوان
                        col.Caption = col.FieldName;
                        col.Width = 100;
                        break;
                }
            }
            // محاذاة النص في الخلايا إلى الوسط
            gv.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            // محاذاة الرؤوس إلى الوسط
            gv.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            // ضبط عرض الأعمدة بشكل مثالي بعد تعيين العناوين
            gv.BestFitColumns();

            // السماح بتوزيع المساحات الفارغة
            gv.OptionsView.ColumnAutoWidth = true;
        }
        private void SetupButtonsAppearance()
        {
            // تخصيص زر جديد
            btnAdd.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnAdd.Appearance.ForeColor = Color.White;
            btnAdd.Appearance.BackColor = Color.FromArgb(41, 128, 185); // أزرق غامق
            btnAdd.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnAdd.Cursor = Cursors.Hand;

            // زر تعديل
            btnEdit.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnEdit.Appearance.ForeColor = Color.White;
            btnEdit.Appearance.BackColor = Color.FromArgb(39, 174, 96); // أخضر
            btnEdit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnEdit.Cursor = Cursors.Hand;

          
            btnDelete.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnDelete.Appearance.ForeColor = Color.White;
            btnDelete.Appearance.BackColor = Color.FromArgb(192, 57, 43); // أحمر
            btnDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnDelete.Cursor = Cursors.Hand;


        }
    }
}
