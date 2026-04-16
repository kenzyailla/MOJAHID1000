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
using Accounting.Core.Services;
using Accounting.Core.Models;
using Accounting.Core; // أو اسم المشروع عندك
using Accounting.Core.Common;
using static Accounting.Core.Forms.frm_CustomerStatement;

namespace Accounting.Core.Forms


{
    public partial class frm_ProductMovement : Form
    {
        private ProductService ps;
        private SupplierService cs;


        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_ProductMovement()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cbxProduct.SelectedValue == null)
                return;

            int productId = Convert.ToInt32(cbxProduct.SelectedValue);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
DECLARE @OpeningBalance DECIMAL(18,3)

SELECT @OpeningBalance = ISNULL(SUM(Quantity),0)
FROM InventoryTransactions
WHERE ProductId = @ProductId
AND TransactionDate < @FromDate

SELECT
    t.TransactionDate,

    CASE 
        WHEN t.TransactionType = 0 THEN N'رصيد افتتاحي'
        WHEN t.TransactionType = 1 THEN N'مشتريات'
        WHEN t.TransactionType = 2 THEN N'مبيعات'
        WHEN t.TransactionType = 3 THEN N'مرتجع'
        ELSE N'أخرى'
    END AS TransactionName,

    t.ReferenceId,

    CASE WHEN t.Quantity > 0 THEN t.Quantity ELSE 0 END AS InQty,
    CASE WHEN t.Quantity < 0 THEN ABS(t.Quantity) ELSE 0 END AS OutQty,

    @OpeningBalance +
    SUM(t.Quantity) OVER (
        ORDER BY t.TransactionDate, t.TransactionId
        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
    ) AS RunningBalance

FROM InventoryTransactions t
WHERE t.ProductId = @ProductId
AND t.TransactionDate BETWEEN @FromDate AND @ToDate

ORDER BY t.TransactionDate, t.TransactionId";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                //cmd.Parameters.AddWithValue("@From", dtFrom.Value.Date);
                //cmd.Parameters.AddWithValue("@To", dtTo.Value.Date.AddDays(1));
                cmd.Parameters.AddWithValue("@FromDate", dtFrom.Value.Date);
                cmd.Parameters.AddWithValue("@ToDate", dtTo.Value.Date.AddDays(1));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gridControl1.DataSource = dt;
            }
        }
        private void LoadProducts()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = "SELECT ProductId, Name FROM Products WHERE IsActive = 1 ORDER BY Name";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxProduct.DataSource = dt;
                cbxProduct.DisplayMember = "Name";
                cbxProduct.ValueMember = "ProductId";
                cbxProduct.SelectedIndex = -1; // لا يحدد أول منتج تلقائياً
            }
        }

        private void frm_ProductMovement_Load(object sender, EventArgs e)
        {
            LoadProducts();
            CustomizeGridView(gridView1);

            dtFrom.Format = DateTimePickerFormat.Custom;
            dtFrom.CustomFormat = "dd/MM/yyyy";

            dtTo.Format = DateTimePickerFormat.Custom;
            dtTo.CustomFormat = "dd/MM/yyyy";


            ps = new ProductService(connectionString);
            cs = new SupplierService(connectionString);

            var products = ps.GetAllProducts();

            var productList = new List<Product>();
            productList.Add(new Product { ProductId = 0, Name = "الكل" });
            productList.AddRange(products);
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

            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;
            // مهم
        


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

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int productId))
                return;

            // 🔥 البحث داخل الكومبو وتحديد العنصر
            for (int i = 0; i < cbxProduct.Items.Count; i++)
            {
                var item = cbxProduct.Items[i];

                if (item is DataRowView row)
                {
                    if (Convert.ToInt32(row["ProductId"]) == productId)
                    {
                        cbxProduct.SelectedIndex = i;
                        break;
                    }
                }
                else if (item is Product p)
                {
                    if (p.ProductId == productId)
                    {
                        cbxProduct.SelectedIndex = i;
                        break;
                    }
                }
            }

            // 🔥 تنفيذ البحث مباشرة
            btnSearch.PerformClick();
        }
        //================================================================حديد
      

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            string transType = gridView1.GetFocusedRowCellValue("TransactionName")?.ToString();
            object refObj = gridView1.GetFocusedRowCellValue("ReferenceId");

            if (refObj == null || refObj == DBNull.Value)
                return;

            int refId = Convert.ToInt32(refObj);

            if (transType == "مشتريات")
            {
                InvoiceOpener.OpenBuyById(refId);
            }
            else if (transType == "مبيعات")
            {
                InvoiceOpener.OpenSaleById(refId);
            }
            else if (transType == "مرتجع")
            {
                InvoiceOpener.OpenReturnById(refId); // 🔥 الجديد
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataTable dt = gridControl1.DataSource as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة");
                return;
            }
            decimal finalBalance = 0;

            if (dt.Rows.Count > 0)
            {
                finalBalance = Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningBalance"]);
            }
            rptProductMovement rpt = new rptProductMovement();
            rpt.SetDataSource(dt);
            decimal totalIn = dt.AsEnumerable().Sum(r => r.Field<decimal>("InQty"));
            decimal totalOut = dt.AsEnumerable().Sum(r => r.Field<decimal>("OutQty"));
            string tafqeetQty = NumberConverter.ConvertToArabicWords(finalBalance);
            rpt.SetParameterValue("PTotalIn", totalIn);
            rpt.SetParameterValue("PTotalOut", totalOut);
            // 🟢 معلومات إضافية
            rpt.SetParameterValue("PProductName", cbxProduct.Text);
            rpt.SetParameterValue("PFromDate", dtFrom.Value);
            rpt.SetParameterValue("PToDate", dtTo.Value);
            rpt.SetParameterValue("PFinalBalance", finalBalance);
            rpt.SetParameterValue("PTafqeetQty", tafqeetQty);
            frm_ReportViewer frm = new frm_ReportViewer(rpt);
            frm.ShowDialog();
        }
    }
}
