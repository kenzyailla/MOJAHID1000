using Accounting.Core.Services;
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
    public partial class frm_TrialBalance : Form
    {
        AccountService service;

        string connectionString =
        @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_TrialBalance()
        {
            InitializeComponent();
            service = new AccountService(connectionString);
        }

        private void frm_TrialBalance_Load(object sender, EventArgs e)
        {
            datefrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
            dateto.DateTime = DateTime.Today;
            gridView1.RowCellStyle += gridView1_RowCellStyle;
            CustomizeGridView(gridView1);

        }

        private void btnshow_Click(object sender, EventArgs e)
        {
            DateTime from = datefrom.DateTime.Date;
            DateTime to = dateto.DateTime.Date;

            DataTable dt = service.GetTrialBalance(from, to);

            gridControl1.DataSource = dt;
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Debit"] != DBNull.Value)
                    totalDebit += Convert.ToDecimal(row["Debit"]);

                if (row["Credit"] != DBNull.Value)
                    totalCredit += Convert.ToDecimal(row["Credit"]);
            }

            lblTotalDebit.Text = $"إجمالي المدين : {totalDebit:N2}";
            lblTotalCredit.Text = $"إجمالي الدائن : {totalCredit:N2}";
           

            gridView1.PopulateColumns();

            gridView1.Columns["AccountCode"].Caption = "رقم الحساب";
            gridView1.Columns["AccountName"].Caption = "اسم الحساب";
            gridView1.Columns["Debit"].Caption = "مدين";
            gridView1.Columns["Credit"].Caption = "دائن";
            gridView1.Columns["Balance"].Caption = "الرصيد";

            lblTotalDebit.Font = new Font(lblTotalDebit.Font, FontStyle.Bold);
            lblTotalCredit.Font = new Font(lblTotalCredit.Font, FontStyle.Bold);

        }
        private void gridView1_RowCellStyle(object sender,
          DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            decimal debit = Convert.ToDecimal(
                gridView1.GetRowCellValue(e.RowHandle, "Debit") ?? 0);

            decimal credit = Convert.ToDecimal(
                gridView1.GetRowCellValue(e.RowHandle, "Credit") ?? 0);

            if (debit > 0)
                e.Appearance.BackColor = Color.LightBlue;

            if (credit > 0)
                e.Appearance.BackColor = Color.MistyRose;
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
