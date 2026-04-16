using System;
using System.Windows.Forms;
using Accounting.Core.Services;
using Accounting.Core.Models;
using System.Drawing;
using System.Data.SqlClient;

namespace Accounting.Core.Forms
{
    public partial class frm_ProductEditor : Form
    {
        private ProductService _service;
        private Product _product;

        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_ProductEditor(Product product = null)
        {
            InitializeComponent();

            _service = new ProductService(connectionString);
            _product = product;
        }

        private void frm_ProductEditor_Load(object sender, EventArgs e)
        {
            if (_product != null)
            {
                txtName.Text = _product.Name;
                txtUnit.Text = _product.Unit;
                txtPrice.Text = _product.Price.ToString();
                txtTax.Text = _product.TaxRate.ToString();

                // ❌ لا تحمل الرصيد الافتتاحي من هنا
                txtCostPrice.Text = "0";
            }
        }
      
     private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = (_product == null || _product.ProductId == 0);

                if (_product == null)
                    _product = new Product();

                _product.Name = txtName.Text;
                _product.Unit = txtUnit.Text;
                _product.Price = Convert.ToDecimal(txtPrice.Text);
                _product.TaxRate = Convert.ToDecimal(txtTax.Text);

                if (isNew)
                {
                    _service.AddProduct(_product);

                    MessageBox.Show("تم إضافة المنتج ✔️");
                }
                else
                {
                    _service.UpdateProduct(_product);

                    MessageBox.Show("تم تعديل المنتج ✔️");
                }
                AppEvents.RefreshDashboard();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
     
    }
}
