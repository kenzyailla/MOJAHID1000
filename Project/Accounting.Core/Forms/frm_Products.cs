using System;
using System.Windows.Forms;
using Accounting.Core.Services;
using Accounting.Core.Models;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Forms
{
    public partial class frm_Products : Form
    {
        ProductService service;


        public frm_Products()
        {
            InitializeComponent();

            service = new ProductService(
 @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True");

        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";
        private void frm_Products_Load(object sender, EventArgs e)
        {
            LoadProducts();
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
        private void LoadProducts()
        {
            //gridControl1.DataSource = service.GetAllProducts();
            var list = service.GetAllProducts();

            foreach (var p in list)
            {
                p.CurrentStock = GetCurrentStock(p.ProductId); // 🔥
            }

            gridControl1.DataSource = list;
            gridView1.PopulateColumns();
            if (gridView1.Columns["CurrentStock"] != null)
            {
                gridView1.Columns["CurrentStock"].Caption = "الرصيد الحالي";
            }
            lblTotalStock.Text = "إجمالي الكمية: " + GetTotalStockValue();

        }
        private decimal GetCurrentStock(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(Quantity),0)
FROM InventoryTransactions
WHERE ProductId = @p", con);

                cmd.Parameters.AddWithValue("@p", productId);

                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
        private decimal GetTotalStockValue()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(Quantity),0)
FROM InventoryTransactions", con);

                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

     
        private void btnAdd_Click(object sender, EventArgs e)
        {
            frm_ProductEditor frm = new frm_ProductEditor();
            frm.ShowDialog();

            LoadProducts();

            //frm_BuyInvoiceEditor frm = new frm_BuyInvoiceEditor();

            //if (frm.ShowDialog() == DialogResult.OK)
            //{
            //    LoadProducts(); // 🔥 هذا المهم جداً
            //}
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() == null)
                return;

            Product product = new Product();

            product.ProductId =
                Convert.ToInt32(gridView1.GetFocusedRowCellValue("ProductId"));

            product.Name =
                gridView1.GetFocusedRowCellValue("Name").ToString();

            product.Unit =
                gridView1.GetFocusedRowCellValue("Unit").ToString();

            product.Price =
                Convert.ToDecimal(gridView1.GetFocusedRowCellValue("Price"));

            product.TaxRate =
                Convert.ToDecimal(gridView1.GetFocusedRowCellValue("TaxRate"));

            frm_ProductEditor frm = new frm_ProductEditor(product);
            frm.ShowDialog();

            LoadProducts();
        }
        private void frm_Products_Activated(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRow() == null)
                return;

            int productId =
                Convert.ToInt32(gridView1.GetFocusedRowCellValue("ProductId"));

            if (MessageBox.Show("هل تريد حذف المنتج؟",
                "تأكيد",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            service.DeleteProduct(productId);

            LoadProducts();
        }
    }
}
