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
    public partial class frm_SalesVatDeclaration : Form
    {
        private readonly SalesVatDeclarationService _service;
        private readonly string _connectionString;
        public frm_SalesVatDeclaration(string connectionString)
        {
            InitializeComponent();
            _service = new SalesVatDeclarationService(connectionString);
            _connectionString = connectionString;
        }
        string connectionString =
   @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
     

        private void frm_SalesVatDeclaration_Load(object sender, EventArgs e)
        {
            dateFrom.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
            dateTo.DateTime = DateTime.Today;
            CustomizeGridView(gridView1);
           
        }

        private void btnCalculate_Click_1(object sender, EventArgs e)
        {
            try
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

                    // ✅ 1) تفاصيل الجريد (مبيعات + مرتجع)
                    string sqlDetails = @"
SELECT
    N'مبيعات' AS DocType,
    i.InvoiceId AS DocId,
    CAST(i.InvoiceNumber AS NVARCHAR(50)) AS DocNumber,
    i.InvoiceDate AS DocDate,
    p.Name AS ProductName,
    l.Quantity AS Qty,
    l.UnitPrice,
    l.TaxRate,
    l.TotalBeforeTax AS TotalBeforeTax,
    l.TotalTax       AS TotalTax,
    l.TotalAfterTax  AS TotalAfterTax
FROM Invoices i
INNER JOIN InvoiceLines l ON i.InvoiceId = l.InvoiceId
INNER JOIN Products p ON l.ProductId = p.ProductId
WHERE i.InvoiceType = 1
  AND i.InvoiceDate >= @FromDate
  AND i.InvoiceDate < DATEADD(DAY,1,@ToDate)

UNION ALL

SELECT
    N'مرتجع مبيعات' AS DocType,
    r.SalesReturnId AS DocId,
    CAST(r.SalesReturnId AS NVARCHAR(50)) AS DocNumber,
    CAST(r.ReturnDate AS DATE) AS DocDate,
    p.Name AS ProductName,
    -rl.Quantity AS Qty,
    rl.UnitPrice,
    rl.TaxRate,
    -rl.LineBeforeTax AS TotalBeforeTax,
    -rl.LineTax       AS TotalTax,
    -rl.LineAfterTax  AS TotalAfterTax
FROM SalesReturns r
INNER JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
INNER JOIN Products p ON rl.ProductId = p.ProductId
WHERE r.ReturnDate >= @FromDate
  AND r.ReturnDate < DATEADD(DAY,1,@ToDate)

ORDER BY DocDate DESC;";

                    DataTable dt = new DataTable();

                    using (SqlCommand cmd = new SqlCommand(sqlDetails, con))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    // ✅ 2) ربط الجريد
                    gridControl1.DataSource = dt;//تحت جديد
                                                 // 🔵 جدول المبيعات
                   



                        DataTable dtSales = new DataTable();
                    dtSales.Columns.Add("TaxRate", typeof(decimal));
                    dtSales.Columns.Add("BeforeTax", typeof(decimal));
                    dtSales.Columns.Add("Tax", typeof(decimal));
                    dtSales.Columns.Add("AfterTax", typeof(decimal));
                   
                    var salesGroups = dt.AsEnumerable()
                        .Where(r => r["DocType"].ToString() == "مبيعات")
                        .GroupBy(r => Convert.ToDecimal(r["TaxRate"]));

                    foreach (var g in salesGroups)
                    {
                        dtSales.Rows.Add(
                            g.Key,
                            g.Sum(r => Convert.ToDecimal(r["TotalBeforeTax"])),
                            g.Sum(r => Convert.ToDecimal(r["TotalTax"])),
                            g.Sum(r => Convert.ToDecimal(r["TotalAfterTax"]))
                        );
                    }

                    DataTable dtReturns = new DataTable();
                    dtReturns.Columns.Add("TaxRate", typeof(decimal));
                    dtReturns.Columns.Add("BeforeTax", typeof(decimal));
                    dtReturns.Columns.Add("Tax", typeof(decimal));
                    dtReturns.Columns.Add("AfterTax", typeof(decimal));

                    var returnGroups = dt.AsEnumerable()
                        .Where(r => r["DocType"].ToString() == "مرتجع مبيعات")
                        .GroupBy(r => Convert.ToDecimal(r["TaxRate"]));

                    foreach (var g in returnGroups)
                    {
                        dtReturns.Rows.Add(
                            g.Key,
                            g.Sum(r => Math.Abs(Convert.ToDecimal(r["TotalBeforeTax"]))),
                            g.Sum(r => Math.Abs(Convert.ToDecimal(r["TotalTax"]))),
                            g.Sum(r => Math.Abs(Convert.ToDecimal(r["TotalAfterTax"])))
                        );
                    }

                    gridControl2.DataSource = dtSales;
                    gridView2.PopulateColumns();
                    gridView2.BestFitColumns();
                    CustomizeGridView(gridView2); // 🔥 هنا
                    gridControl3.DataSource = dtReturns;
                    gridView3.PopulateColumns();
                    gridView3.BestFitColumns();
                    CustomizeGridView(gridView3); // 🔥 هنا



                    // ✅ 3) حساب المجاميع من نفس الداتا
                    CalculateTotals(dt);
                }
                CustomizeGridView(gridView2);
                CustomizeGridView(gridView3);

            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message);
            }
        }
        private void CalculateTotals(DataTable dt)
        {
            decimal salesBefore = 0;
            decimal salesTax = 0;
            decimal salesAfter = 0;

            decimal returnBefore = 0;
            decimal returnTax = 0;
            decimal returnAfter = 0;

            foreach (DataRow row in dt.Rows)
            {
                string type = row["DocType"].ToString();

                decimal before = row["TotalBeforeTax"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalBeforeTax"]);
                decimal tax = row["TotalTax"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalTax"]);
                decimal after = row["TotalAfterTax"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalAfterTax"]);

                if (type == "مبيعات")
                {
                    salesBefore += before;
                    salesTax += tax;
                    salesAfter += after;
                }
                else if (type == "مرتجع مبيعات")
                {
                    // المرتجع في الجريد سالب
                    returnBefore += Math.Abs(before);
                    returnTax += Math.Abs(tax);
                    returnAfter += Math.Abs(after);
                }
            }

            // 🔵 المبيعات
         
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
      
  

