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

    public partial class frm_BuyReturn : Form
    {
        private int? _buyReturnId;
        public frm_BuyReturn()
        {
            InitializeComponent();
            service = new InvoiceService(connectionString);
        }
        public frm_BuyReturn(int buyReturnId) : this() // يستدعي المنشئ الافتراضي أولاً
        {
            _buyReturnId = buyReturnId;
            this.Text = "مرتجع رقم: " + buyReturnId; // عرض الرقم في شريط العنوان
            LoadBuyReturnData();
        }
        private void LoadBuyReturnData()
        {
            if (_buyReturnId == null) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT
    l.BuyReturnId,
    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TaxRate,
    l.LineBeforeTax,
    l.LineTax,
    l.LineAfterTax
FROM BuyReturnLines l
LEFT JOIN Products p
    ON l.ProductId = p.ProductId
WHERE l.BuyReturnId = @Id
ORDER BY l.ProductId", con);

                da.SelectCommand.Parameters.AddWithValue("@Id", _buyReturnId.Value);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = null;
                gridControl1.DataSource = dt;

                gridView1.Columns.Clear();
                gridView1.PopulateColumns();
                            
                gridView1.Columns["ProductName"].Caption = "اسم المادة";
                gridView1.Columns["Quantity"].Caption = "الكمية";
                gridView1.Columns["UnitPrice"].Caption = "السعر";
                gridView1.Columns["TaxRate"].Caption = "الضريبة";
                gridView1.Columns["LineBeforeTax"].Caption = "قبل الضريبة";
                gridView1.Columns["LineTax"].Caption = "قيمة الضريبة";
                gridView1.Columns["LineAfterTax"].Caption = "الإجمالي";

                // إخفاء رقم المنتج
                gridView1.Columns["ProductId"].Visible = false;

                gridView1.BestFitColumns();

                gridView1.RefreshData();
            }
        }

        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        InvoiceService service;
        private void frm_BuyReturn_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // استعلام بسيط: رقم الفاتورة فقط (بدون اسم المورد)
                string sql = @"
            SELECT BuyInvoiceId, InvoiceNumber
            FROM BuyInvoices
            ORDER BY BuyInvoiceId DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxBuyInvoice.DataSource = dt;
                cbxBuyInvoice.DisplayMember = "InvoiceNumber";   // عرض رقم الفاتورة فقط
                cbxBuyInvoice.ValueMember = "BuyInvoiceId";      // القيمة المستخدمة في الكود
            }

            CustomizeGridView(gridView1);

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cbxBuyInvoice.SelectedValue == null)
                return;

            int invoiceId = Convert.ToInt32(cbxBuyInvoice.SelectedValue);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TaxRate
FROM BuyInvoiceLines l
INNER JOIN Products p ON l.ProductId = p.ProductId
WHERE l.BuyInvoiceId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                dt.Columns.Add("ReturnQty", typeof(decimal));

                gridControl1.DataSource = dt;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbxBuyInvoice.SelectedValue == null)
                return;

            int invoiceId = Convert.ToInt32(cbxBuyInvoice.SelectedValue);

            var returnDict = new Dictionary<int, decimal>();

            DataTable dt = (DataTable)gridControl1.DataSource;

            foreach (DataRow row in dt.Rows)
            {
                if (row["ReturnQty"] == DBNull.Value)
                    continue;

                decimal qty = Convert.ToDecimal(row["ReturnQty"]);
                if (qty <= 0)
                    continue;

                int productId = Convert.ToInt32(row["ProductId"]);

                returnDict.Add(productId, qty);
            }

            if (returnDict.Count == 0)
            {
                MessageBox.Show("لم يتم إدخال كميات مرتجعة");
                return;
            }

            service.CreateBuyReturn(
                invoiceId,
                returnDict
              
            );

            MessageBox.Show("تم حفظ مرتجع المشتريات بنجاح");
            AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
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

        private void cbxBuyInvoice_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbxBuyInvoice.SelectedValue == null)
                return;

            int invoiceId = Convert.ToInt32(cbxBuyInvoice.SelectedValue);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TaxRate
FROM BuyInvoiceLines l
INNER JOIN Products p ON l.ProductId = p.ProductId
WHERE l.BuyInvoiceId = @Id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                dt.Columns.Add("ReturnQty", typeof(decimal));

                gridControl1.DataSource = dt;
            }
        }

        private void txtNotes_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
