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
using DevExpress.XtraEditors.Repository;

namespace Accounting.Core.Forms
{
   

    public partial class frm_InvoiceEditor : Form
    {
        public frm_InvoiceEditor(int invoiceId)
        {
            InitializeComponent();
            _invoiceId = invoiceId;
        }
        private int _invoiceId;

        string connectionString =
        @"Data Source=.\SQLEXPRESS;
  Initial Catalog=AccountingCoreDB;
  Integrated Security=True";
        private int _originalInvoiceType;
        private int _originalPaymentType; // إذا احتجت PaymentType أيضًا
        private void frm_InvoiceEditor_Load(object sender, EventArgs e)
        {
            LoadInvoice();
            CustomerService cs = new CustomerService(connectionString);

            cbxCustomer.DataSource = cs.GetAllCustomers();
            cbxCustomer.DisplayMember = "Name";
            cbxCustomer.ValueMember = "CustomerId";
            CustomizeGridView(gridView1);
        }
       

        private void LoadInvoice()
        {
            gridView1.CellValueChanged += gridView1_CellValueChanged;

            InvoiceService service = new InvoiceService(connectionString);
            CustomerService cs = new CustomerService(connectionString);

            // تحميل العملاء
            cbxCustomer.DataSource = cs.GetAllCustomers();
            cbxCustomer.DisplayMember = "Name";
            cbxCustomer.ValueMember = "CustomerId";

            DataRow header = service.GetInvoiceHeader(_invoiceId);

            if (header != null)
            {
                txtInvoiceNumber.Text = header["InvoiceNumber"].ToString();
                dtInvoiceDate.DateTime = Convert.ToDateTime(header["InvoiceDate"]);
                txtTotal.Text = header["TotalAfterTax"].ToString();
                cbxCustomer.SelectedValue = header["CustomerId"];

                // ⭐ حفظ نوع الفاتورة وطريقة الدفع
                _originalInvoiceType = Convert.ToInt32(header["InvoiceType"]);
                _originalPaymentType = Convert.ToInt32(header["PaymentType"]);
            }

            DataTable dtLines = service.GetInvoiceLines(_invoiceId);
            dtLines.Columns["Quantity"].DataType = typeof(decimal);
            dtLines.Columns["UnitPrice"].DataType = typeof(decimal);
            dtLines.Columns["Discount"].DataType = typeof(decimal);
            dtLines.Columns["TaxRate"].DataType = typeof(decimal);
            dtLines.Columns["TotalBeforeTax"].DataType = typeof(decimal);
            dtLines.Columns["TotalTax"].DataType = typeof(decimal);
            dtLines.Columns["TotalAfterTax"].DataType = typeof(decimal);

            gridControlLines.DataSource = dtLines;
            gridView1.RefreshData();

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if (gridView1.IsNewItemRow(i))
                    continue;

                CalculateRow(i);
            }

            RecalculateTotals();
            gridView1.PopulateColumns();
            gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;

            gridView1.ValidateRow += gridView1_ValidateRow;
            gridView1.OptionsBehavior.Editable = true;

