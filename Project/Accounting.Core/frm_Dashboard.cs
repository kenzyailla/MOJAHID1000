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
using DevExpress.XtraCharts;
using Accounting.Core.Forms;

namespace Accounting.Core
{
    public partial class frm_Dashboard : DevExpress.XtraEditors.XtraForm
    {
   

        public frm_Dashboard()
        {
            InitializeComponent();
        }
        private string _connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private readonly string connectionString =
    @"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";

        private Label lblPurchases;

        private void frm_Dashboard_Load(object sender, EventArgs e)
        {
            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.AutoScroll = true;
    
            flow.Controls.Add(CreateCard("💰 الصندوق", Color.SeaGreen, out lblCash));
            flow.Controls.Add(CreateCard("🏦 البنك", Color.RoyalBlue, out lblBank));
            flow.Controls.Add(CreateCard("📈 المبيعات اليوم", Color.Orange, out lblSales));
            //flow.Controls.Add(CreateCard("💵 الأرباح", Color.DarkViolet, out lblProfit));
            flow.Controls.Add(CreateCard("🧾 الشيكات", Color.Firebrick, out lblCheques));
            flow.Controls.Add(CreateCard("🧮 عدد الفواتير", Color.DarkCyan, out lblInvoices));
            flow.Controls.Add(CreateCard("🛒 المشتريات اليوم", Color.SaddleBrown, out lblPurchases));
            this.Controls.Add(flow);
            LoadPurchasesChart();
            LoadDashboard();
            LoadSalesChart();
            LoadTopProductsChart();
            LoadExpensesChart();
            LoadStockChart();
            frm_Main.DashboardNeedsRefresh += RefreshDashboard;
            chartExpenses.ObjectSelected += chartExpenses_ObjectSelected;
        }
        private void RefreshDashboard()
        {
            LoadDashboard();
        }
        private void LoadDashboard()
        {
            lblCash.Text = GetCashBalance().ToString("N2");
            lblBank.Text = GetBankBalance().ToString("N2");
            lblSales.Text = GetTodaySales().ToString("N2");
            //lblProfit.Text = GetTodayProfit().ToString("N2");
            lblCheques.Text = GetPendingCheques().ToString("N2");
            lblInvoices.Text = GetTodayInvoicesCount().ToString();
            lblPurchases.Text = GetTodayPurchases().ToString("N2");
        }
        private decimal GetCashBalance()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 Balance 
FROM CashTransactions 
ORDER BY CashId DESC", con);

                object result = cmd.ExecuteScalar();

