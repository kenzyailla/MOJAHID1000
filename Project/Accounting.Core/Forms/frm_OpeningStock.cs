using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
namespace Accounting.Core.Forms
{
    public partial class frm_OpeningStock : Form
    {
        private readonly string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private DataTable dt;
        public frm_OpeningStock()
        {
            InitializeComponent();
        }

        private void frm_OpeningStock_Load(object sender, EventArgs e)
        {
            PrepareGrid();
            LoadProductsToGrid(); // تقدر تخليها بزر تحميل فقط إذا تحب
            CustomizeGridView(gridView1);
        }
        private void PrepareGrid()
        {
            dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("OpeningQty", typeof(decimal));
            dt.Columns.Add("CostPrice", typeof(decimal));   // يدوي
            dt.Columns.Add("TotalValue", typeof(decimal));  // تلقائي
            dt.Columns.Add("Notes", typeof(string));

            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();

            gridView1.Columns["ProductId"].Visible = false;
            gridView1.Columns["ProductName"].Caption = "المنتج";
            gridView1.Columns["OpeningQty"].Caption = "الرصيد الافتتاحي";
            gridView1.Columns["CostPrice"].Caption = "سعر التكلفة";
            gridView1.Columns["TotalValue"].Caption = "إجمالي القيمة";
            gridView1.Columns["Notes"].Caption = "ملاحظات";

            gridView1.Columns["ProductName"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["TotalValue"].OptionsColumn.AllowEdit = false; // لا يعدل يدويًا

            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsBehavior.Editable = true;
        }

        private void btnLoadProducts_Click(object sender, EventArgs e)
        {
            LoadProductsToGrid();
        }

        private void LoadProductsToGrid()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                DataTable dtProducts = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT ProductId, Name
FROM Products
WHERE IsActive = 1
ORDER BY Name", con))
                {
                    da.Fill(dtProducts);
                }

                DataTable dtOpening = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT ProductId, Quantity, CostPrice, Notes
FROM InventoryTransactions
WHERE TransactionType = 0", con))
                {
                    da.Fill(dtOpening);
                }

                dt.Clear();

                foreach (DataRow p in dtProducts.Rows)
                {
                    int pid = Convert.ToInt32(p["ProductId"]);
                    string name = p["Name"].ToString();

                    var existing = dtOpening.AsEnumerable()
                        .FirstOrDefault(r => Convert.ToInt32(r["ProductId"]) == pid);

                    decimal openingQty = 0m;
                    decimal costPrice = 0m;
                    string notes = "";

                    if (existing != null)
                    {
                        openingQty = existing["Quantity"] == DBNull.Value ? 0m : Convert.ToDecimal(existing["Quantity"]);
                        costPrice = existing["CostPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(existing["CostPrice"]);
                        notes = existing["Notes"] == DBNull.Value ? "" : existing["Notes"].ToString();
                    }

                    DataRow row = dt.NewRow();
                    row["ProductId"] = pid;
                    row["ProductName"] = name;
                    row["OpeningQty"] = openingQty;
                    row["CostPrice"] = costPrice;
                    decimal totalValue = GetCurrentStockValue(pid);
                    row["TotalValue"] = totalValue;
                    row["Notes"] = notes;

                    dt.Rows.Add(row);
                }
            }

            gridView1.RefreshData();
            gridView1.BestFitColumns();

            UpdateTotalLabel();
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
           
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();

            var rowsToSave = dt.AsEnumerable()
                .Where(r => r["OpeningQty"] != DBNull.Value && Convert.ToDecimal(r["OpeningQty"]) > 0m)
                .ToList();

            if (rowsToSave.Count == 0)
            {
                XtraMessageBox.Show("أدخل رصيد افتتاحي (أكبر من صفر) لمنتج واحد على الأقل.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 🔥 تحقق هل يوجد حركات
                    bool hasTransactions = false;

                    using (SqlCommand cmdCheck = new SqlCommand(@"
SELECT COUNT(*) 
FROM InventoryTransactions 
WHERE TransactionType <> 0", con, trans))
                    {
                        hasTransactions = Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0;
                    }

                    // 🔥 إذا يوجد حركات → اسأل المستخدم
                    if (hasTransactions)
                    {
                        DialogResult res = MessageBox.Show(
                            "يوجد حركات على المخزون.\nهل تريد إعادة ضبط الرصيد الافتتاحي؟",
                            "تحذير",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (res == DialogResult.No)
                        {
                            trans.Rollback();
                            return;
                        }
                    }

                    // 🔥 حذف الافتتاحي القديم
                    using (SqlCommand cmdDelete = new SqlCommand(@"
DELETE FROM InventoryTransactions 
WHERE TransactionType = 0", con, trans))
                    {
                        cmdDelete.ExecuteNonQuery();
                    }

                    // 🔥 إدخال جديد
                    foreach (var r in rowsToSave)
                    {
                        int productId = Convert.ToInt32(r["ProductId"]);
                        decimal qty = Convert.ToDecimal(r["OpeningQty"]);
                        decimal cost = Convert.ToDecimal(r["CostPrice"]);
                        string notes = (r["Notes"] ?? "").ToString();

                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO InventoryTransactions
(ProductId, Quantity, CostPrice, TransactionType, ReferenceId, TransactionDate, Notes)
VALUES
(@P, @Q, @C, 0, NULL, GETDATE(), @N)", con, trans))
                        {
                            cmd.Parameters.AddWithValue("@P", productId);
                            cmd.Parameters.AddWithValue("@Q", qty);
                            cmd.Parameters.AddWithValue("@C", cost);
                            cmd.Parameters.AddWithValue("@N", notes);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();

                    XtraMessageBox.Show("تم حفظ الرصيد الافتتاحي بنجاح ✅");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    XtraMessageBox.Show(ex.Message);
                }
            }

            lblOpeningValue.Text = "قيمة المخزون الافتتاحي: " + GetOpeningStockValue().ToString("N3");
            AppEvents.RefreshDashboard();


        }

        private decimal GetCurrentStockValue(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // مجموع الكمية الحالية
                SqlCommand cmdQty = new SqlCommand(@"
SELECT ISNULL(SUM(Quantity),0)
FROM InventoryTransactions
WHERE ProductId = @P", con);

                cmdQty.Parameters.AddWithValue("@P", productId);
                decimal qty = Convert.ToDecimal(cmdQty.ExecuteScalar());

                // آخر سعر شراء (أو تكلفة)
                SqlCommand cmdCost = new SqlCommand(@"
SELECT TOP 1 ISNULL(CostPrice,0)
FROM InventoryTransactions
WHERE ProductId = @P AND CostPrice IS NOT NULL
ORDER BY TransactionDate DESC", con);

                cmdCost.Parameters.AddWithValue("@P", productId);
                decimal cost = Convert.ToDecimal(cmdCost.ExecuteScalar());

                return qty * cost;
            }
        }

        private decimal GetOpeningStockValue()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(Quantity * CostPrice),0)
FROM InventoryTransactions
WHERE TransactionType = 0", con);

                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
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

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "OpeningQty" || e.Column.FieldName == "CostPrice")
            {
                decimal qty = 0m;
                decimal cost = 0m;

                object qtyObj = gridView1.GetRowCellValue(e.RowHandle, "OpeningQty");
                object costObj = gridView1.GetRowCellValue(e.RowHandle, "CostPrice");

                if (qtyObj != null && qtyObj != DBNull.Value)
                    qty = Convert.ToDecimal(qtyObj);

                if (costObj != null && costObj != DBNull.Value)
                    cost = Convert.ToDecimal(costObj);

                gridView1.SetRowCellValue(e.RowHandle, "TotalValue", qty * cost);
            }
        }
        private void UpdateTotalLabel()
        {
            if (dt == null) return;
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["TotalValue"] != DBNull.Value)
                    total += Convert.ToDecimal(row["TotalValue"]);
            }
            lblOpeningValue.Text = $"إجمالي قيمة المخزون: {total:N2}";
        }
    }
}
