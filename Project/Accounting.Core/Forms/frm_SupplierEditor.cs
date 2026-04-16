using Accounting.Core.Models;
using Accounting.Core.Services;
using System;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_SupplierEditor : Form
    {
        private SupplierService service;
        private int? _supplierId = null;

        private string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_SupplierEditor(int? supplierId = null)
        {
            InitializeComponent();

            service = new SupplierService(connectionString);
            _supplierId = supplierId;
        }

        private void frm_SupplierEditor_Load(object sender, EventArgs e)
        {
            //if (_supplierId != null)
            //{
            //    var supplier = service.GetSupplierById(_supplierId.Value);

            //    if (supplier != null)
            //    {
            //        txtName.Text = supplier.Name;
            //        txtPhone.Text = supplier.Phone;
            //        txtTax.Text = supplier.TaxNumber;
            //    }
            //}
            cbxBalanceType.Items.Clear();

            cbxBalanceType.Items.Add("مدين");
            cbxBalanceType.Items.Add("دائن");

            cbxBalanceType.SelectedIndex = 0;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Supplier supplier = new Supplier
            {
                SupplierId = _supplierId ?? 0,
                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                TaxNumber = txtTax.Text.Trim(),
                IsActive = true
            };

            if (_supplierId == null)
            {
                service.AddSupplier(supplier);
            }
            else
            {
                service.UpdateSupplier(supplier);
            }

            MessageBox.Show("تم حفظ المورد بنجاح");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {

            Supplier supplier = new Supplier
            {
                SupplierId = _supplierId ?? 0,
                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                TaxNumber = txtTax.Text.Trim(),
                IsActive = true
            };

            decimal openingBalance = 0;
            decimal.TryParse(txtOpeningBalance.Text, out openingBalance);

            string balanceType = cbxBalanceType.Text;

            if (_supplierId == null)
            {
                service.AddSupplierWithOpeningBalance(
                    supplier,
                    openingBalance,
                    balanceType);
            }
            else
            {
                service.UpdateSupplier(supplier);
            }

            MessageBox.Show("تم حفظ المورد بنجاح");
            AppEvents.RefreshDashboard();
            this.DialogResult = DialogResult.OK;
            this.Close();
           
        }
    }
}



