using Accounting.Core.Common;
using Accounting.Core.Models;
using Accounting.Core.Services;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class uc_BuyInvoiceEditor : UserControl

    {
        private SupplierService supplierService;
        private string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";




        int? _invoiceId = null;




        RepositoryItemGridLookUpEdit repoProducts;

        bool isUpdatingGrid = false;

        private BuyInvoiceService service;
        public uc_BuyInvoiceEditor(int? invoiceId = null)

        {
            InitializeComponent();
            string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

            service = new BuyInvoiceService(connectionString);
            supplierService = new SupplierService(connectionString);
            _invoiceId = invoiceId;

            this.Padding = new Padding(0);
            this.Margin = new Padding(0);
            this.AutoScaleMode = AutoScaleMode.None;


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
                dateDue.Value = header["DueDate"] == DBNull.Value
                    ? DateTime.Today
                    : Convert.ToDateTime(header["DueDate"]);

                cbxSupplier.SelectedValue = header["SupplierId"];
                cbxPaymentStatus.SelectedIndex =
                    Convert.ToInt32(header["PaymentStatus"]) - 1;
            }

            DataTable dtLines = service.GetBuyInvoiceLines(id);

            gridItems.DataSource = dtLines;


        }

        private void frm_BuyInvoiceEditor_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            LoadPaymentStatus();

            PrepareGrid();              // 1️⃣ جهّز الجريد أولاً
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

            panel1.Dock = DockStyle.Top;
            panel1.Margin = new Padding(0);
            panel1.Padding = new Padding(0);

            gridItems.Dock = DockStyle.Fill;
            gridItems.Margin = new Padding(0);
            gridItems.Padding = new Padding(0);

            panel1.BringToFront();

        }

        private void uc_BuyInvoiceEditor_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            LoadPaymentStatus();

            PrepareGrid();
            PrepareProductLookup();

            if (_invoiceId != null)
            {
                LoadInvoice(_invoiceId.Value);
                gridViewItems.RefreshData();
            }

            gridViewItems.CellValueChanged += gridViewItems_CellValueChanged;
        }

        private void LoadPaymentStatus()
        {
            cbxPaymentStatus.Items.Clear();

            cbxPaymentStatus.Items.Add("آجلة");
            cbxPaymentStatus.Items.Add("مدفوعة");

            cbxPaymentStatus.SelectedIndex = 0;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            BuyInvoice inv = new BuyInvoice();

            inv.InvoiceNumber = Convert.ToInt32(txtInvoiceNumber.Text);
            inv.SupplierId = Convert.ToInt32(cbxSupplier.SelectedValue);
            inv.InvoiceDate = dateInvoice.Value;
            inv.DueDate = dateDue.Value;

            inv.PaymentStatus =
                (PaymentStatus)(cbxPaymentStatus.SelectedIndex + 1);

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

            if (_invoiceId == null)
            {
                service.AddBuyInvoice(inv);
            }
            else
            {
                service.UpdateBuyInvoice(_invoiceId.Value, inv);
            }


            MessageBox.Show("تم حفظ الفاتورة");
        }
        private void PrepareProductLookup()
        {

            repoProducts = new RepositoryItemGridLookUpEdit();

            ProductService ps = new ProductService(connectionString);

            repoProducts.DataSource = ps.GetAllProducts();
            repoProducts.DisplayMember = "Name";
            repoProducts.ValueMember = "ProductId";

            repoProducts.NullText = "";
            repoProducts.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            repoProducts.ImmediatePopup = true;
            repoProducts.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            repoProducts.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;

            repoProducts.PopulateViewColumns();

            // اخفاء رقم المنتج من قائمة البحث
            if (repoProducts.PopupView.Columns["ProductId"] != null)
                repoProducts.PopupView.Columns["ProductId"].Visible = false;

            gridItems.RepositoryItems.Add(repoProducts);

            if (gridViewItems.Columns["ProductId"] != null)
            {
                gridViewItems.Columns["ProductId"].ColumnEdit = repoProducts;
                gridViewItems.Columns["ProductId"].Caption = "المنتج";
            }
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



    }
}

