using Accounting.Core.Models;
using Accounting.Core.Services;
using DevExpress.XtraEditors.Repository;
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
    public partial class frm_ProductCard : Form
    {
        private ProductService productService;
        private List<Product> dtProducts;

        private bool isSelecting = false;
        public frm_ProductCard()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

       

        private void frm_ProductCard_Load(object sender, EventArgs e)
        {
            productService = new ProductService(connectionString);

            var products = productService.GetAllProducts();

            //searchLookUpEdit1.Properties.DataSource = products;
            //searchLookUpEdit1.Properties.DisplayMember = "Name";
            //searchLookUpEdit1.Properties.ValueMember = "ProductId";

            //searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            //searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            //searchLookUpEdit1.Properties.NullText = "";

            //// 🔥 مهم جدًا
            //searchLookUpEdit1.Properties.View.PopulateColumns();

            searchLookUpEdit1.Properties.DataSource = products;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "ProductId";

            searchLookUpEdit1.Properties.NullText = "";

            // 🔥 الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم المنتج");
           

            // 🔥 الشكل
            searchLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit1.Properties.View.BestFitColumns();

            // 🔥 الخط
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10);

            // 🔥 البحث
            searchLookUpEdit1.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            searchLookUpEdit1.Properties.ImmediatePopup = true;

            // 🔥 حجم الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;

            CustomizeGridView(gridView1);

            if (products.Count > 0)
                LoadProductCard(products[0].ProductId);

        }
        private void LoadProducts()
        {
            dtProducts = productService.GetAllProducts();

            cbxProducts.DataSource = dtProducts;
            cbxProducts.DisplayMember = "Name";
            cbxProducts.ValueMember = "ProductId";

            if (dtProducts.Count > 0)
            {
                LoadProductCard(dtProducts[0].ProductId);
            }
        }
        private void LoadProductCard(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    t.TransactionId,
    t.TransactionDate,

    t.TransactionType,        -- 🔥 رقم مهم للكود
    t.ReferenceId,            -- 🔥 مهم للربط

    CASE 
        WHEN t.TransactionType = 1 THEN N'شراء'
        WHEN t.TransactionType = 2 THEN N'بيع'
        WHEN t.TransactionType = 3 THEN N'مرتجع'
        WHEN t.TransactionType = 4 THEN N'مرتجع شراء'
        WHEN t.TransactionType = 5 THEN N'تعديل رصيد'
        ELSE N'رصيد اول المدة'
    END AS TransactionName,   -- 👈 للعرض فقط

    t.Quantity,

    SUM(t.Quantity) OVER (
        PARTITION BY t.ProductId
        ORDER BY t.TransactionDate, t.TransactionId
    ) AS RunningBalance

FROM InventoryTransactions t
WHERE t.ProductId = @ProductId
ORDER BY t.TransactionDate, t.TransactionId";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                  
                    gridControl1.DataSource = dt;

                    // 🔥 تنسيق الجريد
                    gridView1.Columns["TransactionType"].Visible = false;
                    gridView1.Columns["ReferenceId"].Visible = false;

                    gridView1.Columns["TransactionName"].Caption = "نوع العملية";
                    gridView1.Columns["Quantity"].Caption = "الكمية";
                    gridView1.Columns["RunningBalance"].Caption = "الرصيد";
                }
            }
           
        }

        private void cbxProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxProducts.SelectedValue == null) return;

            if (int.TryParse(cbxProducts.SelectedValue.ToString(), out int productId))
            {
                LoadProductCard(productId);
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            int type = Convert.ToInt32(gridView1.GetFocusedRowCellValue("TransactionType"));

            object refObj = gridView1.GetFocusedRowCellValue("ReferenceId");

            if (refObj == null || refObj == DBNull.Value)
            {
                MessageBox.Show("هذه الحركة ليست مرتبطة بفاتورة");
                return;
            }

            int refId = Convert.ToInt32(refObj);

            if (type == 1)
                new frm_BuyInvoiceEditor(refId).ShowDialog();

            else if (type == 2)
                new frm_InvoiceEditor(refId).ShowDialog();

            else if (type == 3)
                new frm_SalesReturn(refId).ShowDialog();

            else if (type == 4)
                new frm_BuyReturn(refId).ShowDialog();

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

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            int productId = Convert.ToInt32(searchLookUpEdit1.EditValue);

            LoadProductCard(productId);
        }
      
    }
}
