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
    public partial class frm_payment : Form
    {
        public frm_payment()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private void frm_payment_Load(object sender, EventArgs e)
        {
            LoadPayments();
            CustomizeGridView(gridView1);
            SetupButtonsAppearance();
        }

        private void LoadPayments()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
        SELECT 
            p.BuyPaymentId,
            s.Name AS SupplierName,
            p.PaymentDate,
            p.Amount,
            p.PaymentMethod
        FROM BuyPayments p
        JOIN Suppliers s ON p.SupplierId = s.SupplierId
        ORDER BY p.BuyPaymentId DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = dt;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0) return;

            int id = Convert.ToInt32(
                gridView1.GetRowCellValue(
                    gridView1.FocusedRowHandle,
                    "BuyPaymentId"));

            if (MessageBox.Show("هل تريد حذف السند؟",
                "تأكيد",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                BuyPaymentService service =
                    new BuyPaymentService(connectionString);

                service.DeletePayment(id);

                LoadPayments();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frm_BuyPayment frm = new frm_BuyPayment();
            frm.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0) return;

            int id = Convert.ToInt32(
                gridView1.GetRowCellValue(
                    gridView1.FocusedRowHandle,
                    "BuyPaymentId"));

            frm_BuyPayment frm = new frm_BuyPayment(id);
            frm.ShowDialog();

            LoadPayments();
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
           
        }

    
    }
}
