using Accounting.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_SalesReturn : Form
    {
        private readonly string _connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private int? _returnId = null;

        //-----------------------------------
        // إنشاء مرتجع جديد
        //-----------------------------------

        public frm_SalesReturn()
        {
            InitializeComponent();
        }

        //-----------------------------------
        // فتح مرتجع موجود
        //-----------------------------------

        public frm_SalesReturn(int returnId)
        {
            InitializeComponent();

            _returnId = returnId;

            LoadReturn(returnId);
        }
  


    private void LoadReturn(int returnId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT 
    l.*,
    p.Name AS ProductName
FROM SalesReturnLines l
LEFT JOIN Products p
    ON l.ProductId = p.ProductId
WHERE l.SalesReturnId=@Id", con);

                da.SelectCommand.Parameters.AddWithValue("@Id", returnId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gridLines.DataSource = dt;
                gridView1.Columns["ProductId"].Visible = false;
                gridView1.Columns["ProductName"].Caption = "اسم الصنف";

            }
        }
        private void frm_SalesReturn_Load(object sender, EventArgs e)
        {
            InvoiceService service = new InvoiceService(_connectionString);

            DataTable dt = service.GetInvoices();

            cbxInvoicecomboBox1.DataSource = dt;
            cbxInvoicecomboBox1.DisplayMember = "InvoiceNumber";
            cbxInvoicecomboBox1.ValueMember = "InvoiceId";

            // تعيين حجم النموذج
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Width = (int)(screenWidth * 0.7);
            this.Height = (int)(screenHeight * 0.75);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);



            // ثم تطبيق التنسيقات الأخرى (الألوان، الخطوط) إذا أردت
            CustomizeGridView(gridView1);

        }

        private void cbxInvoicecomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int invoiceId = Convert.ToInt32(cbxInvoicecomboBox1.SelectedValue);

            DataTable dt = gridLines.DataSource as DataTable;

            Dictionary<int, decimal> returnQty = new Dictionary<int, decimal>();

            foreach (DataRow row in dt.Rows)
            {
                decimal qty = row["ReturnQty"] == DBNull.Value ? 0 :
                              Convert.ToDecimal(row["ReturnQty"]);

                if (qty > 0)
                {
                    int productId = Convert.ToInt32(row["ProductId"]);
                    returnQty.Add(productId, qty);
                }
            }

            if (returnQty.Count == 0)
            {
                MessageBox.Show("لم يتم إدخال أي كمية مرتجعة");
                return;
            }

            InvoiceService service = new InvoiceService(_connectionString);

            service.CreateSalesReturn(invoiceId, returnQty, "مرتجع من الشاشة");

            MessageBox.Show("تم حفظ المرتجع بنجاح");

            this.Close();
            AppEvents.RefreshDashboard();
        }

        private void cbxInvoicecomboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbxInvoicecomboBox1.SelectedValue == null)
                return;

            int invoiceId = Convert.ToInt32(cbxInvoicecomboBox1.SelectedValue);

            InvoiceService service = new InvoiceService(_connectionString);
            DataTable dt = service.GetInvoiceLines(invoiceId);

            if (!dt.Columns.Contains("ReturnQty"))
                dt.Columns.Add("ReturnQty", typeof(decimal));

            foreach (DataRow row in dt.Rows)
                row["ReturnQty"] = 0m;

            gridLines.DataSource = null;
            gridLines.DataSource = dt;

            gridView1.Columns.Clear();
            gridView1.PopulateColumns();

            // إخفاء رقم المنتج
            if (gridView1.Columns.ColumnByFieldName("ProductId") != null)
                gridView1.Columns["ProductId"].Visible = false;

            // عرض اسم المنتج
            if (gridView1.Columns.ColumnByFieldName("ProductName") != null)
            {
                gridView1.Columns["ProductName"].Caption = "اسم الصنف";
                gridView1.Columns["ProductName"].VisibleIndex = 0;
            }


            if (gridView1.Columns.ColumnByFieldName("Quantity") != null)
                gridView1.Columns["Quantity"].Caption = "الكمية";

            if (gridView1.Columns.ColumnByFieldName("UnitPrice") != null)
                gridView1.Columns["UnitPrice"].Caption = "سعر الوحدة";

            if (gridView1.Columns.ColumnByFieldName("Discount") != null)
                gridView1.Columns["Discount"].Caption = "الخصم";

            if (gridView1.Columns.ColumnByFieldName("TaxRate") != null)
                gridView1.Columns["TaxRate"].Caption = "نسبة الضريبة";

            if (gridView1.Columns.ColumnByFieldName("TotalBeforeTax") != null)
                gridView1.Columns["TotalBeforeTax"].Caption = "قبل الضريبة";

            if (gridView1.Columns.ColumnByFieldName("TotalTax") != null)
                gridView1.Columns["TotalTax"].Caption = "الضريبة";

            if (gridView1.Columns.ColumnByFieldName("TotalAfterTax") != null)
                gridView1.Columns["TotalAfterTax"].Caption = "الإجمالي";

            if (gridView1.Columns.ColumnByFieldName("ReturnQty") != null)
                gridView1.Columns["ReturnQty"].Caption = "الكمية المرتجعة";

            gridView1.BestFitColumns();
            gridView1.RefreshData();
        }
        private void CustomizeGridView(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            gv.Appearance.Row.BackColor = Color.White;
            gv.Appearance.Row.ForeColor = Color.Black;

            gv.Appearance.EvenRow.BackColor = ColorTranslator.FromHtml("#f0f8ff");
            gv.OptionsView.EnableAppearanceEvenRow = true;

            gv.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("#ADD8E6");
            gv.Appearance.FocusedRow.ForeColor = Color.Black;

            gv.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 8, FontStyle.Bold);
            gv.Appearance.Row.Font = new Font("Noto Kufi Arabic", 8);

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
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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
    }
}
