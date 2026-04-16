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
using System.Data.SqlClient;

namespace Accounting.Core.Forms
{
    public partial class frm_TransferBetweenAccounts : DevExpress.XtraEditors.XtraForm
    {
        public frm_TransferBetweenAccounts()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private void frm_TransferBetweenAccounts_Load(object sender, EventArgs e)
        {
            var accounts = new List<dynamic>()
    {
        new { Id = 1, Name = "الصندوق" },
        new { Id = 2, Name = "البنك" }
    };

            cbxfrom.DataSource = accounts.ToList();
            cbxfrom.DisplayMember = "Name";
            cbxfrom.ValueMember = "Id";

            cbxTo.DataSource = accounts.ToList();
            cbxTo.DisplayMember = "Name";
            cbxTo.ValueMember = "Id";

            cbxfrom.SelectedValue = 1;
            cbxTo.SelectedValue = 2;

            dtDate.Format = DateTimePickerFormat.Custom;
            dtDate.CustomFormat = "dd/MM/yyyy";
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (cbxfrom.Text == cbxTo.Text)
            {
                MessageBox.Show("لا يمكن التحويل لنفس الحساب");
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("أدخل مبلغ صحيح");
                return;
            }

            DateTime date = dtDate.Value;
            string notes = txtnote.Text;

            // 🔴 من الحساب (Credit)
            if (cbxfrom.Text == "الصندوق")
            {
                AddCashTransaction(date, "تحويل إلى " + cbxTo.Text + " - " + notes, 0, amount, "Transfer", null);
            }
            else if (cbxfrom.Text == "البنك")
            {
                AddBankTransaction(date, "تحويل إلى " + cbxTo.Text + " - " + notes, 0, amount, "Transfer", null);
            }

            // 🟢 إلى الحساب (Debit)
            if (cbxTo.Text == "الصندوق")
            {
                AddCashTransaction(date, "تحويل من " + cbxfrom.Text + " - " + notes, amount, 0, "Transfer", null);
            }
            else if (cbxTo.Text == "البنك")
            {
                AddBankTransaction(date, "تحويل من " + cbxfrom.Text + " - " + notes, amount, 0, "Transfer", null);
            }

            MessageBox.Show("تم التحويل بنجاح ✔️");
            AppEvents.RefreshDashboard();
            txtAmount.Clear();
            txtnote.Clear();

           
        }
        private void AddBankTransaction(DateTime date, string desc, decimal debit, decimal credit, string refType, int? refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 🔥 آخر رصيد
                SqlCommand cmdBalance = new SqlCommand(
                    "SELECT TOP 1 Balance FROM BankTransactions ORDER BY Id DESC", con);

                object lastBalanceObj = cmdBalance.ExecuteScalar();

                decimal lastBalance = lastBalanceObj != null ? Convert.ToDecimal(lastBalanceObj) : 0;

                decimal newBalance = lastBalance + debit - credit;

                // 🔥 إدخال الحركة
                SqlCommand cmd = new SqlCommand(@"
INSERT INTO BankTransactions
(TransDate, Description, Debit, Credit, Balance, RefType, RefId)
VALUES
(@date, @desc, @debit, @credit, @balance, @type, @ref)", con);

                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@debit", debit);
                cmd.Parameters.AddWithValue("@credit", credit);
                cmd.Parameters.AddWithValue("@balance", newBalance);
                cmd.Parameters.AddWithValue("@type", refType);
                cmd.Parameters.AddWithValue("@ref", (object)refId ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
        private void AddCashTransaction(DateTime date, string desc, decimal debit, decimal credit, string refType, int? refId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // آخر رصيد
                SqlCommand cmdBalance = new SqlCommand(
                    "SELECT TOP 1 Balance FROM CashTransactions ORDER BY CashId DESC", con);

                object lastBalanceObj = cmdBalance.ExecuteScalar();

                decimal lastBalance = lastBalanceObj != null ? Convert.ToDecimal(lastBalanceObj) : 0;

                decimal newBalance = lastBalance + debit - credit;

                // إدخال الحركة
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
                cmd.Parameters.AddWithValue("@ref", (object)refId ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}