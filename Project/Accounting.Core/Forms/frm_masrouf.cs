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
using Accounting.Core.Services;

namespace Accounting.Core.Forms
{
    public partial class frm_masrouf : DevExpress.XtraEditors.XtraForm
    {
        private int? _expenseId;
        SupplierService service;
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        public frm_masrouf()
        {
            InitializeComponent();
            service = new SupplierService(connectionString);
        }
        public frm_masrouf(int expenseId) : this()
        {
            _expenseId = expenseId;
            this.Text = "سند مصروف رقم: " + expenseId;

            LoadExpenseData();
        }
        private void LoadExpenseData()
        {
            if (_expenseId == null) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ExpenseDate, Description, Amount, Category, PaymentType
FROM Expenses
WHERE ExpenseId = @id", con);

                cmd.Parameters.AddWithValue("@id", _expenseId);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    dtpDate.Value = Convert.ToDateTime(dr["ExpenseDate"]);
                    txtDesc.Text = dr["Description"].ToString();
                    txtAmount.Text = dr["Amount"].ToString();
                    cbxCategory.Text = dr["Category"].ToString();
                    cbxPaymentType.Text = dr["PaymentType"].ToString();
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                decimal amount = Convert.ToDecimal(txtAmount.Text);
                DateTime expenseDate = dtpDate.Value.Date;
                string description = txtDesc.Text;
                string category = cbxCategory.Text;
                string paymentType = cbxPaymentType.Text;

                int expenseAccountId = GetExpenseAccountId(category);
                int paymentAccountId = GetPaymentAccountId(paymentType);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction trans = con.BeginTransaction();

                    try
                    {
                        // 🥇 1) إنشاء القيد
                        int journalId;

                        SqlCommand cmd1 = new SqlCommand(@"
INSERT INTO JournalEntries (EntryDate, Description)
OUTPUT INSERTED.JournalId
VALUES (@date, @desc)", con, trans);

                        cmd1.Parameters.AddWithValue("@date", expenseDate);
                        cmd1.Parameters.AddWithValue("@desc", "مصروف - " + description);

                        journalId = Convert.ToInt32(cmd1.ExecuteScalar());

                        // 🥇 2) هنا نضيف السطور 👇👇👇

                        // مدين (المصروف)
                        AddJournalLine(con, trans, journalId, expenseAccountId, amount, 0);

                        // دائن (طريقة الدفع)
                        AddJournalLine(con, trans, journalId, paymentAccountId, 0, amount);

                        // 🥇 3) حفظ المصروف
                        SqlCommand cmd2 = new SqlCommand(@"
INSERT INTO Expenses (ExpenseDate, Description, Amount, Category, PaymentType, JournalId)
OUTPUT INSERTED.ExpenseId
VALUES (@date, @desc, @amount, @cat, @pay, @journalId)", con, trans);

                      

                        cmd2.Parameters.AddWithValue("@date", expenseDate);
                        cmd2.Parameters.AddWithValue("@desc", description);
                        cmd2.Parameters.AddWithValue("@amount", amount);
                        cmd2.Parameters.AddWithValue("@cat", category);
                        cmd2.Parameters.AddWithValue("@pay", paymentType);
                        cmd2.Parameters.AddWithValue("@journalId", journalId);
                        int expenseId = Convert.ToInt32(cmd2.ExecuteScalar());
                      

                        if (paymentType == "شيك")
                        {
                            SqlCommand cmdCheque = new SqlCommand(@"
INSERT INTO OutgoingCheques
(PayeeName, ChequeNumber, Amount, IssueDate, DueDate, Description)
VALUES (@payee, @no, @amount, @issueDate, @date, @desc)", con, trans);

                            // 🔥 هذا هو السطر الناقص
                            cmdCheque.Parameters.AddWithValue("@payee", txtDesc.Text);

                            cmdCheque.Parameters.AddWithValue("@no", txtChequeNumber.Text);
                            cmdCheque.Parameters.AddWithValue("@amount", amount);
                            cmdCheque.Parameters.AddWithValue("@issueDate", DateTime.Today);
                            cmdCheque.Parameters.AddWithValue("@date", dtpChequeDate.Value.Date);
                            cmdCheque.Parameters.AddWithValue("@desc", txtDesc.Text);

                            cmdCheque.ExecuteNonQuery();
                        }
                        // 🔥 فقط إذا نقدي
                        if (paymentType == "نقدي")
                        {
                            AddCashTransaction(
                                expenseDate,                 // بدل date
                                "مصروف - " + description,
                                0,
                                amount,
                                "Expense",
                                expenseId                   // الآن موجود ✔️
                            );
                        }
                        trans.Commit();

                        MessageBox.Show("تم حفظ المصروف مع القيد ✔️");
                        AppEvents.RefreshDashboard();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void employees_Load(object sender, EventArgs e)
        {
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "dd/MM/yyyy";
            dtpChequeDate.Format = DateTimePickerFormat.Custom;
            dtpChequeDate.CustomFormat = "dd/MM/yyyy";
            cbxPaymentType.Items.Clear();
            cbxPaymentType.Items.Add("نقدي");
            cbxPaymentType.Items.Add("شيك");
            cbxPaymentType.SelectedIndex = 0;

        

        }
        private int GetExpenseAccountId(string category)
        {
            return GetAccountId("502"); // لازم تضيف حساب مصروفات
        }
        private int GetPaymentAccountId(string paymentType)
        {
            if (paymentType == "نقدي")
                return GetAccountId("101"); // الصندوق

            if (paymentType == "شيك")
                return GetAccountId("105"); // شيكات صادرة

            return GetAccountId("101");
        }
        private void AddJournalLine(SqlConnection con, SqlTransaction trans,
    int journalId, int accountId, decimal debit, decimal credit)
        {
            SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalLines (JournalId, AccountId, Debit, Credit)
VALUES (@j, @a, @d, @c)", con, trans);

            cmd.Parameters.AddWithValue("@j", journalId);
            cmd.Parameters.AddWithValue("@a", accountId);
            cmd.Parameters.AddWithValue("@d", debit);
            cmd.Parameters.AddWithValue("@c", credit);

            cmd.ExecuteNonQuery();
        }
        private int GetAccountId(string accountCode)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT AccountId FROM Accounts WHERE AccountCode = @code", con);

                cmd.Parameters.AddWithValue("@code", accountCode);

                object result = cmd.ExecuteScalar();

                if (result == null)
                    throw new Exception("الحساب غير موجود: " + accountCode);

                return Convert.ToInt32(result);
            }
        }

        private void cbxPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isCheque = cbxPaymentType.Text == "شيك";

            txtChequeNumber.Visible = isCheque;
            dtpChequeDate.Visible = isCheque;
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
    }
}