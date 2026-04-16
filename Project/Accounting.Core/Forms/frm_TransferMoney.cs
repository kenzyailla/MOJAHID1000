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

namespace Accounting.Core.Forms
{
    public partial class frm_TransferMoney : DevExpress.XtraEditors.XtraForm
    {
        public frm_TransferMoney()
        {
            InitializeComponent();
        }
        public decimal Amount { get; set; }
        private void frm_TransferMoney_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal a) || a <= 0)
            {
                MessageBox.Show("أدخل مبلغ صحيح");
                return;
            }

            Amount = a;
            AppEvents.RefreshDashboard();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}