            PrepareProductLookup(); // ⭐ هذا السطر مهم جداً
        }
        private void gridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            gridView1.SetRowCellValue(e.RowHandle, "Quantity", 1m);
            gridView1.SetRowCellValue(e.RowHandle, "Discount", 0m);
            gridView1.SetRowCellValue(e.RowHandle, "TaxRate", 0m);
            gridView1.SetRowCellValue(e.RowHandle, "UnitPrice", 0m);
        }



        private void gridView1_ValidateRow(object sender,
    DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            int rowHandle = e.RowHandle;

            decimal qty = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "Quantity"));
            decimal price = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "UnitPrice"));
            decimal discount = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "Discount"));
            decimal taxRate = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "TaxRate"));

            decimal beforeTax = (qty * price) - discount;
            decimal tax = beforeTax * taxRate / 100;
            decimal afterTax = beforeTax + tax;

            gridView1.SetRowCellValue(rowHandle, "TotalBeforeTax", beforeTax);
            gridView1.SetRowCellValue(rowHandle, "TotalTax", tax);
            gridView1.SetRowCellValue(rowHandle, "TotalAfterTax", afterTax);

            RecalculateTotals();
        }


        private void btnSave_Click_1(object sender, EventArgs e)
        {
           

            RecalculateRows();

            InvoiceService service = new InvoiceService(connectionString);
            Invoice invoice = new Invoice();

            // تعيين النوع وطريقة الدفع من القيم المحفوظة من الفاتورة الأصلية
            invoice.InvoiceType = _originalInvoiceType;
            invoice.PaymentType = _originalPaymentType;

            invoice.InvoiceNumber = txtInvoiceNumber.Text;
            invoice.InvoiceDate = dtInvoiceDate.DateTime;

            // قراءة العميل من ComboBox
            invoice.CustomerId = Convert.ToInt32(cbxCustomer.SelectedValue);

            // قراءة الأصناف من Grid
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                // تخطي السطر الجديد الفارغ
                if (gridView1.IsNewItemRow(i))
                    continue;

                object productIdVal = gridView1.GetRowCellValue(i, "ProductId");
                if (productIdVal == null || productIdVal == DBNull.Value)
                    continue;

                InvoiceLine line = new InvoiceLine();

                line.ProductId = Convert.ToInt32(productIdVal);
                line.Quantity = Convert.ToDecimal(gridView1.GetRowCellValue(i, "Quantity"));
                line.UnitPrice = Convert.ToDecimal(gridView1.GetRowCellValue(i, "UnitPrice"));
                line.Discount = Convert.ToDecimal(gridView1.GetRowCellValue(i, "Discount"));
                line.TaxRate = Convert.ToDecimal(gridView1.GetRowCellValue(i, "TaxRate"));
                line.TotalBeforeTax = Convert.ToDecimal(gridView1.GetRowCellValue(i, "TotalBeforeTax"));
                line.TotalTax = Convert.ToDecimal(gridView1.GetRowCellValue(i, "TotalTax"));
                line.TotalAfterTax = Convert.ToDecimal(gridView1.GetRowCellValue(i, "TotalAfterTax"));

                invoice.Lines.Add(line);
            }

            // حساب الإجماليات
            invoice.TotalBeforeTax = invoice.Lines.Sum(x => x.TotalBeforeTax);
            invoice.TotalTax = invoice.Lines.Sum(x => x.TotalTax);
            invoice.TotalAfterTax = invoice.Lines.Sum(x => x.TotalAfterTax);

            service.UpdateInvoice(_invoiceId, invoice);

            MessageBox.Show("تم تعديل الفاتورة بنجاح");
            this.Close();
            AppEvents.RefreshDashboard();

        }
        private decimal GetDecimalValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToDecimal(value);
        }


        private bool isUpdating = false;

        public object txt_Tax { get; private set; }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (isUpdating) return;

            try
            {
                isUpdating = true;

                gridView1.PostEditor();
                gridView1.UpdateCurrentRow();

                int rowHandle = e.RowHandle;

                if (rowHandle < 0) return;

                //-------------------------------------------------
                // اختيار منتج
                //-------------------------------------------------
                if (e.Column.FieldName == "ProductId")
                {
                    object val = gridView1.GetRowCellValue(rowHandle, "ProductId");

                    if (val != null && val != DBNull.Value)
                    {
                        int productId = Convert.ToInt32(val);

                        ProductService ps = new ProductService(connectionString);
                        var product = ps.GetProductById(productId);

                        if (product != null)
                        {
                            gridView1.SetRowCellValue(rowHandle, "UnitPrice", product.Price);
                            gridView1.SetRowCellValue(rowHandle, "TaxRate", product.TaxRate);
                        }
                    }
                }

                CalculateRow(rowHandle);

                RecalculateTotals();
            }
            finally
            {
                isUpdating = false;
            }
        }
      

        private void RecalculateRows()
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                decimal qty = Convert.ToDecimal(gridView1.GetRowCellValue(i, "Quantity"));
                decimal price = Convert.ToDecimal(gridView1.GetRowCellValue(i, "UnitPrice"));
                decimal discount = Convert.ToDecimal(gridView1.GetRowCellValue(i, "Discount"));
                decimal taxRate = Convert.ToDecimal(gridView1.GetRowCellValue(i, "TaxRate"));

                decimal beforeTax = (qty * price) - discount;
                decimal tax = beforeTax * taxRate / 100;
                decimal afterTax = beforeTax + tax;

                gridView1.SetRowCellValue(i, "TotalBeforeTax", beforeTax);
                gridView1.SetRowCellValue(i, "TotalTax", tax);
                gridView1.SetRowCellValue(i, "TotalAfterTax", afterTax);
            }
        }
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
            repo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            repo.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;

            repo.PopulateViewColumns();

            gridControlLines.RepositoryItems.Add(repo);

            if (gridView1.Columns["ProductId"] != null)
                gridView1.Columns["ProductId"].ColumnEdit = repo;
        }
        private void RecalculateTotals()
        {
            decimal subTotal = 0;
            decimal taxTotal = 0;
            decimal grandTotal = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                subTotal += GetDecimalValue(
                    gridView1.GetRowCellValue(i, "TotalBeforeTax"));

                taxTotal += GetDecimalValue(
                    gridView1.GetRowCellValue(i, "TotalTax"));

                grandTotal += GetDecimalValue(
                    gridView1.GetRowCellValue(i, "TotalAfterTax"));
            }

            txtSubTotal.Text = subTotal.ToString("N2");
            txt_tax.Text = taxTotal.ToString("N2");
            txtTotal.Text = grandTotal.ToString("N2");
        }

        private void CalculateRow(int rowHandle)
        {
            if (rowHandle < 0) return;

            decimal qty = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "Quantity"));
            decimal price = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "UnitPrice"));
            decimal discount = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "Discount"));
            decimal taxRate = GetDecimalValue(gridView1.GetRowCellValue(rowHandle, "TaxRate"));

            decimal beforeTax = (qty * price) - discount;
            decimal tax = beforeTax * taxRate / 100;
            decimal afterTax = beforeTax + tax;

            gridView1.SetRowCellValue(rowHandle, "TotalBeforeTax", beforeTax);
            gridView1.SetRowCellValue(rowHandle, "TotalTax", tax);
            gridView1.SetRowCellValue(rowHandle, "TotalAfterTax", afterTax);
        }

        private async void btnSendToTax_Click(object sender, EventArgs e)
        {
            try
            {
                int invoiceId = _invoiceId;

                if (IsInvoicePosted(invoiceId))
                {
                    MessageBox.Show("تم إرسال الفاتورة مسبقاً");
                    return;
                }

                InvoiceProcessor processor =
                    new InvoiceProcessor(connectionString);

                processor.SellerName = "مؤسسة يزن للتجهيزات العلمية";
                processor.SellerTaxNo = "1160788";

                await processor.SendInvoiceToTaxAsync(invoiceId);

                MessageBox.Show("تم إرسال الفاتورة إلى نظام الفوترة");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool IsInvoicePosted(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                "SELECT PostedToTax FROM Invoices WHERE InvoiceId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", invoiceId);

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToBoolean(result);

                return false;
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
