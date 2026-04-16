using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;

namespace Accounting.Core.Forms
    {
        public partial class frm_PurchasesDetails : Form
        {
            private DateTime _date;
            private string connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

            public frm_PurchasesDetails(DateTime date)
            {
                InitializeComponent();
                _date = date;
            }

            private void frm_PurchasesDetails_Load(object sender, EventArgs e)
            {
                LoadData();
            CustomizeGridView(gridView1);
        }

            private void LoadData()
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string sql = @"
SELECT
    b.BuyInvoiceId,
    b.InvoiceNumber,
    b.InvoiceDate,
    s.Name AS SupplierName,
    b.TotalAfterTax,
    b.Notes
FROM BuyInvoices b
LEFT JOIN Suppliers s
    ON b.SupplierId = s.SupplierId
WHERE CAST(b.InvoiceDate AS DATE) = @date
ORDER BY b.BuyInvoiceId DESC";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@date", _date.Date);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                gridControl1.DataSource = dt;
                gridView1.PopulateColumns();

                if (gridView1.Columns["BuyInvoiceId"] != null)
                    gridView1.Columns["BuyInvoiceId"].Caption = "الرقم";

                if (gridView1.Columns["InvoiceNumber"] != null)
                    gridView1.Columns["InvoiceNumber"].Caption = "رقم الفاتورة";

                if (gridView1.Columns["InvoiceDate"] != null)
                    gridView1.Columns["InvoiceDate"].Caption = "التاريخ";

                if (gridView1.Columns["SupplierName"] != null)
                    gridView1.Columns["SupplierName"].Caption = "اسم المورد";

                if (gridView1.Columns["TotalAfterTax"] != null)
                    gridView1.Columns["TotalAfterTax"].Caption = "الإجمالي";

                if (gridView1.Columns["Notes"] != null)
                    gridView1.Columns["Notes"].Caption = "ملاحظات";

                gridView1.BestFitColumns();
            }
            }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (gridView1.FocusedRowHandle < 0)
                    return;

                object idObj = gridView1.GetFocusedRowCellValue("BuyInvoiceId");
                if (idObj == null || idObj == DBNull.Value)
                    return;

                int invoiceId = Convert.ToInt32(idObj);

                frm_BuyInvoiceEditor frm = new frm_BuyInvoiceEditor(invoiceId);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

