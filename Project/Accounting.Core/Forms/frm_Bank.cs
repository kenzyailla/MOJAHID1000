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
    public partial class frm_Bank : DevExpress.XtraEditors.XtraForm
    {
        private string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        public frm_Bank()
        {
            InitializeComponent();
        }

        private void frm_Bank_Load(object sender, EventArgs e)
        {
            LoadBank();
            CustomizeGridView(gridView1);
          
        }
     

        private void LoadBank()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT 
    Id,
    TransDate,
    Description,
    Debit,
    Credit,
    Balance,
    RefType,
    RefId
FROM BankTransactions
ORDER BY Id", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = dt;

                // 🔥 مهم جداً
                gridView1.PopulateColumns();

                // 🔥 الآن الأعمدة موجودة
                if (gridView1.Columns["Id"] != null)
                    gridView1.Columns["Id"].Visible = false;

                if (gridView1.Columns["RefType"] != null)
                    gridView1.Columns["RefType"].Visible = false;

                if (gridView1.Columns["RefId"] != null)
                    gridView1.Columns["RefId"].Visible = false;

                // أسماء الأعمدة
                gridView1.Columns["TransDate"].Caption = "التاريخ";
                gridView1.Columns["Description"].Caption = "البيان";
                gridView1.Columns["Debit"].Caption = "مدين";
                gridView1.Columns["Credit"].Caption = "دائن";
                gridView1.Columns["Balance"].Caption = "الرصيد";

                gridView1.BestFitColumns();

                // 🔥 حساب الرصيد
                decimal balance = 0;

                if (dt.Rows.Count > 0)
                {
                    balance = Convert.ToDecimal(
                        dt.Rows[dt.Rows.Count - 1]["Balance"]);
                }

                lblBalance.Text = "رصيد البنك: " + balance.ToString("N3");
            }
        }

        private void btnOpeningBalance_Click(object sender, EventArgs e)
        {
            frm_AddBankBalance frm = new frm_AddBankBalance();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                AddBankTransaction(
                    DateTime.Now,
                    "رصيد افتتاحي للبنك",
                    frm.Amount,
                    0,
                    "Opening",
                    null
                );

                MessageBox.Show("تم الإدخال ✔️");

                LoadBank();
            }
        }
        private void AddBankTransaction(DateTime date, string desc, decimal debit, decimal credit, string refType, int? refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 🔥 آخر رصيد
                SqlCommand cmdBalance = new SqlCommand(
                    "SELECT TOP 1 Balance FROM BankTransactions ORDER BY Id DESC", con);

                object lastBalanceObj = cmdBalance.ExecuteScalar();

                decimal lastBalance = lastBalanceObj != null ? Convert.ToDecimal(lastBalanceObj) : 0;

                decimal newBalance = lastBalance + debit - credit;

                // 🔥 إدخال الحركة
                SqlCommand cmd = new SqlCommand(@"
INSERT INTO BankTransactions
(TransDate, Description, Debit, Credit, Balance, RefType, RefId)
VALUES
(@date, @desc, @debit, @credit, @balance, @type, @ref)", con);

                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@debit", debit);
                cmd.Parameters.AddWithValue("@credit", credit);
                cmd.Parameters.AddWithValue("@balance", newBalance);
                cmd.Parameters.AddWithValue("@type", refType);
                cmd.Parameters.AddWithValue("@ref", (object)refId ?? DBNull.Value);

                cmd.ExecuteNonQuery();
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

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (gridView1.FocusedRowHandle < 0) return;

                string refType = gridView1.GetFocusedRowCellValue("RefType")?.ToString();
                object refIdObj = gridView1.GetFocusedRowCellValue("RefId");

                if (string.IsNullOrEmpty(refType))
                {
                    MessageBox.Show("لا يوجد نوع عملية");
                    return;
                }
                int refId = 0;
                if (refIdObj != null && refIdObj != DBNull.Value)
                    refId = Convert.ToInt32(refIdObj);
                

                // 🔥 فتح حسب النوع
                switch (refType)
                {
                    case "Invoice":
                        new frm_Invoices(refId).ShowDialog();
                        break;

                    case "Purchase":
                        new frm_BuyInvoiceEditor(refId).ShowDialog();
                        break;

                    case "Transfer":
                        MessageBox.Show("عملية تحويل\n\n" +
                            gridView1.GetFocusedRowCellValue("Description"));
                        break;

                    case "Opening":
                        MessageBox.Show("رصيد افتتاحي");
                        break;

                    case "OldCheque":
                        MessageBox.Show("شيك قديم");
                        break;

                    default:
                        MessageBox.Show("عملية غير مرتبطة");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

      
    }
}