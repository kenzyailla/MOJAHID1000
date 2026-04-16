using System;
using System.Data;
using System.Windows.Forms;
using Accounting.Core.Services;
using Accounting.Core.Models;
using System.Drawing;
using System.Collections.Generic;

namespace Accounting.Core.Forms
{
    public partial class frm_BuyReportAdvanced : Form
    {
     

        string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private SupplierService supplierService;
        private BuyReportService service;

        public frm_BuyReportAdvanced()
        {
            InitializeComponent();
            service = new BuyReportService(connectionString);
        }
        SupplierService cs;
        ProductService ps;
        private void frm_BuyReportAdvanced_Load(object sender, EventArgs e)
        {
            LoadFilters();

            dateFrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
            dateTo.DateTime = DateTime.Today;

            CustomizeGridView(gridView1);
            SetupButtonsAppearance();

            ps = new ProductService(connectionString);
            cs = new SupplierService(connectionString);

            var products = ps.GetAllProducts();

            searchLookUpEdit1.Properties.DataSource = products;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "ProductId";

            searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            searchLookUpEdit1.Properties.View.PopulateColumns();


            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;

            var suppliers = cs.GetAllSuppliers();


            searchLookUpEdit2.Properties.DataSource = suppliers;
            searchLookUpEdit2.Properties.DisplayMember = "Name";
            searchLookUpEdit2.Properties.ValueMember = "SupplierId";

            searchLookUpEdit2.Properties.NullText = "";

            // 🔥 تنظيف الأعمدة
            searchLookUpEdit2.Properties.View.Columns.Clear();
            searchLookUpEdit2.Properties.View.Columns.AddVisible("Name", "اسم المورد");

            // 🔥 تحسين العرض
            searchLookUpEdit2.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit2.Properties.View.BestFitColumns();

            // 🔥 الخط
            searchLookUpEdit2.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Regular);

            searchLookUpEdit2.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit2.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            searchLookUpEdit1.Properties.DataSource = products;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "ProductId";

            searchLookUpEdit1.Properties.NullText = "";

            // 🔥 الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم المنتج");


            // 🔥 الشكل
            searchLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit1.Properties.View.BestFitColumns();

            // 🔥 الخط
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10);

            // 🔥 البحث
            searchLookUpEdit1.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            searchLookUpEdit1.Properties.ImmediatePopup = true;

            // 🔥 حجم الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;

            // 🔥 البحث
            searchLookUpEdit2.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            searchLookUpEdit2.Properties.ImmediatePopup = true;

            // 🔥 حجم الصف
            searchLookUpEdit2.Properties.View.RowHeight = 28;

