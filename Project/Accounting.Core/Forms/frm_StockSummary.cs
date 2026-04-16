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
    public partial class frm_StockSummary : DevExpress.XtraEditors.XtraForm
    {
        public frm_StockSummary()
        {
            InitializeComponent();
        }
        private string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private void frm_StockSummary_Load(object sender, EventArgs e)
        {
            LoadData();
            CustomizeGridView(gridView1);
        }
      

        private DataTable GetStockSummary()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT 
    p.ProductId,
    p.Name,

    ISNULL(SUM(CASE WHEN t.TransactionType = 0 THEN t.Quantity ELSE 0 END),0) AS OpeningQty,
    ISNULL(SUM(CASE WHEN t.TransactionType = 1 THEN t.Quantity ELSE 0 END),0) AS PurchaseQty,
    ISNULL(SUM(CASE WHEN t.TransactionType = 2 THEN t.Quantity ELSE 0 END),0) AS SaleQty,
    ISNULL(SUM(CASE WHEN t.TransactionType = 5 THEN t.Quantity ELSE 0 END),0) AS AdjustQty,

    ISNULL(SUM(t.Quantity),0) AS CurrentQty,

    -- القيمة الجديدة: الرصيد الحالي × سعر التكلفة الافتتاحي
    ISNULL(
        ISNULL(SUM(t.Quantity),0) *
        (SELECT TOP 1 CostPrice FROM InventoryTransactions 
         WHERE ProductId = p.ProductId AND TransactionType = 0 AND CostPrice IS NOT NULL 
         ORDER BY TransactionDate DESC)
    ,0) AS TotalValue

FROM Products p
LEFT JOIN InventoryTransactions t ON p.ProductId = t.ProductId
WHERE p.IsActive = 1
GROUP BY p.ProductId, p.Name
ORDER BY p.Name", con);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        private void LoadData()
        {
            DataTable dt = GetStockSummary();

            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();

            if (gridView1.Columns["ProductId"] != null)
                gridView1.Columns["ProductId"].Visible = false;

            if (gridView1.Columns["Name"] != null)
                gridView1.Columns["Name"].Caption = "المنتج";

            if (gridView1.Columns["OpeningQty"] != null)
                gridView1.Columns["OpeningQty"].Caption = "رصيد أول المدة";

            if (gridView1.Columns["PurchaseQty"] != null)
                gridView1.Columns["PurchaseQty"].Caption = "المشتريات";

            if (gridView1.Columns["SaleQty"] != null)
                gridView1.Columns["SaleQty"].Caption = "المبيعات";

            if (gridView1.Columns["AdjustQty"] != null)
                gridView1.Columns["AdjustQty"].Caption = "التعديل";

            if (gridView1.Columns["CurrentQty"] != null)
                gridView1.Columns["CurrentQty"].Caption = "الرصيد الحالي";

            if (gridView1.Columns["TotalValue"] != null)
                gridView1.Columns["TotalValue"].Caption = "قيمة المخزون";

            gridView1.BestFitColumns();

            decimal totalValue = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["TotalValue"] != DBNull.Value)
                    totalValue += Convert.ToDecimal(row["TotalValue"]);
            }

            lblTotalValue.Text = "إجمالي قيمة المخزون: " + totalValue.ToString("N3");
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