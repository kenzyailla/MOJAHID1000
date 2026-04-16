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
using Accounting.Core.Services;
using System.Data.SqlClient;
using DevExpress.XtraEditors;

namespace Accounting.Core.Forms
{
    public partial class frm_StockAdjustmentcs : DevExpress.XtraEditors.XtraForm
    {
        private readonly string _connectionString =
  @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_StockAdjustmentcs()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private void LoadProducts()
        {
            ProductService ps = new ProductService(connectionString);

            var data = ps.GetAllProducts();

            cbxProduct.Properties.DataSource = data;
            cbxProduct.Properties.DisplayMember = "Name";
            cbxProduct.Properties.ValueMember = "ProductId";

            cbxProduct.Properties.NullText = "";

            cbxProduct.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            cbxProduct.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            cbxProduct.Properties.ImmediatePopup = true;

            cbxProduct.Properties.ImmediatePopup = true;

            cbxProduct.Properties.View.Columns.Clear();
            cbxProduct.Properties.View.Columns.AddVisible("Name", "اسم المنتج");

            cbxProduct.Properties.View.OptionsView.ShowAutoFilterRow = true;

            cbxProduct.Properties.View.BestFitColumns();
        }
        private void LoadTypes()
        {
            cbxType.Items.Clear();
            cbxType.Items.Add("زيادة");
            cbxType.Items.Add("نقص");
            cbxType.SelectedIndex = 0;
        }

        private void frm_StockAdjustmentcs_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadTypes();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int productId = Convert.ToInt32(cbxProduct.EditValue);
                decimal qty = Convert.ToDecimal(txtQty.Text);
                decimal cost = Convert.ToDecimal(txtCost.Text);

                // 🔥 تحديد نوع الحركة
                if (cbxType.SelectedIndex == 1) // نقص
                    qty = qty * -1;

                AddInventoryTransaction(
                    productId,
                    qty,
                    cost,
                    5, // Adjustment
                    null
                );

                MessageBox.Show("تم إضافة التعديل بنجاح ✔️");

                LoadAdjustments(); // تحديث الجريد
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadAdjustments()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                DataTable dt = new DataTable();

                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT 
    t.ProductId,
    p.Name AS ProductName,
    t.Quantity,
    t.CostPrice,
    t.TransactionDate
FROM InventoryTransactions t
LEFT JOIN Products p ON t.ProductId = p.ProductId
WHERE t.TransactionType = 5
ORDER BY t.TransactionDate DESC", con);

                da.Fill(dt);

                gridControl1.DataSource = dt;
            }
        }
        private void AddInventoryTransaction(int productId, decimal qty, decimal unitPrice, int type, int? refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                // التحقق من عدم وجود نفس الحركة مسبقاً
                string checkSql = @"
IF NOT EXISTS (
    SELECT 1 FROM InventoryTransactions
    WHERE ProductId = @p AND ReferenceId = @r AND TransactionType = @t
)
BEGIN
    INSERT INTO InventoryTransactions
    (ProductId, Quantity, CostPrice, TransactionType, ReferenceId, TransactionDate)
    VALUES (@p, @q, @c, @t, @r, GETDATE())
END";
                using (SqlCommand cmd = new SqlCommand(checkSql, con))
                {
                    cmd.Parameters.AddWithValue("@p", productId);
                    cmd.Parameters.AddWithValue("@q", qty);
                    cmd.Parameters.AddWithValue("@c", unitPrice);
                    cmd.Parameters.AddWithValue("@t", type);
                    //cmd.Parameters.AddWithValue("@r", refId);
                    cmd.Parameters.AddWithValue("@r", (object)refId ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}