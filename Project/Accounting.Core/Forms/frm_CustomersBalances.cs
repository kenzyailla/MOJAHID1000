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
    public partial class frm_CustomersBalances : Form
    {
        CustomerService service;

        string connectionString =
        @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_CustomersBalances()
        {
            InitializeComponent();
            service = new CustomerService(connectionString);
        }

        private void frm_CustomersBalances_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = service.GetCustomersBalances();

            gridView1.PopulateColumns();

            if (gridView1.Columns["CustomerName"] != null)
                gridView1.Columns["CustomerName"].Caption = "العميل";

            if (gridView1.Columns["Sales"] != null)
                gridView1.Columns["Sales"].Caption = "المبيعات";

            if (gridView1.Columns["Returns"] != null)
                gridView1.Columns["Returns"].Caption = "المرتجع";

            if (gridView1.Columns["Receipts"] != null)
                gridView1.Columns["Receipts"].Caption = "المقبوض";

            if (gridView1.Columns["Balance"] != null)
                gridView1.Columns["Balance"].Caption = "الرصيد";

            if (gridView1.Columns["InvoicesCount"] != null)
                gridView1.Columns["InvoicesCount"].Caption = "عدد الفواتير";

            if (gridView1.Columns["LastTransaction"] != null)
                gridView1.Columns["LastTransaction"].Caption = "آخر حركة";

            CustomizeGridView(gridView1);
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
