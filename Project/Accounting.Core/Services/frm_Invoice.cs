using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Accounting.Core.Models;

namespace Accounting.Core.Services
{
    public partial class frm_Invoice : DevExpress.XtraEditors.XtraForm
    {
        public frm_Invoice()
        {
            InitializeComponent();
            string connectionString =
       @"Data Source=.\SQLEXPRESS;
          Initial Catalog=AccountingCoreDB;
          Integrated Security=True";

            _invoiceService = new InvoiceService(connectionString);
        }
        private InvoiceService _invoiceService;

        private void frm_Invoice_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = new List<InvoiceLine>();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}