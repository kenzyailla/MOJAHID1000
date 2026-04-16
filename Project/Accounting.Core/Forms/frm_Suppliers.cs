using Accounting.Core.Services;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_Suppliers : Form
    {
        private SupplierService service;

        private string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_Suppliers()
        {
            InitializeComponent();
            service = new SupplierService(connectionString);
        }

        private void frm_Suppliers_Load(object sender, EventArgs e)
        {
            gridControl.MainView = gridView1;
            gridView1.GridControl = gridControl;

            LoadSuppliers();

            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            CustomizeGridView(gridView1);
        }

        private void LoadSuppliers()
        {
            DataTable dt = service.GetAllSuppliers();

            gridControl.DataSource = null;
            gridControl.DataSource = dt;

            gridView1.Columns.Clear();     // مهم
            gridView1.PopulateColumns();   // مهم

            // إخفاء المفتاح
            if (gridView1.Columns["SupplierId"] != null)
                gridView1.Columns["SupplierId"].Visible = false;

            // التأكد من ظهور اسم المورد
            if (gridView1.Columns["Name"] != null)
            {
                gridView1.Columns["Name"].Caption = "اسم المورد";
                gridView1.Columns["Name"].Visible = true;
            }

            if (gridView1.Columns["Phone"] != null)
                gridView1.Columns["Phone"].Caption = "الهاتف";

            if (gridView1.Columns["TaxNumber"] != null)
                gridView1.Columns["TaxNumber"].Caption = "الرقم الضريبي";

            if (gridView1.Columns["IsActive"] != null)
                gridView1.Columns["IsActive"].Caption = "نشط";

            if (gridView1.Columns["CreatedAt"] != null)
                gridView1.Columns["CreatedAt"].Caption = "تاريخ الإضافة";
        }


        private int? GetSelectedSupplierId()
        {
            if (gridView1.FocusedRowHandle < 0) return null;

            object val = gridView1.GetFocusedRowCellValue("SupplierId");
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToInt32(val);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
           
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
          
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(sender, e);
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            frm_SupplierEditor frm = new frm_SupplierEditor();
            if (frm.ShowDialog() == DialogResult.OK)
                LoadSuppliers();
        }

        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            int? id = GetSelectedSupplierId();
            if (id == null) return;

            frm_SupplierEditor frm = new frm_SupplierEditor(id);
            if (frm.ShowDialog() == DialogResult.OK)
                LoadSuppliers();
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            int? id = GetSelectedSupplierId();
            if (id == null) return;

            var confirm = MessageBox.Show("هل تريد حذف المورد المحدد؟", "تأكيد الحذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                service.DeleteSupplier(id.Value);
                LoadSuppliers();
            }
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
    }
}
