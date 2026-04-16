using Accounting.Core.Models;
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
using static Accounting.Core.Forms.frm_CustomerStatement;

namespace Accounting.Core.Forms
{
  

    public partial class frm_ProductLedger : Form
    {
        private ProductService ps;
        private SupplierService cs;

        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        public frm_ProductLedger()
        {
            InitializeComponent();
        }
    
     
        private void frm_ProductLedger_Load(object sender, EventArgs e)
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


            //searchLookUpEdit1.Properties.DataSource = products;
            //searchLookUpEdit1.Properties.DisplayMember = "Name";
            //searchLookUpEdit1.Properties.ValueMember = "ProductId";

            //searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            //searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            //searchLookUpEdit1.Properties.NullText = "";

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

            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;

        }
        private void LoadProducts()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT ProductId, Name FROM Products WHERE IsActive=1 ORDER BY Name", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxProduct.DataSource = dt;
                cbxProduct.DisplayMember = "Name";
                cbxProduct.ValueMember = "ProductId";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cbxProduct.SelectedValue == null) return;

            int productId = Convert.ToInt32(searchLookUpEdit1.EditValue);
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
DECLARE @ProductId INT = @pProductId;
DECLARE @From DATETIME = @pFrom;
DECLARE @To DATETIME = DATEADD(DAY, 1, @pTo);

;WITH x AS
(
    SELECT
        t.TransactionId,
        t.TransactionDate,
        t.TransactionType,
        t.ReferenceId,
        t.Quantity,
        t.Notes,
        CASE WHEN t.Quantity > 0 THEN t.Quantity ELSE 0 END AS InQty,
        CASE WHEN t.Quantity < 0 THEN ABS(t.Quantity) ELSE 0 END AS OutQty
    FROM InventoryTransactions t
    WHERE t.ProductId = @ProductId
      AND t.TransactionDate >= @From
      AND t.TransactionDate <  @To
)
SELECT
    x.TransactionDate,
    CASE TransactionType
        WHEN 0 THEN N'رصيد افتتاحي'
        WHEN 1 THEN N'مشتريات'
        WHEN 2 THEN N'مبيعات'
        ELSE  N'غير معروف'
    END AS TransName,
    COALESCE(
        CASE WHEN x.TransactionType = 1 THEN CAST(b.InvoiceNumber AS NVARCHAR(50)) END,
        CASE WHEN x.TransactionType = 2 THEN i.InvoiceNumber END,
        CAST(x.ReferenceId AS NVARCHAR(50)),
        N''
    ) AS RefNo,
    x.InQty,
    x.OutQty,
    SUM(x.Quantity) OVER(ORDER BY x.TransactionDate, x.TransactionId) AS RunningBalance,
    x.Notes
FROM x
LEFT JOIN BuyInvoices b
    ON x.TransactionType = 1 AND x.ReferenceId = b.BuyInvoiceId
LEFT JOIN Invoices i
    ON x.TransactionType = 2 AND x.ReferenceId = i.InvoiceId
ORDER BY x.TransactionDate, x.TransactionId;";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@pProductId", productId);
                cmd.Parameters.AddWithValue("@pFrom", from);
                cmd.Parameters.AddWithValue("@pTo", to);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = dt;
                gridView1.PopulateColumns();

                // عناوين عربية
                gridView1.Columns["TransactionDate"].Caption = "التاريخ";
                gridView1.Columns["TransName"].Caption = "نوع الحركة";
                gridView1.Columns["RefNo"].Caption = "المرجع";
                gridView1.Columns["InQty"].Caption = "وارد";
                gridView1.Columns["OutQty"].Caption = "صادر";
                gridView1.Columns["RunningBalance"].Caption = "الرصيد";
                gridView1.Columns["Notes"].Caption = "ملاحظات";

                gridView1.OptionsBehavior.Editable = false;
                gridView1.OptionsView.ShowGroupPanel = false;
                gridView1.BestFitColumns();
            }
        }
        private decimal GetProductBalance(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
        SELECT ISNULL(SUM(Quantity),0)
        FROM InventoryTransactions
        WHERE ProductId = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", productId);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        private void cbxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxProduct.SelectedValue == null)
                return;

            if (!int.TryParse(cbxProduct.SelectedValue.ToString(), out int productId))
                return;

            decimal balance = GetProductBalance(productId);

            lblBalancelabel1.Text = balance.ToString("N3") + " kg";
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

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            string transType = gridView1.GetFocusedRowCellValue("TransName")?.ToString();
            string refNo = gridView1.GetFocusedRowCellValue("RefNo")?.ToString();

            if (string.IsNullOrEmpty(refNo))
                return;

            // 🟢 إذا مشتريات
            if (transType == "مشتريات")
            {
                OpenBuyInvoice(refNo);
            }
            // 🟢 إذا مبيعات
            else if (transType == "مبيعات")
            {
                OpenSaleInvoice(refNo);
            }
        }
        private void OpenBuyInvoice(string invoiceNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT BuyInvoiceId FROM BuyInvoices WHERE InvoiceNumber=@no", con);

                cmd.Parameters.AddWithValue("@no", invoiceNumber);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    int id = Convert.ToInt32(result);

                    frm_BuyInvoiceEditor frm = new frm_BuyInvoiceEditor(id);
                    frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على الفاتورة");
                }
            }
        }
        private void OpenSaleInvoice(string invoiceNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT InvoiceId FROM Invoices WHERE InvoiceNumber=@no", con);

                cmd.Parameters.AddWithValue("@no", invoiceNumber);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    int id = Convert.ToInt32(result);

                    frm_NewInvoice frm = new frm_NewInvoice(id);
                    frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على الفاتورة");
                }
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
            rptProductLedger rpt = new rptProductLedger();
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
   