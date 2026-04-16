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
    public partial class frm_OutgoingCheques : Form
    {
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_OutgoingCheques()
        {
            InitializeComponent();
        }
        SupplierService cs;
        private ChequeService service;
        private void frm_OutgoingCheques_Load(object sender, EventArgs e)
        {
            service = new ChequeService(connectionString); // 🔥 مهم جدًا
            cbxFilterStatus.DataSource = new List<object>
{
    new { Id = 0, Name = "الكل" },
    new { Id = 1, Name = "Pending" },
    new { Id = 2, Name = "Paid" },
    new { Id = 3, Name = "Returned" }
};

            cbxFilterStatus.DisplayMember = "Name";
            cbxFilterStatus.ValueMember = "Id";
            cbxFilterStatus.SelectedIndex = 0;
            CustomizeGridView(gridView1);
            cs = new SupplierService(connectionString);
            var suppliers = cs.GetAllSuppliers();

            searchLookUpEditSupplier.Properties.DataSource = suppliers;
            searchLookUpEditSupplier.Properties.DisplayMember = "Name";
            searchLookUpEditSupplier.Properties.ValueMember = "SupplierId";

            searchLookUpEditSupplier.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            searchLookUpEditSupplier.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            searchLookUpEditSupplier.Properties.View.PopulateColumns();
            LoadCheques();
            dtFrom.Format = DateTimePickerFormat.Custom;
            dtFrom.CustomFormat = "dd/MM/yyyy";

            dtTo.Format = DateTimePickerFormat.Custom;
            dtTo.CustomFormat = "dd/MM/yyyy";

        }
        private void LoadCheques()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM OutgoingCheques";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridControl1.DataSource = dt;
            }
        }
  

    private void btnMarkCollected_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            int chequeId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("ChequeId")
            );

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
UPDATE OutgoingCheques
SET Status = 2,
    CollectionDate = GETDATE()
WHERE ChequeId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", chequeId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("تم تسجيل صرف الشيك");

            var status = Convert.ToInt32(gridView1.GetFocusedRowCellValue("Status"));

            if (status == 2)
            {
                MessageBox.Show("هذا الشيك تم صرفه مسبقًا");
                return;
            }

            LoadCheques();
        }

        private void btnMarkReturned_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            int chequeId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("ChequeId")
            );

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
UPDATE OutgoingCheques
SET Status = 3,
    CollectionDate = GETDATE()
WHERE ChequeId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", chequeId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("تم تسجيل صرف الشيك");


            LoadCheques();
        }

        private void txtChequeNo_TextChanged(object sender, EventArgs e)
        {
            txtSupplierName.Text = ""; // منع التعارض

            if (string.IsNullOrWhiteSpace(txtChequeNo.Text))
            {
                LoadCheques();
                return;
            }

            gridControl1.DataSource =
     service.SearchOutgoingByChequeNo(txtChequeNo.Text);
        }

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            txtChequeNo.Text = ""; // منع التعارض

            if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                LoadCheques();
                return;
            }

            gridControl1.DataSource =
    service.SearchOutgoingBySupplierName(txtSupplierName.Text);
        }

        private void cbxFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = "SELECT * FROM OutgoingCheques WHERE 1=1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                // ✅ الحل هنا
                int status = ((dynamic)cbxFilterStatus.SelectedItem).Id;

                if (status != 0)
                {
                    sql += " AND Status = @Status";
                    cmd.Parameters.AddWithValue("@Status", status);
                }

                cmd.CommandText = sql;

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gridControl1.DataSource = dt;
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

        private void searchLookUpEditSupplier_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditSupplier.EditValue == null)
            {
                LoadCheques();
                return;
            }

            if (!int.TryParse(searchLookUpEditSupplier.EditValue.ToString(), out int supplierId))
                return;

            // 🔥 فلترة مباشرة بالـ ID
            gridControl1.DataSource =
                service.SearchOutgoingBySupplierId(supplierId);
        }
        private void LoadChequesByDate(DateTime from, DateTime to)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT * FROM OutgoingCheques
WHERE DueDate >= @from
AND DueDate < @to
ORDER BY DueDate DESC";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to.AddDays(1)); // 🔥 مهم جداً

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gridControl1.DataSource = dt;
            }
        }

        private void btnFilterDate_Click(object sender, EventArgs e)
        {
            LoadChequesByDate(
       dtFrom.Value.Date,
      dtTo.Value.Date.AddDays(1)
   );
            UpdateTotalAmount();
        }
        private void UpdateTotalAmount()
        {
            decimal total = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                int rowHandle = gridView1.GetVisibleRowHandle(i);
                if (rowHandle < 0) continue;

                object val = gridView1.GetRowCellValue(rowHandle, "Amount");

                if (val != null && val != DBNull.Value)
                    total += Convert.ToDecimal(val);
            }

            lblTotalAmount.Text = "مجموع الشيكات: " + total.ToString("N2");
        }
    }
}
