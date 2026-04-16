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
    public partial class frm_AddBankBalance : DevExpress.XtraEditors.XtraForm
    {
        public frm_AddBankBalance()
        {
            InitializeComponent();
        }
        public decimal Amount { get; set; }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal a) || a <= 0)
            {
                MessageBox.Show("أدخل مبلغ صحيح");
                return;
            }

            Amount = a;

            this.DialogResult = DialogResult.OK;
            this.Close();
            AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
        }
    }
}