using Accounting.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_Customers : Form   
    {
        CustomerService service;

        string connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_Customers()
        {
            InitializeComponent();
            service = new CustomerService(connectionString);
        }

        private void frm_Customers_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            CustomizeGridView(gridView1);
        }

        private void LoadCustomers()
        {
            gridControl1.DataSource = service.GetAllCustomers();

            gridView1.PopulateColumns();

            gridView1.Columns["CustomerId"].Visible = false;
            gridView1.Columns["IsActive"].Visible = false;
            gridView1.Columns["CreatedAt"].Visible = false;

            gridView1.Columns["Name"].Caption = "اسم العميل";
            var colPhone = gridView1.Columns.ColumnByFieldName("Phone");
            if (colPhone != null) colPhone.Caption = "الهاتف";
            gridView1.Columns["TaxNumber"].Caption = "الرقم الضريبي";
            // ترتيب تنازلي
            gridView1.Columns["CustomerId"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
          
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            frm_CustomerEditor frm = new frm_CustomerEditor();
            frm.ShowDialog();

            LoadCustomers();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRowCellValue("CustomerId") == null)
                return;

            Customer customer = new Customer();

            customer.CustomerId =
                Convert.ToInt32(gridView1.GetFocusedRowCellValue("CustomerId"));

            customer.Name =
                gridView1.GetFocusedRowCellValue("Name").ToString();

            customer.Phone =
                gridView1.GetFocusedRowCellValue("Phone")?.ToString();

            customer.TaxNumber =
                gridView1.GetFocusedRowCellValue("TaxNumber")?.ToString();

            frm_CustomerEditor frm = new frm_CustomerEditor(customer);
            frm.ShowDialog();

            LoadCustomers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRowCellValue("CustomerId") == null)
                return;

            int id = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("CustomerId"));

            if (MessageBox.Show("هل تريد حذف العميل؟",
                "تأكيد",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                service.DeleteCustomer(id);
                LoadCustomers();
            }
        }
     

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
