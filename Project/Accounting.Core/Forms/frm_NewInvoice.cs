using Accounting.Core.Enums;
using Accounting.Core.Models;
using Accounting.Core.Reportingcrystal;
using Accounting.Core.Services;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Accounting.Core.Forms
{
  
    public partial class frm_NewInvoice : Form
    {
        private bool _isSaving = false;
        private CustomerService cs;
        private InvoiceService service;
        private InvoiceService _service;
        private int _invoiceTypeValue = 0;
        string connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private int? _invoiceId = null;

        public frm_NewInvoice()
        {
            InitializeComponent();
            _service = new InvoiceService(connectionString);
        }

        public frm_NewInvoice(int invoiceId)
        {
            InitializeComponent();
            _invoiceId = invoiceId;
            _service = new InvoiceService(connectionString);
        }
     
        bool isSelecting = false;
        bool isCalculating = false;
        private RepositoryItemGridLookUpEdit repoProducts;
        private bool isUpdatingGrid = false;
   
        private void frm_NewInvoice_Load(object sender, EventArgs e)
        {
           

            dateEdit1.Format = DateTimePickerFormat.Custom;
            dateEdit1.CustomFormat = "dd/MM/yyyy";
            // =========================
            // 🧾 تجهيز جدول الجريد
            // =========================
            DataTable dt = new DataTable();

            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("TaxRate", typeof(decimal));
            dt.Columns.Add("TotalBeforeTax", typeof(decimal));
            dt.Columns.Add("TotalTax", typeof(decimal));
            dt.Columns.Add("TotalAfterTax", typeof(decimal));

            gridControlLines.DataSource = dt;
            GridView.PopulateColumns();

            // =========================
            // 🧱 إعداد الجريد
            // =========================
            PrepareProductLookup();

            GridView.CellValueChanged += GridView_CellValueChanged;

            GridView.OptionsView.NewItemRowPosition =
                DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;

            GridView.Columns["ProductId"].Caption = "المنتج";
            GridView.Columns["Quantity"].Caption = "الكمية";
            GridView.Columns["UnitPrice"].Caption = "السعر";
            GridView.Columns["Discount"].Caption = "الخصم";
            GridView.Columns["TaxRate"].Caption = "الضريبة";
            GridView.Columns["TotalBeforeTax"].Caption = "قبل الضريبة";
            GridView.Columns["TotalTax"].Caption = "قيمة الضريبة";
            GridView.Columns["TotalAfterTax"].Caption = "الإجمالي";

            GridView.Columns["TotalBeforeTax"].OptionsColumn.AllowEdit = false;
            GridView.Columns["TotalTax"].OptionsColumn.AllowEdit = false;
            GridView.Columns["TotalAfterTax"].OptionsColumn.AllowEdit = false;
        
           

            // =========================
            // 📄 نوع الفاتورة (مهم جدًا)
            // =========================
            cbxInvoiceTypee.DataSource = new List<ComboItem>
{
    new ComboItem { Id = (int)InvoiceType.Sales, Name = "بيع" },
    new ComboItem { Id = (int)InvoiceType.Purchase, Name = "شراء" },
    new ComboItem { Id = (int)InvoiceType.SalesReturn, Name = "مرتجع بيع" },
    new ComboItem { Id = (int)InvoiceType.PurchaseReturn, Name = "مرتجع شراء" },
    new ComboItem { Id = (int)InvoiceType.ExemptSales, Name = "بيع معفى" }
};

            cbxInvoiceTypee.DisplayMember = "Name";
            cbxInvoiceTypee.ValueMember = "Id";
            cbxInvoiceTypee.SelectedIndex = 0;

            // =========================
            // 💳 طريقة الدفع
            // =========================
            cbxPaymentType.DataSource = new List<object>
    {
        new { Id = 1, Name = "Cash" },
        new { Id = 2, Name = "Bank" },
        new { Id = 3, Name = "OnAccount" }
    };

            cbxPaymentType.DisplayMember = "Name";
            cbxPaymentType.ValueMember = "Id";
            cbxPaymentType.SelectedIndex = 0;


            CustomerService cs = new CustomerService(connectionString);

            // ================= العملاء =================
            var customers = cs.GetAllCustomers();

            searchLookUpEdit1.Properties.DataSource = customers;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "CustomerId";

            searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            searchLookUpEdit1.Properties.View.PopulateColumns();
            // 🔥 إخفاء كل الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();

            // 🔥 عرض اسم العميل فقط
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم العميل");
            // الخط داخل القائمة
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Regular);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            // الخط داخل مربع الإدخال
            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            // محاذاة
            searchLookUpEdit1.Properties.Appearance.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Near;

            // ارتفاع الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;

            txtInvoiceNumber.Text = GetNextInvoiceNumber();


            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;

            cs = new CustomerService(connectionString);

           
            cbxCustomer.DropDownStyle = ComboBoxStyle.DropDownList;

            cbxCustomer.DataSource = customers;
            cbxCustomer.DisplayMember = "Name";
            cbxCustomer.ValueMember = "CustomerId";
            cbxCustomer.SelectedIndex = -1;

            // =========================
            // ✏️ تحميل فاتورة للتعديل
            // =========================
            if (_invoiceId != null)
            {
                LoadInvoiceData(_invoiceId.Value);
             
            }
            CustomizeGridView(GridView);

        }

        private void LoadInvoiceData(int invoiceId)
        {
            DataRow inv = _service.GetInvoiceHeader(invoiceId);
            if (inv == null) return;

            txtInvoiceNumber.Text = inv["InvoiceNumber"].ToString();
            dateEdit1.Value = Convert.ToDateTime(inv["InvoiceDate"]);

            cbxCustomer.SelectedValue = Convert.ToInt32(inv["CustomerId"]);
            cbxPaymentType.SelectedValue = Convert.ToInt32(inv["PaymentType"]);

            txtSubTotal.Text = inv["TotalBeforeTax"].ToString();
            txt_tax.Text = inv["TotalTax"].ToString();
            txt_Total.Text = inv["TotalAfterTax"].ToString();

            gridControlLines.DataSource = _service.GetInvoiceLines(invoiceId);

            // 🔥 تحديد نوع الفاتورة بشكل صحيح
            int invType = Convert.ToInt32(inv["InvoiceType"]);

            for (int i = 0; i < cbxInvoiceTypee.Items.Count; i++)
            {
                var item = (ComboItem)cbxInvoiceTypee.Items[i];

                if (item.Id == invType)
                {
                    cbxInvoiceTypee.SelectedIndex = i;
                    break;
                }
            }
        }

        //private void PrepareProductLookup()
        //{


        //    RepositoryItemGridLookUpEdit repo = new RepositoryItemGridLookUpEdit();

        //    ProductService ps = new ProductService(connectionString);

        //    repo.DataSource = ps.GetAllProducts();
        //    repo.DisplayMember = "Name";
        //    repo.ValueMember = "ProductId";

        //    repo.NullText = "";
        //    repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        //    repo.ImmediatePopup = true;

        //    // 🔥 البحث بالاسم فقط
        //    repo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
        //    repo.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;

        //    // 🔥 مهم جداً: تجهيز الأعمدة
        //    repo.PopulateViewColumns();

        //    // 🔥 إخفاء ID
        //    repo.View.Columns["ProductId"].Visible = false;

        //    // 🔥 تأكد أن البحث يتم على الاسم فقط
        //    repo.View.Columns["Name"].Caption = "اسم المنتج";
        //    repo.View.Columns["Name"].OptionsFilter.AutoFilterCondition =
        //        DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;

        //    // 🔥 منع البحث في الأعمدة الأخرى
        //    foreach (DevExpress.XtraGrid.Columns.GridColumn col in repo.View.Columns)
        //    {
        //        if (col.FieldName != "Name")
        //            col.OptionsFilter.AllowFilter = false;
        //    }

        //    //repo.View.BestFitColumns();
        //    // 🔥 تكبير عمود الاسم
        //    repo.View.Columns["Name"].Width = 300; // جرب 300 أو 400

        //    // 🔥 جعل الأعمدة لا تتضغط تلقائياً
        //    repo.View.OptionsView.ColumnAutoWidth = false;
        //    gridControlLines.RepositoryItems.Add(repo);

        //    GridView.Columns["ProductId"].ColumnEdit = repo;
        //}

        private void PrepareProductLookup()
        {
            RepositoryItemGridLookUpEdit repo = new RepositoryItemGridLookUpEdit();

            ProductService ps = new ProductService(connectionString);

            repo.DataSource = ps.GetAllProducts();
            repo.DisplayMember = "Name";
            repo.ValueMember = "ProductId";

            repo.NullText = "";
            repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            repo.ImmediatePopup = true;

            // 🔥 البحث
            repo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            repo.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;

            // 🔥 لا تجلب كل الأعمدة
            repo.View.Columns.Clear();

            // 🔥 أظهر اسم المادة فقط
            repo.View.Columns.AddVisible("Name", "اسم المادة");

            // 🔥 خط عربي
            repo.View.Appearance.Row.Font = new Font("Noto Kufi Arabic", 9, FontStyle.Regular);
            repo.View.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 9, FontStyle.Bold);
            repo.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            // 🔥 تحسين الشكل
            repo.View.RowHeight = 28;
            repo.View.OptionsView.ShowAutoFilterRow = true;
            repo.View.OptionsView.ColumnAutoWidth = false;

            // 🔥 تكبير عمود الاسم
            repo.View.Columns["Name"].Width = 300;

            gridControlLines.RepositoryItems.Add(repo);
            GridView.Columns["ProductId"].ColumnEdit = repo;
        }
        private void LoadInvoice(int invoiceId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmdHeader = new SqlCommand(@"
            SELECT 
            InvoiceNumber,
            CustomerId,
            InvoiceType,
            PaymentType,
            InvoiceDate,
            TotalBeforeTax,
            TotalTax,
            TotalAfterTax
            FROM Invoices
            WHERE InvoiceId=@Id", con);

                    cmdHeader.Parameters.AddWithValue("@Id", invoiceId);

                    SqlDataReader dr = cmdHeader.ExecuteReader();

                    if (dr.Read())
                    {
                        txtInvoiceNumber.Text = dr["InvoiceNumber"].ToString();
                        cbxCustomer.SelectedValue = dr["CustomerId"];
                        cbxPaymentType.SelectedValue = dr["InvoiceType"];
                        cbxInvoiceTypee.SelectedValue = dr["PaymentType"];
                        dateEdit1.Value = Convert.ToDateTime(dr["InvoiceDate"]);
                        txtSubTotal.Text = dr["TotalBeforeTax"].ToString();
                        txt_tax.Text = dr["TotalTax"].ToString();
                        txt_Total.Text = dr["TotalAfterTax"].ToString();
                    }

                    dr.Close();

                    SqlCommand cmdLines = new SqlCommand(@"
            SELECT 
                ProductId,
                Quantity,
                UnitPrice,
                Discount,
                TaxRate,
                TotalBeforeTax,
                TotalTax,
                TotalAfterTax
            FROM InvoiceLines
            WHERE InvoiceId=@Id", con);

                    cmdLines.Parameters.AddWithValue("@Id", invoiceId);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmdLines);
                    da.Fill(dt);

                    gridControlLines.DataSource = dt;

                    GridView.RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void GridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            

            if (isUpdatingGrid) return;

            try
            {
                isUpdatingGrid = true;

                // عند اختيار المنتج
                if (e.Column.FieldName == "ProductId")
                {
                    if (e.Value == null || e.Value == DBNull.Value) return;

                    int productId = Convert.ToInt32(e.Value);

                    ProductService ps = new ProductService(connectionString);
                    var product = ps.GetProductById(productId);

                    if (product != null)
                    {
                        GridView.SetRowCellValue(e.RowHandle, "UnitPrice", product.Price);

                        decimal taxRate = product.TaxRate;

                        if (IsExemptInvoice())
                            taxRate = 0;

                        GridView.SetRowCellValue(e.RowHandle, "TaxRate", taxRate);
                    }
                }

                CalculateRow(e.RowHandle);
            }
            finally
            {
                isUpdatingGrid = false;

            }
        }
        private decimal GetDecimalValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToDecimal(value);
        }


        private void CalculateRow(int rowHandle)
        {
            if (isCalculating)
                return;

            isCalculating = true;

            decimal qty = GetDecimalValue(GridView.GetRowCellValue(rowHandle, "Quantity"));
            decimal price = GetDecimalValue(GridView.GetRowCellValue(rowHandle, "UnitPrice"));
            decimal discount = GetDecimalValue(GridView.GetRowCellValue(rowHandle, "Discount"));
            //decimal taxRate = GetDecimalValue(GridView.GetRowCellValue(rowHandle, "TaxRate"));
            decimal taxRate = GetDecimalValue(GridView.GetRowCellValue(rowHandle, "TaxRate"));

            if (IsExemptInvoice())
            {
                taxRate = 0;
                GridView.SetRowCellValue(rowHandle, "TaxRate", 0);
            }

            decimal beforeTax = (qty * price) - discount;
            decimal tax = beforeTax * taxRate / 100;
            decimal afterTax = beforeTax + tax;

            GridView.SetRowCellValue(rowHandle, "TotalBeforeTax", beforeTax);
            GridView.SetRowCellValue(rowHandle, "TotalTax", tax);
            GridView.SetRowCellValue(rowHandle, "TotalAfterTax", afterTax);

            RecalculateTotals();

            isCalculating = false;
        }
        private void RecalculateTotals()
        {
            GridView.PostEditor();
            GridView.UpdateCurrentRow();


            decimal subTotal = 0;
            decimal taxTotal = 0;
            decimal grandTotal = 0;

            DataTable dt = gridControlLines.DataSource as DataTable;

            if (dt == null) return;

            foreach (DataRow row in dt.Rows)
            {
                subTotal += row["TotalBeforeTax"] == DBNull.Value
                    ? 0 : Convert.ToDecimal(row["TotalBeforeTax"]);

                taxTotal += row["TotalTax"] == DBNull.Value
                    ? 0 : Convert.ToDecimal(row["TotalTax"]);

                grandTotal += row["TotalAfterTax"] == DBNull.Value
                    ? 0 : Convert.ToDecimal(row["TotalAfterTax"]);
            }

            txtSubTotal.Text = subTotal.ToString("N2");
            txt_tax.Text = taxTotal.ToString("N2");
            txt_Total.Text = grandTotal.ToString("N2");
        }

        private void RepoProduct_EditValueChanged(object sender, EventArgs e)
        {
            GridView.PostEditor();

            int rowHandle = GridView.FocusedRowHandle;

            int productId = Convert.ToInt32(GridView.GetRowCellValue(rowHandle, "ProductId"));

            ProductService ps = new ProductService(connectionString);

            var product = ps.GetAllProducts()
                            .FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                GridView.SetRowCellValue(rowHandle, "UnitPrice", product.Price);
                GridView.SetRowCellValue(rowHandle, "TaxRate", product.TaxRate);
            }

            CalculateRow(rowHandle);
        }
       

        private async void btnSave_Click(object sender, EventArgs e)
        {
            // منع التنفيذ المتزامن
            if (_isSaving) return;
            _isSaving = true;
            // تعطيل الزر مؤقتاً
            btnSave.Enabled = false;
            try
            {
                if (cbxCustomer.SelectedValue == null)
                {
                    MessageBox.Show("اختر العميل أولاً");
                    return;
                }
                // تحقق
                // =========================
                if (cbxInvoiceTypee.SelectedItem == null)
                {
                    MessageBox.Show("اختر نوع الفاتورة");
                    return;
                }

                GridView.CloseEditor();
                GridView.UpdateCurrentRow();

                // =========================
                // إنشاء الفاتورة
                // =========================
                Invoice invoice = new Invoice
                {
                    InvoiceNumber = txtInvoiceNumber.Text,
                    InvoiceDate = dateEdit1.Value,
                    CustomerId = Convert.ToInt32(cbxCustomer.SelectedValue),
                    InvoiceType = ((ComboItem)cbxInvoiceTypee.SelectedItem).Id,
                    PaymentType = Convert.ToInt32(cbxPaymentType.SelectedValue),
                    TotalBeforeTax = Convert.ToDecimal(txtSubTotal.Text),
                    TotalTax = IsExemptInvoice() ? 0 : Convert.ToDecimal(txt_tax.Text),
                    TotalAfterTax = Convert.ToDecimal(txt_Total.Text),
                    Lines = new List<InvoiceLine>()
                };

                // =========================
                // إضافة السطور
                // =========================
                for (int i = 0; i < GridView.RowCount; i++)
                {
                    if (GridView.IsNewItemRow(i))
                        continue;

                    object productValue = GridView.GetRowCellValue(i, "ProductId");
                    if (productValue == null || productValue == DBNull.Value)
                        continue;

                    decimal taxRate = Convert.ToDecimal(GridView.GetRowCellValue(i, "TaxRate"));

                    if (IsExemptInvoice())
                        taxRate = 0;

                    invoice.Lines.Add(new InvoiceLine
                    {
                        ProductId = Convert.ToInt32(productValue),
                        Quantity = Convert.ToDecimal(GridView.GetRowCellValue(i, "Quantity")),
                        UnitPrice = Convert.ToDecimal(GridView.GetRowCellValue(i, "UnitPrice")),
                        Discount = 0,
                        TaxRate = taxRate
                    });
                }

                // =========================
                // الحفظ
                // =========================
                int invoiceId = await _service.AddInvoice(invoice);

                // =================================================
                // 🔥 إضافة حركات المخزون وتحديث الرصيد الافتتاحي
                // =================================================
                foreach (var line in invoice.Lines)
                {
                    // 1) تسجيل حركة بيع (TransactionType = 2) بكمية سالبة
                    AddInventoryTransaction(line.ProductId, -line.Quantity, line.UnitPrice, 2, invoiceId);

                    // 2) تحديث الرصيد الافتتاحي بطرح الكمية المباعة
                    //UpdateOpeningStockForSale(line.ProductId, line.Quantity);
                }




                // 🔥 فقط إذا نقدي
                if (invoice.PaymentType == 1)
                {
                    AddCashTransaction(
                        invoice.InvoiceDate,
                        "فاتورة بيع رقم " + invoice.InvoiceNumber,
                        invoice.TotalAfterTax,
                        0,
                        "Invoice",
                        invoiceId
                    );
                }

                MessageBox.Show("تم حفظ الفاتورة رقم: " + invoiceId);

                // 🔥 توليد QR
                string qrText = GenerateQRText(invoiceId); // سنعملها
                string qrPath = GenerateQRCodeImage(qrText, invoiceId);

                // 🔥 حفظ المسار في DB
                SaveQRPath(invoiceId, qrPath);
                AppEvents.RefreshDashboard();
                // =========================
                // 🔥 إغلاق الشاشة (احترافي)
                // =========================
                this.DialogResult = DialogResult.OK;
                this.Close();
                //========================================================================جديد
             

                //// 🔥 توليد QR
                //string qrText = GenerateQRText(invoiceId); // سنعملها
                //string qrPath = GenerateQRCodeImage(qrText, invoiceId);

                //// 🔥 حفظ المسار في DB
                //SaveQRPath(invoiceId, qrPath);
                //=====================================================================



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btnSave.Enabled = true;
            _isSaving = false;

            AppEvents.RefreshDashboard();
        }
        private string GenerateQRCodeImage(string text, int invoiceId)
        {
            string folder = @"C:\InvoicesImages";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, "QR_" + invoiceId + ".png");

            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCoder.QRCode(qrData);

                using (Bitmap qrImage = qrCode.GetGraphic(20))
                {
                    qrImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            return filePath;
        }
        private string GenerateQRText(int invoiceId)
        {
            return "InvoiceID: " + invoiceId;
        }
        private void SaveQRPath(int invoiceId, string path)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
UPDATE Invoices
SET QRPath = @p
WHERE InvoiceId = @id", con);

                cmd.Parameters.AddWithValue("@p", path);
                cmd.Parameters.AddWithValue("@id", invoiceId);

                cmd.ExecuteNonQuery();
            }
        }
     
        //===================================================================هنا نهاية الاضافات السابقة
        private void PrintInvoice(int invoiceId)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("rpt_PrintInvoice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        da.Fill(dt);
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للطباعة");
                    return;
                }

                // جلب مسار QR من قاعدة البيانات
                string qrPath = GetQRPathFromDatabase(invoiceId);

                rptInvoice rpt = new rptInvoice();

                rpt.SetDataSource(dt);

                // تمرير مسار QR للتقرير
                rpt.SetParameterValue("QRCodeImagePath", qrPath);

                frm_ReportViewer frm = new frm_ReportViewer(rpt);

                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private decimal GetDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToDecimal(value);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private string GetQRPathFromDatabase(int invoiceId)
        {
            string qrPath = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT QRCodePath FROM Invoices WHERE InvoiceId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", invoiceId);

                qrPath = cmd.ExecuteScalar()?.ToString();
            }

            return qrPath;
        }

        private void cbxInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }
        private bool IsExemptInvoice()
        {
            if (cbxInvoiceTypee.SelectedItem == null)
                return false;
            return ((ComboItem)cbxInvoiceTypee.SelectedItem).Id == (int)InvoiceType.ExemptSales;

        }

        private void cbxInvoiceTypee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gridControlLines.DataSource == null)
                return;

            for (int i = 0; i < GridView.RowCount; i++)
            {
                if (GridView.IsNewItemRow(i))
                    continue;

                if (IsExemptInvoice())
                    GridView.SetRowCellValue(i, "TaxRate", 0);

                CalculateRow(i);
            }

            RecalculateTotals();
        }

        private void cbxCustomer_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int customerId))
                return;

            // 🔥 ربط العميل فقط
            cbxCustomer.SelectedValue = customerId;
        }
        private string GetNextInvoiceNumber()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(InvoiceNumber AS INT)), 2910) + 1
FROM Invoices", con);

                int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());

                return nextNumber.ToString();
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

        // تسجيل حركة مخزون (نوع البيع = 2)
        private void AddInventoryTransaction(int productId, decimal qty, decimal unitPrice, int type, int refId)
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
                    cmd.Parameters.AddWithValue("@r", refId);
                    cmd.ExecuteNonQuery();
                }
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

    }
}
