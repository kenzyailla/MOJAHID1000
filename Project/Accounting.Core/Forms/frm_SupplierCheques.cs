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
    public partial class frm_SupplierCheques : DevExpress.XtraEditors.XtraForm
    {
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        public frm_SupplierCheques()
        {
            InitializeComponent();
        }
        private void LoadCheques()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = "SELECT * FROM SupplierCheques ORDER BY DueDate";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = dt;

                SqlCommand cmd = new SqlCommand(@"
SELECT SUM(Amount) 
FROM SupplierCheques
WHERE ISNULL(IsCleared,0) = 0", con);

                object result = cmd.ExecuteScalar();

                decimal total = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                lilbalance.Text = "الشيكات غير المصروفة: " + total.ToString("N3");
            }


        }
        private void frm_SupplierCheques_Load(object sender, EventArgs e)
        {
            LoadCheques();
            CustomizeGridView(gridView1);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCheques();
        }

        private void btnMarkCleared_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            int chequeId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("ChequeId")
            );

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = "UPDATE SupplierCheques SET IsCleared = 1 WHERE ChequeId = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", chequeId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("تم صرف الشيك ✔️");

            LoadCheques();
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            var cleared = gridView1.GetRowCellValue(e.RowHandle, "IsCleared");

            if (cleared != null && Convert.ToBoolean(cleared))
            {
                e.Appearance.BackColor = Color.LightGreen;
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