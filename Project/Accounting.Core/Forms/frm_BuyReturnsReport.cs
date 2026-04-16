using Accounting.Core.Models;
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
    public partial class frm_BuyReturnsReport : Form
    {
        private SupplierService supplierService;
        BuyReturnReportService service;

        string connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_BuyReturnsReport()
        {
            InitializeComponent();
            service = new BuyReturnReportService(connectionString);
        }
        private void frm_BuyReturnsReport_Load(object sender, EventArgs e)
        {
            dateFrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
            dateTo.DateTime = DateTime.Today;
            cbxSupplier.Size = new Size(204, 20);
            LoadSuppliers();
            CustomizeGridView(gridView1);


            SupplierService supplierService = new SupplierService(connectionString);

            var suppliers = supplierService.GetAllSuppliers();

            //searchLookUpEdit1.Properties.DataSource = suppliers;
            //searchLookUpEdit1.Properties.DisplayMember = "Name";
            //searchLookUpEdit1.Properties.ValueMember = "SupplierId";

            //searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            //searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            //searchLookUpEdit1.Properties.NullText = "";

            //// 🔥 مهم
            //searchLookUpEdit1.Properties.View.PopulateColumns();
            searchLookUpEdit1.Properties.DataSource = suppliers;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "SupplierId";

            searchLookUpEdit1.Properties.NullText = "";

            // 🔥 تنظيف الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم المورد");

            // 🔥 تحسين العرض
            searchLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit1.Properties.View.BestFitColumns();

            // 🔥 الخط
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Regular);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            // 🔥 البحث
            searchLookUpEdit1.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            searchLookUpEdit1.Properties.ImmediatePopup = true;

            // 🔥 حجم الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;

            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;




        }
        private void LoadSuppliers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                "SELECT SupplierId,Name FROM Suppliers", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxSupplier.DataSource = dt;
                cbxSupplier.DisplayMember = "Name";
                cbxSupplier.ValueMember = "SupplierId";

                cbxSupplier.SelectedIndex = -1;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

        private void CalculateTotals(DataTable dt)
        {
            decimal before = 0;
            decimal tax = 0;
            decimal after = 0;

            foreach (DataRow r in dt.Rows)
            {
                before += Convert.ToDecimal(r["LineBeforeTax"]);
                tax += Convert.ToDecimal(r["LineTax"]);
                after += Convert.ToDecimal(r["LineAfterTax"]);
            }

            lblBefore.Text = before.ToString("N2") + " د.أ";
            lblTax.Text = tax.ToString("N2") + " د.أ";
            lblAfter.Text = after.ToString("N2") + " د.أ";
        }

        private void cbxSupplier_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int? supplierId = null;

            if (cbxSupplier.SelectedValue != null)
                supplierId = Convert.ToInt32(cbxSupplier.SelectedValue);

            DataTable dt = service.GetReport(
                dateFrom.DateTime,
                dateTo.DateTime,
                supplierId);

            gridControl1.DataSource = dt;

            CalculateTotals(dt);
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            var gridView = gridControl1.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;

            int rowHandle = gridView.FocusedRowHandle;
            if (rowHandle < 0) return;

            // الحصول على الصف كـ DataRow
            DataRow row = gridView.GetDataRow(rowHandle);
            if (row == null) return;

            // استخراج معرف المرتجع (تأكد من اسم العمود الصحيح)
            int buyReturnId = Convert.ToInt32(row["BuyReturnId"]);
          
            // فتح النموذج مع تمرير المعرف
            frm_BuyReturn frm = new frm_BuyReturn(buyReturnId);
            frm.ShowDialog();

            // (اختياري) إعادة تحميل التقرير بعد إغلاق النموذج إذا حدث تعديل
            // btnSearch.PerformClick();
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

        }

        private void lblAfter_Click(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int supplierId))
                return;

            // 🔥 تحديد المورد داخل cbxSupplier بشكل مضمون
            for (int i = 0; i < cbxSupplier.Items.Count; i++)
            {
                var item = cbxSupplier.Items[i];

                if (item is DataRowView row)
                {
                    if (Convert.ToInt32(row["SupplierId"]) == supplierId)
                    {
                        cbxSupplier.SelectedIndex = i;
                        break;
                    }
                }
                else if (item is Supplier s)
                {
                    if (s.SupplierId == supplierId)
                    {
                        cbxSupplier.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
    }

