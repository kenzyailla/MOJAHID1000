
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Accounting.Core.Services;

namespace Accounting.Core.Forms
{
    public partial class frm_VatSummary : Form
    {
        private readonly string connectionString =
            @"Data Source=.\SQLEXPRESS;
              Initial Catalog=AccountingCoreDB;
              Integrated Security=True";

        VatSummaryService service;

        public frm_VatSummary()
        {
            InitializeComponent();
            service = new VatSummaryService(connectionString);
        }
        private void frm_VatSummary_Load(object sender, EventArgs e)
        {

            dateFrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
            dateTo.DateTime = DateTime.Today;

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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            var srv = new VatReportService(connectionString);

            DateTime fromDate = dateFrom.DateTime;
            DateTime toDate = dateTo.DateTime;

            DataTable dt = srv.GetVatSummary(fromDate, toDate);

            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.OptionsView.ShowFooter = true;

            // مجموع ضريبة المبيعات
            gridView1.Columns["SalesTax"].SummaryItem.SummaryType =
                DevExpress.Data.SummaryItemType.Sum;
            gridView1.Columns["SalesTax"].SummaryItem.DisplayFormat = "د.أ {0:N2}";

            // مجموع ضريبة المشتريات
            gridView1.Columns["BuyTax"].SummaryItem.SummaryType =
                DevExpress.Data.SummaryItemType.Sum;
            gridView1.Columns["BuyTax"].SummaryItem.DisplayFormat = "د.أ {0:N2}";

            // مجموع صافي الضريبة
            gridView1.Columns["NetTax"].SummaryItem.SummaryType =
                DevExpress.Data.SummaryItemType.Sum;
            gridView1.Columns["NetTax"].SummaryItem.DisplayFormat = "د.أ {0:N2}";

          

            // عناوين
            gridView1.Columns["TaxRate"].Caption = "نسبة الضريبة";
            gridView1.Columns["SalesBeforeTax"].Caption = "مبيعات قبل الضريبة";
            gridView1.Columns["SalesTax"].Caption = "ضريبة المبيعات";
            gridView1.Columns["BuyBeforeTax"].Caption = "مشتريات قبل الضريبة";
            gridView1.Columns["BuyTax"].Caption = "ضريبة المشتريات";
            gridView1.Columns["NetTax"].Caption = "صافي الضريبة";

            // تنسيق أرقام
            foreach (var name in new[] { "SalesBeforeTax", "SalesTax", "BuyBeforeTax", "BuyTax", "NetTax" })
            {
                gridView1.Columns[name].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns[name].DisplayFormat.FormatString = "N2";
            }

            // ✅ حساب الإجمالي النهائي (مخرجات/مدخلات/صافي)
            decimal salesVat = 0, buyVat = 0, netVat = 0;

            foreach (DataRow r in dt.Rows)
            {
                salesVat += Convert.ToDecimal(r["SalesTax"]);
                buyVat += Convert.ToDecimal(r["BuyTax"]);
                netVat += Convert.ToDecimal(r["NetTax"]);
            }

            txtSalesVat.Text = salesVat.ToString("N2");
            txtBuyVat.Text = buyVat.ToString("N2");
            txtNetVat.Text = netVat.ToString("N2");

        

        }
  
}
}