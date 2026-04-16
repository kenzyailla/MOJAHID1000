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
    public partial class frm_CustomersAging : Form
    {
        CustomerService service;

        string connectionString =
        @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        public frm_CustomersAging()
        {
            InitializeComponent();
            service = new CustomerService(connectionString);
        }

        private void frm_CustomersAging_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = service.GetCustomersAging();

            gridView1.Columns["CustomerName"].Caption = "العميل";
            gridView1.Columns["0_30"].Caption = "0 - 30 يوم";
            gridView1.Columns["31_60"].Caption = "31 - 60 يوم";
            gridView1.Columns["61_90"].Caption = "61 - 90 يوم";
            gridView1.Columns["90_Plus"].Caption = "أكثر من 90 يوم";
            gridView1.Columns["TotalBalance"].Caption = "الإجمالي";

            CustomizeGridView(gridView1);

            CalculateTotals();   // ⭐ حساب المجاميع
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            object val = gridView1.GetFocusedRowCellValue("CustomerId");

            if (val == null)
                return;

            int customerId = Convert.ToInt32(val);

            frm_CustomerStatement frm = new frm_CustomerStatement();

            frm.Show();

            frm.LoadCustomer(customerId);
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
        private decimal GetDecimalValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToDecimal(value);
        }
        private void CalculateTotals()
        {
            decimal sum0_30 = 0;
            decimal sum31_60 = 0;
            decimal sum61_90 = 0;
            decimal sum90Plus = 0;
            decimal total = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                sum0_30 += GetDecimalValue(gridView1.GetRowCellValue(i, "0_30"));
                sum31_60 += GetDecimalValue(gridView1.GetRowCellValue(i, "31_60"));
                sum61_90 += GetDecimalValue(gridView1.GetRowCellValue(i, "61_90"));
                sum90Plus += GetDecimalValue(gridView1.GetRowCellValue(i, "90_Plus"));
                total += GetDecimalValue(gridView1.GetRowCellValue(i, "TotalBalance"));
            }

            lbl0_30.Text = $"{sum0_30:N3} د.أ";
            lbl31_60.Text = $"{sum31_60:N3} د.أ";
            lbl61_90.Text = $"{sum61_90:N3} د.أ";
            lbl90Plus.Text = $"{sum90Plus:N3} د.أ";
            label5.Text = $"{total:N3} د.أ";
        }

        private void btnPrintAging_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. جلب البيانات
                DataTable dt = service.GetCustomersAging();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات");
                    return;
                }

                // 2. حساب المجاميع
                decimal sum0_30 = 0;
                decimal sum31_60 = 0;
                decimal sum61_90 = 0;
                decimal sum90Plus = 0;
                decimal total = 0;

                foreach (DataRow row in dt.Rows)
                {
                    sum0_30 += Convert.ToDecimal(row["0_30"] ?? 0);
                    sum31_60 += Convert.ToDecimal(row["31_60"] ?? 0);
                    sum61_90 += Convert.ToDecimal(row["61_90"] ?? 0);
                    sum90Plus += Convert.ToDecimal(row["90_Plus"] ?? 0);
                    total += Convert.ToDecimal(row["TotalBalance"] ?? 0);
                }

                // 3. تقرير
                rptCustomersAging rpt = new rptCustomersAging();
                rpt.SetDataSource(dt);

                // 4. باراميترات المجاميع
                rpt.SetParameterValue("pSum0_30", sum0_30);
                rpt.SetParameterValue("pSum31_60", sum31_60);
                rpt.SetParameterValue("pSum61_90", sum61_90);
                rpt.SetParameterValue("pSum90Plus", sum90Plus);
                rpt.SetParameterValue("pTotal", total);

                frm_ReportViewer frm = new frm_ReportViewer(rpt);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