            searchLookUpEdit2.EditValueChanged += searchLookUpEdit2_EditValueChanged;



        }
        private void LoadFilters()
        {
            ProductService ps = new ProductService(connectionString);
            SupplierService ss = new SupplierService(connectionString);

            // ================= المنتجات (List<Product>) =================
            var products = ps.GetAllProducts();   // ✅ بدون ToList

            products.Insert(0, new Product
            {
                ProductId = 0,
                Name = "الكل"
            });

            cbxProduct.DataSource = products;
            cbxProduct.DisplayMember = "Name";
            cbxProduct.ValueMember = "ProductId";
            cbxProduct.SelectedIndex = 0;


            // ================= الموردين (DataTable) =================
            DataTable dtSuppliers = ss.GetAllSuppliers();   // ✅ DataTable

            DataRow r = dtSuppliers.NewRow();
            r["SupplierId"] = DBNull.Value;
            r["Name"] = "الكل";
            dtSuppliers.Rows.InsertAt(r, 0);

            cbxSupplier.DataSource = dtSuppliers;
            cbxSupplier.DisplayMember = "Name";
            cbxSupplier.ValueMember = "SupplierId";
            cbxSupplier.SelectedIndex = 0;


            // ================= التاريخ =================
            dateFrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
            dateTo.DateTime = DateTime.Today;


            // ================= الضريبة =================
            comboBoxTax.Items.Clear();   // أو غيّر الاسم حسب الكمبوبوكس عندك
            comboBoxTax.Items.Add("الكل");
            comboBoxTax.Items.Add(0m);
            comboBoxTax.Items.Add(1m);
            comboBoxTax.Items.Add(4m);
            comboBoxTax.Items.Add(16m);
            comboBoxTax.SelectedIndex = 0;
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            int? productId = null;
            int? supplierId = null;
            decimal? taxRate = null;
            if (cbxProduct.SelectedIndex > 0)
                productId = Convert.ToInt32(cbxProduct.SelectedValue);

            if (cbxSupplier.SelectedIndex > 0)
                supplierId = Convert.ToInt32(cbxSupplier.SelectedValue);

            if (comboBoxTax.SelectedIndex > 0)
                taxRate = Convert.ToDecimal(comboBoxTax.SelectedItem);

            if (comboBoxTax.SelectedIndex > 0)
                taxRate = Convert.ToDecimal(comboBoxTax.SelectedItem);

            string invoiceNumber = txtInvoiceNumber.Text.Trim();

            DataTable dt = service.GetBuyReportAdvanced(
                productId,
                supplierId,
                invoiceNumber,
                taxRate,
                dateFrom.DateTime,
                dateTo.DateTime);

            gridControl1.DataSource = dt;

            gridView1.PopulateColumns();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowFooter = true;

            PrepareGridSummaries();
            CalculateTotals(dt);
           
        }
        private void PrepareGridSummaries()
        {
            if (gridView1.Columns["Quantity"] != null)
                gridView1.Columns["Quantity"].SummaryItem.SummaryType =
                    DevExpress.Data.SummaryItemType.Sum;
            gridView1.Columns["Quantity"].SummaryItem.DisplayFormat = "{0:N2}";
            if (gridView1.Columns["LineTotal"] != null)
            {
                gridView1.Columns["LineTotal"].SummaryItem.SummaryType =
                    DevExpress.Data.SummaryItemType.Sum;

                gridView1.Columns["LineTotal"].SummaryItem.DisplayFormat =
                    "د.أ {0:N2}";
            }
        }

        private void CalculateTotals(DataTable dt)
        {
            decimal sub = 0;
            decimal tax = 0;
            decimal total = 0;

            foreach (DataRow r in dt.Rows)
            {
                sub += r["LineSubTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["LineSubTotal"]);
                tax += r["TaxAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TaxAmount"]);
                total += r["LineTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["LineTotal"]);
            }

            txtSubTotal.Text = sub.ToString("N2");
            txtTaxTotal.Text = tax.ToString("N2");
            txtGrandTotal.Text = total.ToString("N2");
        }

        private void comboBoxTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void CustomizeGridView(DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            // General appearance
            gridView.Appearance.Empty.BackColor = Color.White;
            gridView.Appearance.Row.BackColor = Color.White;
            gridView.Appearance.Row.ForeColor = Color.Black;
            gridView.Appearance.FocusedRow.BackColor = Color.FromArgb(173, 216, 230); // Light blue
            gridView.Appearance.FocusedRow.ForeColor = Color.Black;
            gridView.Appearance.SelectedRow.BackColor = Color.FromArgb(173, 216, 230);
            gridView.Appearance.SelectedRow.ForeColor = Color.Black;

            // Header appearance
            gridView.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 8.18868f, FontStyle.Bold);
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView.Appearance.HeaderPanel.BackColor = Color.LightGray;

            // Cell appearance
            gridView.Appearance.Row.Font = new Font("Noto Kufi Arabic", 8.18868f);
            gridView.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center; // Right alignment for Arabic

            // Alternating row colors (if needed)
            gridView.OptionsView.EnableAppearanceEvenRow = true;
            gridView.OptionsView.EnableAppearanceOddRow = true;
            gridView.Appearance.EvenRow.BackColor = Color.FromArgb(240, 248, 255); // AliceBlue
            gridView.Appearance.OddRow.BackColor = Color.White;

            // Grid lines
            gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView.GridControl.LookAndFeel.UseDefaultLookAndFeel = false; // Allow custom styling

            // Row height
            gridView.RowHeight = 30;

            // Disable row headers and group panel (already set elsewhere)
            // gridView.OptionsView.ShowGroupPanel = false;
            // gridView.OptionsView.RowHeadersWidth = 0; // Optional
        }
        private void SetupButtonsAppearance()
        {
            // تخصيص زر جديد
            btnSearch.Appearance.Font = new Font("Noto Kufi Arabic", 8, FontStyle.Bold);
            btnSearch.Appearance.ForeColor = Color.White;
            btnSearch.Appearance.BackColor = Color.FromArgb(41, 128, 185); // أزرق غامق
            btnSearch.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnSearch.Cursor = Cursors.Hand;


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

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit2.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit2.EditValue.ToString(), out int supplierId))
                return;

            // 🔥 ربط المورد مع الكمبو
            cbxSupplier.SelectedValue = supplierId;

            // 🔥 أو شغل البحث مباشرة
            btnSearch.PerformClick();
        }

        private void searchLookUpEdit1_TextChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

