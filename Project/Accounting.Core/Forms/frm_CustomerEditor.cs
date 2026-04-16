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

namespace Accounting.Core.Forms
{
    public partial class frm_CustomerEditor : Form
    {
        private CustomerService service;
        private Customer _customer;

        string connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_CustomerEditor(Customer customer = null)
        {
            InitializeComponent();

            service = new CustomerService(connectionString);
            _customer = customer;
        }

        private void frm_CustomerEditor_Load(object sender, EventArgs e)
        {
            if (_customer != null)
            {
                txtName.Text = _customer.Name;
                txtPhone.Text = _customer.Phone;
                txtTax.Text = _customer.TaxNumber;
            }
            cbxBalanceType.DataSource = new[]
{
    new { Id = 1, Name = "مدين" },
    new { Id = 2, Name = "دائن" }
};

            cbxBalanceType.DisplayMember = "Name";
            cbxBalanceType.ValueMember = "Id";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (_customer == null)
                _customer = new Customer();

            _customer.Name = txtName.Text;
            _customer.Phone = txtPhone.Text;
            _customer.TaxNumber = txtTax.Text;

            if (_customer.CustomerId == 0)
            {
                int newId = service.AddCustomer(_customer);

                decimal opening = 0;
                decimal.TryParse(txtOpeningBalance.Text, out opening);

                if (opening > 0)
                {
                    service.SaveOpeningBalance(
                        newId,
                        opening,
                        Convert.ToInt32(cbxBalanceType.SelectedValue)
                    );
                }
                MessageBox.Show("تم حفظ العميل بنجاح");
            }
            else
            {
                service.UpdateCustomer(_customer);
                MessageBox.Show("تم تعديل بيانات العميل");
            }
            AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
            this.Close();
          

        }


    }

}
