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
    public partial class frm_BuyVatDeclaration : Form
    {
        public frm_BuyVatDeclaration()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private void btnCalculate_Click(object sender, EventArgs e)
        {

            if (dateFrom.EditValue == null || dateTo.EditValue == null)
            {
                MessageBox.Show("الرجاء اختيار الفترة الزمنية");
                return;
            }

            DateTime fromDate = dateFrom.DateTime.Date;
            DateTime toDate = dateTo.DateTime.Date;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    CASE 
        WHEN Type = 1 THEN N'مشتريات'
        WHEN Type = 2 THEN N'مرتجع مشتريات'
    END AS OperationType,

    TaxRate,
    SUM(BeforeTax) AS BeforeTax,
    SUM(TaxAmount) AS TaxAmount,
    SUM(AfterTax) AS AfterTax

FROM
(
    -- المشتريات
    SELECT 
        1 AS Type,
        l.TaxRate,
        l.LineSubTotal AS BeforeTax,
        l.TaxAmount,
        l.LineSubTotal + l.TaxAmount AS AfterTax
    FROM BuyInvoices b
    INNER JOIN BuyInvoiceLines l ON b.BuyInvoiceId = l.BuyInvoiceId
    WHERE b.InvoiceDate >= @FromDate
      AND b.InvoiceDate < DATEADD(DAY,1,@ToDate)

    UNION ALL

    -- مرتجع المشتريات
    SELECT 
        2,
        rl.TaxRate,
        rl.LineBeforeTax,
        rl.LineTax,
        rl.LineAfterTax
    FROM BuyReturns r
    INNER JOIN BuyReturnLines rl ON r.BuyReturnId = rl.BuyReturnId
    WHERE r.ReturnDate >= @FromDate
      AND r.ReturnDate < DATEADD(DAY,1,@ToDate)

) X

GROUP BY Type, TaxRate
ORDER BY TaxRate
";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gridControl1.DataSource = dt;

                CalculateTotalsByRate(dt); // إذا عندك دالة تجميع
                AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
            }
           
        }
        //private void CalculateTotalsByRate(DataTable dt)
        //{
        //    // مساعد للحصول على قيمة صف معين
        //    decimal GetValue(string op, decimal rate, string col)
        //    {
        //        foreach (DataRow r in dt.Rows)
        //        {
        //            string type = r["OperationType"].ToString();
        //            decimal tr = Convert.ToDecimal(r["TaxRate"]);
        //            if (type == op && tr == rate)
        //                return r[col] == DBNull.Value ? 0 : Convert.ToDecimal(r[col]);
        //        }
        //        return 0;
        //    }

        //    // مشتريات
        //    lblBuy16Before.Text = GetValue("مشتريات", 16m, "BeforeTax").ToString("N2");
        //    lblBuy16Tax.Text = GetValue("مشتريات", 16m, "TaxAmount").ToString("N2");
        //    lblBuy16After.Text = GetValue("مشتريات", 16m, "AfterTax").ToString("N2");

        //    lblBuy4Before.Text = GetValue("مشتريات", 4m, "BeforeTax").ToString("N2");
        //    lblBuy4Tax.Text = GetValue("مشتريات", 4m, "TaxAmount").ToString("N2");
        //    lblBuy4After.Text = GetValue("مشتريات", 4m, "AfterTax").ToString("N2");

        //    lblBuy1Before.Text = GetValue("مشتريات", 1m, "BeforeTax").ToString("N2");
        //    lblBuy1Tax.Text = GetValue("مشتريات", 1m, "TaxAmount").ToString("N2");
        //    lblBuy1After.Text = GetValue("مشتريات", 1m, "AfterTax").ToString("N2");

        //    lblBuy0Before.Text = GetValue("مشتريات", 0m, "BeforeTax").ToString("N2");
        //    lblBuy0Tax.Text = GetValue("مشتريات", 0m, "TaxAmount").ToString("N2");
        //    lblBuy0After.Text = GetValue("مشتريات", 0m, "AfterTax").ToString("N2");

        //    // مرتجع مشتريات
        //    label14.Text = GetValue("مرتجع مشتريات", 16m, "BeforeTax").ToString("N2");
        //    label18.Text = GetValue("مرتجع مشتريات", 16m, "TaxAmount").ToString("N2");
        //    label25.Text = GetValue("مرتجع مشتريات", 16m, "AfterTax").ToString("N2");

        //    label15.Text = GetValue("مرتجع مشتريات", 4m, "BeforeTax").ToString("N2");
        //    label21.Text = GetValue("مرتجع مشتريات", 4m, "TaxAmount").ToString("N2");
        //    label26.Text = GetValue("مرتجع مشتريات", 4m, "AfterTax").ToString("N2");

        //    label16.Text = GetValue("مرتجع مشتريات", 1m, "BeforeTax").ToString("N2");
        //    label22.Text = GetValue("مرتجع مشتريات", 1m, "TaxAmount").ToString("N2");
        //    label27.Text = GetValue("مرتجع مشتريات", 1m, "AfterTax").ToString("N2");

        //    label17.Text = GetValue("مرتجع مشتريات", 0m, "BeforeTax").ToString("N2");
        //    label23.Text = GetValue("مرتجع مشتريات", 0m, "TaxAmount").ToString("N2");
        //    label28.Text = GetValue("مرتجع مشتريات", 0m, "AfterTax").ToString("N2");
        //    }

        private void CalculateTotalsByRate(DataTable dt)
        {
            decimal GetValue(string op, decimal rate, string col)
            {
                foreach (DataRow r in dt.Rows)
                {
                    string type = r["OperationType"].ToString();
                    decimal tr = Convert.ToDecimal(r["TaxRate"]);

                    if (type == op && tr == rate)
                        return r[col] == DBNull.Value ? 0 : Convert.ToDecimal(r[col]);
                }
                return 0;
            }

            // 🔥 تعريف النسب
            decimal[] rates = { 16m, 4m, 1m, 0m };

            // 🔥 مشتريات
            foreach (var rate in rates)
            {
                GetLabels("مشتريات", rate,
                    out Label lblBefore,
                    out Label lblTax,
                    out Label lblAfter);

                lblBefore.Text = GetValue("مشتريات", rate, "BeforeTax").ToString("N2");
                lblTax.Text = GetValue("مشتريات", rate, "TaxAmount").ToString("N2");
                lblAfter.Text = GetValue("مشتريات", rate, "AfterTax").ToString("N2");
            }

            // 🔥 مرتجع مشتريات
            foreach (var rate in rates)
            {
                GetLabels("مرتجع مشتريات", rate,
                    out Label lblBefore,
                    out Label lblTax,
                    out Label lblAfter);

                lblBefore.Text = GetValue("مرتجع مشتريات", rate, "BeforeTax").ToString("N2");
                lblTax.Text = GetValue("مرتجع مشتريات", rate, "TaxAmount").ToString("N2");
                lblAfter.Text = GetValue("مرتجع مشتريات", rate, "AfterTax").ToString("N2");
            }
        }
        private void GetLabels(string type, decimal rate,
    out Label before, out Label tax, out Label after)
        {
            before = null;
            tax = null;
            after = null;

            // 🔥 مشتريات
            if (type == "مشتريات")
            {
                if (rate == 16m) { before = lblBuy16Before; tax = lblBuy16Tax; after = lblBuy16After; }
                if (rate == 4m) { before = lblBuy4Before; tax = lblBuy4Tax; after = lblBuy4After; }
                if (rate == 1m) { before = lblBuy1Before; tax = lblBuy1Tax; after = lblBuy1After; }
                if (rate == 0m) { before = lblBuy0Before; tax = lblBuy0Tax; after = lblBuy0After; }
            }

            // 🔥 مرتجع مشتريات
            if (type == "مرتجع مشتريات")
            {
                if (rate == 16m) { before = label14; tax = label18; after = label25; }
                if (rate == 4m) { before = label15; tax = label21; after = label26; }
                if (rate == 1m) { before = label16; tax = label22; after = label27; }
                if (rate == 0m) { before = label17; tax = label23; after = label28; }
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frm_BuyVatDeclaration_Load(object sender, EventArgs e)
        {
            CustomizeGridView(gridView1);
            StyleTableLayout(tableLayoutPanel1);
            StyleTableLayout(tableLayoutPanel2);

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
        private void StyleTableLayout(TableLayoutPanel table)
        {
            table.BackColor = Color.White;
            table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            table.Padding = new Padding(5);

            // 🔥 المرور على كل العناصر داخل الجدول
            foreach (Control ctrl in table.Controls)
            {
                // 🔹 Label
                if (ctrl is Label lbl)
                {
                    lbl.AutoSize = false;
                    lbl.Dock = DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Font = new Font("Noto Kufi Arabic", 9F, FontStyle.Bold);

                    // 🔥 تلوين الهيدر (أول صف)
                    if (table.GetRow(lbl) == 0)
                    {
                        lbl.BackColor = Color.FromArgb(45, 85, 155); // أزرق احترافي
                        lbl.ForeColor = Color.White;
                    }
                    else
                    {
                        lbl.BackColor = Color.FromArgb(245, 247, 250);
                        lbl.ForeColor = Color.Black;
                    }
                }

                // 🔹 TextBox
                if (ctrl is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.None;
                    txt.BackColor = Color.White;
                    txt.TextAlign = HorizontalAlignment.Center;
                    txt.Font = new Font("Noto Kufi Arabic", 9F);
                }
            }
        }
    }
    }

