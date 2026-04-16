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
    public partial class frm_AddCheque : DevExpress.XtraEditors.XtraForm
    {
        SupplierService service;
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_AddCheque()
        {
            InitializeComponent();
            service = new SupplierService(connectionString);
  }
    
        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
INSERT INTO SupplierCheques
(SupplierName, ChequeNumber, Amount, DueDate)
VALUES
(@name, @no, @amount, @date);
SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@name", cbxSupplier.Text);
                cmd.Parameters.AddWithValue("@no", txtChequeNo.Text);
                cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtAmount.Text));
                cmd.Parameters.AddWithValue("@date", dtpDate.Value.Date);

                int chequeId = Convert.ToInt32(cmd.ExecuteScalar());

                // 🔥 إضافة حركة للبنك
                AddBankTransaction(
                    dtpDate.Value.Date, // نفس تاريخ الشيك
                    "شيك صادر قديم رقم " + txtChequeNo.Text,
                    0, // Debit
                    Convert.ToDecimal(txtAmount.Text), // Credit
                    "OldCheque",
                    chequeId
                );

                MessageBox.Show("تم حفظ الشيك وربطه بالبنك ✔️");
                AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
                txtChequeNo.Clear();
                txtAmount.Clear();
                
            }
        }

        private void frm_AddCheque_Load(object sender, EventArgs e)
        {
            cbxSupplier.DataSource = service.GetAllSuppliers();
            cbxSupplier.DisplayMember = "Name";

            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "dd/MM/yyyy";

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
    }
}