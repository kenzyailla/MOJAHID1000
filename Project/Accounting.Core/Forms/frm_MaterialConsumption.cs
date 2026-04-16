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
using DevExpress.XtraEditors.Repository;
using Accounting.Core.Services;

namespace Accounting.Core.Forms
{
   

    public partial class frm_MaterialConsumption : DevExpress.XtraEditors.XtraForm
    {
        public frm_MaterialConsumption()
        {
            InitializeComponent();
            inventoryService = new InventoryService(connectionString);
        }
        private readonly string connectionString =
 @"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";

        InventoryService inventoryService;
        private void frm_MaterialConsumption_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = CreateEmptyTable();

            gridView1.PopulateColumns();

            // 🔥 السماح بالإضافة
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            gridView1.OptionsBehavior.Editable = true;

            PrepareProductLookup();

            dtDate.Format = DateTimePickerFormat.Custom;
            dtDate.CustomFormat = "dd/MM/yyyy";
        }
        private void PrepareProductLookup()
        {
            RepositoryItemGridLookUpEdit repo = new RepositoryItemGridLookUpEdit();

            ProductService ps = new ProductService(connectionString);

            repo.DataSource = ps.GetAllProducts();
            repo.DisplayMember = "Name";
            repo.ValueMember = "ProductId";

            repo.NullText = "";
            repo.ImmediatePopup = true;

            repo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;

            repo.View.Columns.Clear();
            repo.View.Columns.AddVisible("Name", "اسم المادة");

            repo.View.Appearance.Row.Font = new Font("Noto Kufi Arabic", 9);
            repo.Appearance.Font = new Font("Noto Kufi Arabic", 10);

            gridControl1.RepositoryItems.Add(repo);
            gridView1.Columns["ProductId"].ColumnEdit = repo;
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Qty" || e.Column.FieldName == "CostPrice")
            {
                decimal qty = ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "Qty"));
                decimal price = ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "CostPrice"));

                gridView1.SetRowCellValue(e.RowHandle, "Total", qty * price);
            }
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                foreach (var rowHandle in Enumerable.Range(0, gridView1.RowCount))
                {
                    object productObj = gridView1.GetRowCellValue(rowHandle, "ProductId");

                    // 🔥 تجاهل الصفوف الفارغة
                    if (productObj == null || productObj == DBNull.Value)
                        continue;

                    int productId = Convert.ToInt32(productObj);

                    // 🔥 تأكد أن ID صحيح
                    if (productId <= 0)
                        continue;

                    decimal qty = ToDecimal(gridView1.GetRowCellValue(rowHandle, "Qty"));
                    decimal cost = ToDecimal(gridView1.GetRowCellValue(rowHandle, "CostPrice"));

                    inventoryService.AddInventoryTransaction(productId, -qty, cost, 6, null);
                }
                MessageBox.Show("تم الحفظ");
            }

            MessageBox.Show("تم تسجيل استهلاك المواد ✔️");

            AppEvents.RefreshDashboard(); // تحديث

            this.Close();
        }
        private DataTable CreateEmptyTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("Qty", typeof(decimal));
            dt.Columns.Add("CostPrice", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));

            return dt;
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToDecimal(value);
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "ProductId")
            {
                object productObj = gridView1.GetRowCellValue(e.RowHandle, "ProductId");

                if (productObj == null || productObj == DBNull.Value)
                    return;

                int productId = Convert.ToInt32(productObj);

                decimal costPrice = GetLastCostPrice(productId);

                gridView1.SetRowCellValue(e.RowHandle, "CostPrice", costPrice);
            }

            if (e.Column.FieldName == "Qty" || e.Column.FieldName == "CostPrice")
            {
                decimal qty = ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "Qty"));
                decimal price = ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "CostPrice"));

                gridView1.SetRowCellValue(e.RowHandle, "Total", qty * price);
            }
        }
        private decimal GetLastCostPrice(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT TOP 1 CostPrice
FROM InventoryTransactions
WHERE ProductId = @p
  AND CostPrice IS NOT NULL
ORDER BY TransactionDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@p", productId);

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return 0;

                    return Convert.ToDecimal(result);
                }
            }
        }
      
    }
}