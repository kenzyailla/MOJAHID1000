using Accounting.Core.Common;
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
using System.Windows.Controls;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    
    public partial class frm_BuyInvoiceEditor : Form
    {
        private SupplierService supplierService;
       

        private string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
  int? _invoiceId = null;
        RepositoryItemGridLookUpEdit repoProducts;

        bool isUpdatingGrid = false;
        private ProductService productService;
        private BuyInvoiceService service;

        private DataTable dtProducts;

        public frm_BuyInvoiceEditor(int? invoiceId = null)
        {
            InitializeComponent();
            string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

            service = new BuyInvoiceService(connectionString);
            supplierService = new SupplierService(connectionString);
            _invoiceId = invoiceId;

            this.Controls.Add(this.gridItems);
            this.Controls.Add(this.panel1);
            if (_invoiceId.HasValue)
            {
                LoadInvoice(_invoiceId.Value);
            }
        }
        public frm_BuyInvoiceEditor()
        {
            InitializeComponent();
            supplierService = new SupplierService(connectionString); // 🔥 هذا السطر مهم
        }
        private void LoadSuppliers()
        {
            cbxSupplier.DataSource = supplierService.GetAllSuppliers();
            cbxSupplier.DisplayMember = "Name";
            cbxSupplier.ValueMember = "SupplierId";
        }
        private void LoadInvoice(int id)
        {
            DataRow header = service.GetBuyInvoiceHeader(id);

            if (header != null)
            {
                txtInvoiceNumber.Text = header["InvoiceNumber"].ToString();

                dateInvoice.Value = Convert.ToDateTime(header["InvoiceDate"]);

                // هنا ضع كود PaymentStatus
                if (header["PaymentStatus"] != DBNull.Value)
                {
                    cbxPaymentStatus.SelectedValue =
                        Convert.ToInt32(header["PaymentStatus"]);
                }
            }
            // ⭐⭐⭐ تحميل تفاصيل الفاتورة ⭐⭐⭐

            DataTable dtLines = service.GetBuyInvoiceLines(id);

            gridItems.DataSource = dtLines;
            gridViewItems.RefreshData();



        }

        private void frm_BuyInvoiceEditor_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            LoadPaymentStatus();

            PrepareGrid();

            productService = new ProductService(connectionString); // 🔥 الحل
                                                                   // 1️⃣ جهّز الجريد أولاً
            PrepareProductLookup();     // 2️⃣ اربط الـ Repository قبل تحميل البيانات

            if (_invoiceId != null)
            {
                LoadInvoice(_invoiceId.Value);   // 3️⃣ الآن حمّل البيانات
                gridViewItems.RefreshData();     // 4️⃣ أعد الرسم بعد الربط
            }

            gridViewItems.CellValueChanged += gridViewItems_CellValueChanged;

            gridViewItems.OptionsBehavior.EditorShowMode =
                DevExpress.Utils.EditorShowMode.Click;

            this.Padding = new Padding(0);
            this.Margin = new Padding(0);
            this.AutoScaleMode = AutoScaleMode.None;



            gridItems.Dock = DockStyle.Fill;
            gridItems.Margin = new Padding(0);
            gridItems.Padding = new Padding(0);

            dateInvoice.Format = DateTimePickerFormat.Custom;
            dateInvoice.CustomFormat = "dd/MM/yyyy";

            dateDue.Format = DateTimePickerFormat.Custom;
            dateDue.CustomFormat = "dd/MM/yyyy";

            SupplierService supplierService = new SupplierService(connectionString);

            var suppliers = supplierService.GetAllSuppliers();

          
            searchLookUpEdit1.Properties.DataSource = suppliers;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "SupplierId";

            searchLookUpEdit1.Properties.NullText = "";

            // 🔥 تنظيف الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم المورد");

            // 🔥 تحسين العرض
            searchLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit1.Properties.View.BestFitColumns();

            // 🔥 الخط
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Regular);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            // 🔥 البحث
            searchLookUpEdit1.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            searchLookUpEdit1.Properties.ImmediatePopup = true;

            // 🔥 حجم الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;



            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;

            cbxPaymentType.DataSource = new List<object>
{
    new { Id = 1, Name = "Cash" },
    new { Id = 2, Name = "Bank" },
    new { Id = 3, Name = "OnAccount" }
};

            cbxPaymentType.DisplayMember = "Name";
            cbxPaymentType.ValueMember = "Id";
            cbxPaymentType.SelectedIndex = 0;

        }
        private void LoadPaymentStatus()
        {
            

            cbxPaymentStatus.DataSource = new List<dynamic>
            {
                new { Id = 1, Name = "غير مدفوع" },
                new { Id = 2, Name = "مدفوع جزئي" },
                new { Id = 3, Name = "مدفوع بالكامل" }
            };

            cbxPaymentStatus.DisplayMember = "Name";
            cbxPaymentStatus.ValueMember = "Id";
          

        }
        private void PrepareGrid()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ProductId", typeof(int));   // ⭐ هذا الحل
            dt.Columns.Add("ProductName");
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("TaxRate", typeof(decimal));
            dt.Columns.Add("SubTotal", typeof(decimal));
            dt.Columns.Add("TaxAmount", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));

            gridItems.DataSource = dt;

            gridViewItems.OptionsView.NewItemRowPosition =
                DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
        }

        private void CalculateTotals()
        {
            gridViewItems.CloseEditor();
            gridViewItems.UpdateCurrentRow();

            decimal sub = 0;
            decimal tax = 0;

            DataTable dt = gridItems.DataSource as DataTable;

            foreach (DataRow r in dt.Rows)
            {
                decimal rowSub = r["SubTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["SubTotal"]);
                decimal rowTax = r["TaxAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TaxAmount"]);

                sub += rowSub;
                tax += rowTax;
            }

            txtSubTotal.Text = sub.ToString("N2");
            txtTax.Text = tax.ToString("N2");
            txtTotal.Text = (sub + tax).ToString("N2");
        }

       
            //لكي نعيدها كما كانت
      private void btnSave_Click(object sender, EventArgs e)
            {

            btnSave.Enabled = false;
            BuyInvoice inv = new BuyInvoice();

            inv.InvoiceNumber = Convert.ToInt32(txtInvoiceNumber.Text);
            inv.SupplierId = Convert.ToInt32(cbxSupplier.SelectedValue);
            inv.InvoiceDate = dateInvoice.Value;
            inv.DueDate = dateDue.Value;

            inv.PaymentType = Convert.ToInt32(cbxPaymentType.SelectedValue);
            inv.PaymentStatus = (PaymentStatus)(cbxPaymentStatus.SelectedIndex + 1);

            inv.SubTotal = Convert.ToDecimal(txtSubTotal.Text);
            inv.TaxTotal = Convert.ToDecimal(txtTax.Text);
            inv.TotalAfterTax = Convert.ToDecimal(txtTotal.Text);
            inv.Notes = txtNotes.Text;

            //---------------------------------
            // قراءة تفاصيل الجريد
            //---------------------------------
            DataTable dt = gridItems.DataSource as DataTable;

            foreach (DataRow r in dt.Rows)
            {
                inv.Lines.Add(new BuyInvoiceLine
                {
                    ProductId = Convert.ToInt32(r["ProductId"]),
                    Quantity = Convert.ToDecimal(r["Quantity"]),
                    UnitPrice = Convert.ToDecimal(r["UnitPrice"]),
                    TaxRate = Convert.ToDecimal(r["TaxRate"]),
                    LineSubTotal = Convert.ToDecimal(r["SubTotal"]),
                    TaxAmount = Convert.ToDecimal(r["TaxAmount"]),
                    LineTotal = Convert.ToDecimal(r["Total"])
                });
            }

            int invoiceId = 0;

            // 🟢 حفظ الفاتورة مرة واحدة فقط
            if (_invoiceId == null)
            {
                invoiceId = service.AddBuyInvoice(inv);

                // 🔥 1) المخزون (دائماً)
                foreach (var line in inv.Lines)
                {
                    AddInventoryTransaction(
                        line.ProductId,
                        line.Quantity,
                        line.UnitPrice,
                        1, // Purchase
                        invoiceId
                    );
                    // 🔥 تحديث الرصيد الافتتاحي بإضافة الكمية الجديدة
                    //UpdateOpeningStock(line.ProductId, line.Quantity, line.UnitPrice);
                }

                // 🔥 2) الكاش (فقط نقدي)
                if (inv.PaymentType == 1 || inv.PaymentType == 2)
                {
                    AddCashTransaction(
                        inv.InvoiceDate,
                        "فاتورة شراء رقم " + inv.InvoiceNumber + " - " + cbxSupplier.Text,
                        0,
                        inv.TotalAfterTax,
                        "Purchase",
                        invoiceId
                    );
                }
            }
            else
            {
                // 🔵 تعديل
                service.UpdateBuyInvoice(_invoiceId.Value, inv);
                invoiceId = _invoiceId.Value;
            }
            btnSave.Enabled = true;
            MessageBox.Show("تم حفظ الفاتورة");
            AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
            this.DialogResult = DialogResult.OK;
            this.Close();
          

        }




        private void AddInventoryTransaction(int productId, decimal qty, decimal costPrice, int type, int refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
IF NOT EXISTS (
    SELECT 1 FROM InventoryTransactions 
    WHERE ProductId = @p 
    AND ReferenceId = @r 
    AND TransactionType = @t
)
BEGIN
    INSERT INTO InventoryTransactions
    (ProductId, Quantity, CostPrice, TransactionType, ReferenceId, TransactionDate)
    VALUES
    (@p, @q, @c, @t, @r, GETDATE())
END
", con);

                cmd.Parameters.AddWithValue("@p", productId);
                cmd.Parameters.AddWithValue("@q", qty);
                cmd.Parameters.AddWithValue("@c", costPrice);
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@r", refId);

                cmd.ExecuteNonQuery();
            }
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

        //private void PrepareProductLookup()
        //{


        //    RepositoryItemLookUpEdit repoProduct = new RepositoryItemLookUpEdit();
        //    repoProduct.DataSource = dtProducts;
        //    repoProduct.DataSource = productService.GetAllProducts();
        //    repoProduct.DisplayMember = "Name";
        //    repoProduct.ValueMember = "ProductId";

        //    repoProduct.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoSearch;
        //    repoProduct.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        //    repoProduct.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;

        //    repoProduct.NullText = "";
        //    repoProduct.ShowHeader = false;

        //    gridViewItems.Columns["ProductId"].ColumnEdit = repoProduct;


        //}

        private void PrepareProductLookup()
        {
            RepositoryItemGridLookUpEdit repo = new RepositoryItemGridLookUpEdit();

            repo.DataSource = productService.GetAllProducts();
            repo.DisplayMember = "Name";
            repo.ValueMember = "ProductId";

            repo.NullText = "";
            repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            repo.ImmediatePopup = true;

            // 🔥 البحث
            repo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            repo.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;

            // 🔥 الأعمدة (بدون PopulateColumns)
            repo.View.Columns.Clear();
            repo.View.Columns.AddVisible("Name", "اسم المادة");

           

            // 🔥 الخط العربي
            repo.View.Appearance.Row.Font = new Font("Noto Kufi Arabic", 9);
            repo.View.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 9, FontStyle.Bold);
            repo.Appearance.Font = new Font("Noto Kufi Arabic", 10);

            // 🔥 تحسين الشكل
            repo.View.RowHeight = 28;
            repo.View.OptionsView.ShowAutoFilterRow = true;
            repo.View.OptionsView.ColumnAutoWidth = false;

            // 🔥 عرض العمود
            repo.View.Columns["Name"].Width = 250;

            // 🔥 ربطه بالجريد
            gridItems.RepositoryItems.Add(repo);
            gridViewItems.Columns["ProductId"].ColumnEdit = repo;
        }


        private void gridViewItems_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

                   

      
            if (isUpdatingGrid) return;

            try
            {
                isUpdatingGrid = true;

                //-------------------------------------------------
                // ⭐ عند اختيار المنتج فقط
                //-------------------------------------------------
                if (e.Column.FieldName == "ProductId")
                {
                    if (e.Value == null || e.Value == DBNull.Value)
                        return;

                    int productId;

                    if (!int.TryParse(e.Value.ToString(), out productId))
                        return;

                    ProductService ps = new ProductService(connectionString);
                    var p = ps.GetProductById(productId);

                    if (p != null)
                    {
                        gridViewItems.SetRowCellValue(e.RowHandle, "ProductName", p.Name);
                        gridViewItems.SetRowCellValue(e.RowHandle, "UnitPrice", p.Price);
                        gridViewItems.SetRowCellValue(e.RowHandle, "TaxRate", p.TaxRate);
                    }
                }

                //-------------------------------------------------
                // ⭐ الحساب التلقائي فقط عند تغيير القيم المؤثرة
                //-------------------------------------------------
                if (e.Column.FieldName == "Quantity" ||
                    e.Column.FieldName == "UnitPrice" ||
                    e.Column.FieldName == "TaxRate" ||
                    e.Column.FieldName == "ProductId")
                {
                    decimal qty = GetDecimalValue(e.RowHandle, "Quantity");
                    decimal price = GetDecimalValue(e.RowHandle, "UnitPrice");
                    decimal taxRate = GetDecimalValue(e.RowHandle, "TaxRate");

                    decimal sub = qty * price;
                    decimal tax = sub * taxRate / 100;
                    decimal total = sub + tax;

                    gridViewItems.SetRowCellValue(e.RowHandle, "SubTotal", sub);
                    gridViewItems.SetRowCellValue(e.RowHandle, "TaxAmount", tax);
                    gridViewItems.SetRowCellValue(e.RowHandle, "Total", total);

                    CalculateTotals();
                }
            }
            finally
            {
                isUpdatingGrid = false;
            }
        }

        private decimal GetDecimalValue(int rowHandle, string columnName)
        {
            object val = gridViewItems.GetRowCellValue(rowHandle, columnName);

            if (val == null || val == DBNull.Value)
                return 0;

            decimal result;
            return decimal.TryParse(val.ToString(), out result) ? result : 0;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int supplierId))
                return;

            // 🔥 تحديد المورد داخل cbxSupplier بشكل مضمون
            for (int i = 0; i < cbxSupplier.Items.Count; i++)
            {
                var item = cbxSupplier.Items[i];

                if (item is DataRowView row)
                {
                    if (Convert.ToInt32(row["SupplierId"]) == supplierId)
                    {
                        cbxSupplier.SelectedIndex = i;
                        break;
                    }
                }
                else if (item is Supplier s)
                {
                    if (s.SupplierId == supplierId)
                    {
                        cbxSupplier.SelectedIndex = i;
                        break;
                    }
                }
            }


        }
        private void AddCashTransaction(DateTime date, string desc, decimal debit, decimal credit, string refType, int refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmdBalance = new SqlCommand(
                    "SELECT TOP 1 Balance FROM CashTransactions ORDER BY CashId DESC", con);

                object lastBalanceObj = cmdBalance.ExecuteScalar();

                decimal lastBalance = lastBalanceObj != null ? Convert.ToDecimal(lastBalanceObj) : 0;

                decimal newBalance = lastBalance + debit - credit;

                SqlCommand cmd = new SqlCommand(@"
INSERT INTO CashTransactions
(TransDate, Description, Debit, Credit, Balance, RefType, RefId)
VALUES
(@date, @desc, @debit, @credit, @balance, @type, @ref)", con);

                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@debit", debit);
                cmd.Parameters.AddWithValue("@credit", credit);
                cmd.Parameters.AddWithValue("@balance", newBalance);
                cmd.Parameters.AddWithValue("@type", refType);
                cmd.Parameters.AddWithValue("@ref", refId);

                cmd.ExecuteNonQuery();
            }
        }
        // أضف هذه الدالة داخل الفئة frm_BuyInvoiceEditor
      
    }
}
