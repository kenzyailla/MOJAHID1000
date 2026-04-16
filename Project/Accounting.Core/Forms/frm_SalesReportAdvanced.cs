using Accounting.Core.Models;
using Accounting.Core.Services;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_SalesReportAdvanced : Form
    {


        private string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private SalesReportService service;
       

        public frm_SalesReportAdvanced()
        {
            InitializeComponent();
        
            service = new SalesReportService(connectionString);
            
        }
        public class ComboItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        CustomerService cs;
        bool isSelecting = false;

       
        ProductService ps;
        DevExpress.XtraGrid.Views.Grid.GridView gridView1;

        private void frm_SalesReportAdvanced_Load(object sender, EventArgs e)
        {
         
                ps = new ProductService(connectionString);
                cs = new CustomerService(connectionString);

                var products = ps.GetAllProducts();

                var productList = new List<Product>();
                productList.Add(new Product { ProductId = 0, Name = "الكل" });
                productList.AddRange(products);

                cbxProduct.DataSource = productList;
                cbxProduct.DisplayMember = "Name";
                cbxProduct.ValueMember = "ProductId";
                cbxProduct.SelectedIndex = 0;

                cbxCustomer.DataSource = cs.GetAllCustomers();
                cbxCustomer.DisplayMember = "Name";
                cbxCustomer.ValueMember = "CustomerId";

                dateFrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
                dateTo.DateTime = DateTime.Today;

                comboBox1.Items.Clear();
                comboBox1.Items.Add("الكل");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT DISTINCT TaxRate FROM InvoiceLines ORDER BY TaxRate", con);

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        comboBox1.Items.Add(Convert.ToDecimal(dr["TaxRate"]));
                    }
                }

                comboBox1.SelectedIndex = 0;

                SetupGridColumns(pivotGridControl1);
                CustomizeGridView(pivotGridControl1);
                SetupButtonsAppearance();

                 //🔥 SearchLookUpEdit
                searchLookUpEdit1.Properties.DataSource = products;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "ProductId";

            searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            searchLookUpEdit1.Properties.NullText = "";

            //searchLookUpEdit1.Properties.View.PopulateColumns();

            // 🔥 الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم المنتج");

            // 🔥 الخط
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10);

            // 🔥 تحسين
            searchLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit1.Properties.View.RowHeight = 28;
            var view = gridControl1.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {
                view.RowCellClick += view_RowCellClick;
            }
         
            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;
            }
        private void view_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            // 🔥 فقط دبل كليك
            if (e.Clicks != 2)
                return;

            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view == null)
                return;

            if (e.RowHandle < 0)
                return;

            object idObj = view.GetRowCellValue(e.RowHandle, "InvoiceId");

            if (idObj == null || idObj == DBNull.Value)
                return;

            int invoiceId = Convert.ToInt32(idObj);

            frm_NewInvoice frm = new frm_NewInvoice(invoiceId);
            frm.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                int? productId = null;
                int? customerId = null;

                if (Convert.ToInt32(cbxProduct.SelectedValue) != 0)
                    productId = Convert.ToInt32(cbxProduct.SelectedValue);

                if (Convert.ToInt32(cbxCustomer.SelectedValue) != 0)
                    customerId = Convert.ToInt32(cbxCustomer.SelectedValue);

                string invoiceNumber = txtInvoiceNumber.Text.Trim();

                decimal? taxRate = null;
                if (comboBox1.SelectedIndex > 0)
                    taxRate = Convert.ToDecimal(comboBox1.SelectedItem);

                DataTable dt = service.GetSalesReportAdvanced(
                    productId,
                    customerId,
                    invoiceNumber,
                    taxRate,
                    dateFrom.DateTime,
                    dateTo.DateTime
                );

                // ⭐ الربط الصحيح (GridControl فقط)
                gridControl1.DataSource = dt;
              
                CalculateReportTotals(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message);
            }

           
        }
    private void PrepareGrid()
        {
            pivotGridControl1.PopulateColumns();

            pivotGridControl1.OptionsBehavior.Editable = false;
            pivotGridControl1.OptionsView.ShowFooter = true;

            pivotGridControl1.Columns["Quantity"].SummaryItem.SummaryType =
                DevExpress.Data.SummaryItemType.Sum;
            pivotGridControl1.Columns["Quantity"].SummaryItem.DisplayFormat = "{0:N2}";


            pivotGridControl1.Columns["TotalAfterTax"].SummaryItem.SummaryType =
                DevExpress.Data.SummaryItemType.Sum;
            // ⭐ عرض دينار أردني
            pivotGridControl1.Columns["TotalAfterTax"].SummaryItem.DisplayFormat = "د.أ {0:N2}";

        }
        private void CalculateReportTotals(DataTable dt)
        {
            decimal subTotal = 0;
            decimal taxTotal = 0;
            decimal grandTotal = 0;

            foreach (DataRow row in dt.Rows)
            {
                subTotal += row["TotalBeforeTax"] == DBNull.Value
                    ? 0 : Convert.ToDecimal(row["TotalBeforeTax"]);

                taxTotal += row["TotalTax"] == DBNull.Value
                    ? 0 : Convert.ToDecimal(row["TotalTax"]);

                grandTotal += row["TotalAfterTax"] == DBNull.Value
                    ? 0 : Convert.ToDecimal(row["TotalAfterTax"]);
            }

            txtSubTotal.Text = subTotal.ToString("N2");
            txtTaxTotal.Text = taxTotal.ToString("N2");
            txtTotal.Text = grandTotal.ToString("N2");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             btnSearch_Click(null, null);
        }

        private void pivotGridControl1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("DoubleClick works");
            if (pivotGridControl1.FocusedRowHandle < 0)
                return;

            object idObj = pivotGridControl1.GetFocusedRowCellValue("InvoiceId");

            if (idObj == null || idObj == DBNull.Value)
                return;

            int invoiceId = Convert.ToInt32(idObj);

            frm_NewInvoice frm = new frm_NewInvoice(invoiceId);
            frm.ShowDialog();

        }

        private void pivotGridControl1_Click(object sender, EventArgs e)
        {
            if (pivotGridControl1.FocusedRowHandle < 0)
                return;

            int invoiceId = Convert.ToInt32(
                pivotGridControl1.GetFocusedRowCellValue("InvoiceId")
            );

            frm_InvoiceEditor frm = new frm_InvoiceEditor(invoiceId);
            frm.ShowDialog();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

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

            btnSearch.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnSearch.Appearance.ForeColor = Color.White;
            btnSearch.Appearance.BackColor = Color.FromArgb(41, 128, 185); // أزرق غامق
            btnSearch.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnSearch.Cursor = Cursors.Hand;


          


        }

        private void cbxCustomer_TextChanged(object sender, EventArgs e)
        {
            if (isSelecting) return;

            if (cs == null)
                cs = new CustomerService(connectionString);

            string text = cbxCustomer.Text;

            if (text.Length < 2)
                return;

            isSelecting = true;

            var dt = cs.SearchCustomers(text);
            cbxCustomer.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                cbxCustomer.DataSource = dt;
                cbxCustomer.SelectedIndex = -1;
                cbxCustomer.DisplayMember = "Name";
                cbxCustomer.ValueMember = "CustomerId";

                cbxCustomer.DroppedDown = true;
                cbxCustomer.SelectionStart = cbxCustomer.Text.Length;
            }

            isSelecting = false;
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int productId))
                return;

            // 🔥 اجعل الكومبو العادي يتبع الاختيار
            cbxProduct.SelectedValue = productId;

            // 🔥 نفذ البحث مباشرة
            btnSearch.PerformClick();
        }
     
    }
}