                return result != null && result != DBNull.Value
                    ? Convert.ToDecimal(result)
                    : 0m;
            }
        }

        private decimal GetBankBalance()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                object r = new SqlCommand(
                    "SELECT TOP 1 Balance FROM BankTransactions ORDER BY Id DESC", con).ExecuteScalar();
                return r != null ? Convert.ToDecimal(r) : 0;
            }
        }

        private decimal GetTodaySales()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                return Convert.ToDecimal(new SqlCommand(@"
SELECT ISNULL(SUM(TotalAfterTax),0)
FROM Invoices
WHERE CAST(InvoiceDate AS DATE)=CAST(GETDATE() AS DATE)", con).ExecuteScalar());
            }
        }

        private decimal GetTodayProfit()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                return Convert.ToDecimal(new SqlCommand(@"
SELECT ISNULL(SUM((il.UnitPrice - p.CostPrice)*il.Quantity),0)
FROM InvoiceLines il
JOIN Products p ON il.ProductId=p.ProductId
JOIN Invoices i ON il.InvoiceId=i.InvoiceId
WHERE CAST(i.InvoiceDate AS DATE)=CAST(GETDATE() AS DATE)", con).ExecuteScalar());
            }
        }

        private decimal GetPendingCheques()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                return Convert.ToDecimal(new SqlCommand(@"
SELECT ISNULL(SUM(Amount),0)
FROM SupplierCheques
WHERE ISNULL(IsCleared,0)=0", con).ExecuteScalar());
            }
        }

        private int GetTodayInvoicesCount()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                return Convert.ToInt32(new SqlCommand(@"
SELECT COUNT(*)
FROM Invoices
WHERE CAST(InvoiceDate AS DATE)=CAST(GETDATE() AS DATE)", con).ExecuteScalar());
            }
        }
        private Panel CreateCard(string title, Color color, out Label lblValue)
        {
            Panel panel = new Panel();
            panel.Width = 220;
            panel.Height = 120;
            panel.BackColor = color;
            panel.Margin = new Padding(15);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.AutoSize = true;

            lblValue = new Label();
            lblValue.Text = "0";
            lblValue.ForeColor = Color.White;
            lblValue.Font = new Font("Noto Kufi Arabic", 16, FontStyle.Bold);
            lblValue.Location = new Point(10, 50);
            lblValue.AutoSize = true;

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblValue);

            return panel;
        }
        private void LoadSalesChart()
        {
            chartSales.Series.Clear();

            Series series = new Series("", ViewType.Bar);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    CAST(InvoiceDate AS DATE) AS Day,
    SUM(TotalAfterTax) AS Total
FROM Invoices
WHERE InvoiceDate >= DATEADD(DAY, -7, GETDATE())
GROUP BY CAST(InvoiceDate AS DATE)
ORDER BY Day";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    DateTime day = Convert.ToDateTime(rd["Day"]);
                    decimal total = Convert.ToDecimal(rd["Total"]);

                    series.Points.Add(new SeriesPoint(day, total));
                }
            }

            chartSales.Series.Add(series);
            chartSales.Titles.Clear();

            ChartTitle title = new ChartTitle();
            title.Text = "المبيعات";
            title.Font = new Font("Noto Kufi Arabic", 12, FontStyle.Bold);

            chartSales.Titles.Add(title);
        }

        private void LoadTopProductsChart()
        {
            chartTopProducts.Series.Clear();

            Series series = new Series("", ViewType.Pie);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT TOP 10 
    p.Name,
    SUM(il.Quantity) AS Qty
FROM InvoiceLines il
JOIN Products p ON il.ProductId = p.ProductId
GROUP BY p.Name
ORDER BY Qty DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    string name = rd["Name"].ToString();
                    decimal qty = Convert.ToDecimal(rd["Qty"]);

                    series.Points.Add(new SeriesPoint(name, qty));
                }
                series.Label.TextPattern = "{A}: {V} ({VP:P0})";
            }

            chartTopProducts.Series.Add(series);

            chartTopProducts.Titles.Clear();

            ChartTitle title = new ChartTitle();
            title.Text = "الاصناف";
            title.Font = new Font("Noto Kufi Arabic", 12, FontStyle.Bold);

            chartTopProducts.Titles.Add(title);
        }

        private void LoadExpensesChart()
        {
            chartExpenses.Series.Clear();

            Series series = new Series("", ViewType.Bar);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    CAST(TransDate AS DATE) AS Day,
    SUM(Credit) AS Total
FROM CashTransactions
WHERE RefType = 'Expense'
AND TransDate >= DATEADD(DAY, -7, GETDATE())
GROUP BY CAST(TransDate AS DATE)
ORDER BY Day";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                   
                    DateTime day = Convert.ToDateTime(rd["Day"]);
                   decimal total = Convert.ToDecimal(rd["Total"]);

                    series.Points.Add(new SeriesPoint(day, total));
                }
            }

            chartExpenses.Series.Add(series);

            // 🔥 تحسين الشكل
            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series.Label.TextPattern = "{V:N2}";

            chartExpenses.Titles.Clear();

            ChartTitle title = new ChartTitle();
            title.Text = "المصاريف";
            title.Font = new Font("Noto Kufi Arabic", 12, FontStyle.Bold);

            chartExpenses.Titles.Add(title);

        }

        private void chartTopProducts_ObjectSelected(object sender, HotTrackEventArgs e)
        {
            try
            {
                SeriesPoint point = e.AdditionalObject as SeriesPoint;

                if (point == null) return;
                string productName = point.Argument.ToString();

                ShowProductDetails(productName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowDayDetails(DateTime date)
        {
            frm_DayDetails frm = new frm_DayDetails(date);
            frm.ShowDialog();
        }
        private void ShowProductDetails(string productName)
        {
            frm_ProductDetails frm = new frm_ProductDetails(productName);
            frm.ShowDialog();
        }
        private void LoadStockChart()
        {
            chartStock.Series.Clear();

            Series series = new Series("", ViewType.Bar);
          
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT TOP 25
    p.Name,
    ISNULL(SUM(t.Quantity),0) AS Qty
FROM Products p
LEFT JOIN InventoryTransactions t 
    ON p.ProductId = t.ProductId
GROUP BY p.Name
ORDER BY Qty DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    string name = rd["Name"].ToString();
                    decimal qty = Convert.ToDecimal(rd["Qty"]);

                    series.Points.Add(new SeriesPoint(name, qty));
                }
            }

            chartStock.Series.Add(series);

            // 🔥 تحسين الشكل
            series.Label.TextPattern = "{V:N0}";
            series.LegendTextPattern = "{A}";

            chartStock.Titles.Clear();

            ChartTitle title = new ChartTitle();
            title.Text = "المخزون";
            title.Font = new Font("Noto Kufi Arabic", 12, FontStyle.Bold);

            chartStock.Titles.Add(title);

            chartStock.ObjectSelected += (s, e) =>
            {
                SeriesPoint p = e.AdditionalObject as SeriesPoint;
                if (p == null) return;

                string productName = p.Argument.ToString();

                frm_ProductDetails frm = new frm_ProductDetails(productName);
                frm.ShowDialog();
            };
        }
        private void chartExpenses_ObjectSelected(object sender, HotTrackEventArgs e)
        {
            try
            {
                SeriesPoint point = e.AdditionalObject as SeriesPoint;

                if (point == null) return;

                DateTime selectedDate = Convert.ToDateTime(point.Argument);

                ShowExpensesDetails(selectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowExpensesDetails(DateTime date)
        {
            frm_ExpensesDetails frm = new frm_ExpensesDetails(date);
            frm.ShowDialog();
        }
        private void LoadPurchasesChart()
        {
            chartPurchases.Series.Clear();

            Series series = new Series("", ViewType.Bar);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    CAST(InvoiceDate AS DATE) AS Day,
    SUM(TotalAfterTax) AS Total
FROM BuyInvoices
WHERE InvoiceDate >= DATEADD(DAY, -7, GETDATE())
GROUP BY CAST(InvoiceDate AS DATE)
ORDER BY Day";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    DateTime day = Convert.ToDateTime(rd["Day"]);
                    decimal total = Convert.ToDecimal(rd["Total"]);

                    series.Points.Add(new SeriesPoint(day, total));
                }
            }

            chartPurchases.Series.Add(series);

            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series.Label.TextPattern = "{V:N2}";
            series.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.True;
            series.ToolTipPointPattern = "📅 {A:dd/MM/yyyy}\n💰 {V:N2}";

            chartPurchases.Titles.Clear();

            ChartTitle title = new ChartTitle();
            title.Text = "المشتريات";
            title.Font = new Font("Noto Kufi Arabic", 12, FontStyle.Bold);

            chartPurchases.Titles.Add(title);
        }

        private void chartPurchases_ObjectSelected(object sender, HotTrackEventArgs e)
        {
          
                try
                {
                    SeriesPoint point = e.AdditionalObject as SeriesPoint;

                    if (point == null) return;

                    DateTime selectedDate = Convert.ToDateTime(point.Argument);

                    ShowPurchasesDetails(selectedDate);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
           
        }
        private void ShowPurchasesDetails(DateTime date)
        {
            frm_PurchasesDetails frm = new frm_PurchasesDetails(date);
            frm.ShowDialog();
        }
        private decimal GetTodayPurchases()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT ISNULL(SUM(TotalAfterTax),0)
FROM BuyInvoices
WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)";

                SqlCommand cmd = new SqlCommand(sql, con);

                object result = cmd.ExecuteScalar();

                return result != null && result != DBNull.Value
                    ? Convert.ToDecimal(result)
                    : 0;
            }
        }

        private void chartSales_ObjectSelected(object sender, HotTrackEventArgs e)
        {
            try
            {
                SeriesPoint point = e.AdditionalObject as SeriesPoint;

                if (point == null) return;

                DateTime selectedDate = Convert.ToDateTime(point.Argument);

                ShowSalesDetails(selectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowSalesDetails(DateTime date)
        {
            frm_SalesDetails frm = new frm_SalesDetails(date);
            frm.ShowDialog();
        }

    }
}