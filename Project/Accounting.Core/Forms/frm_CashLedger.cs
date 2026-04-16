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
using static Accounting.Core.Forms.frm_CustomerStatement;

namespace Accounting.Core.Forms
{
    public partial class frm_CashLedger : DevExpress.XtraEditors.XtraForm
    {
        static string[] units =
{
    "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة",
    "ستة", "سبعة", "ثمانية", "تسعة", "عشرة",
    "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر",
    "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
};

        static string[] tens =
        {
    "", "", "عشرون", "ثلاثون", "أربعون",
    "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
};

        static string[] hundreds =
        {
    "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة",
    "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
};

        public frm_CashLedger()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private void LoadCash(DateTime from, DateTime to)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 🥇 1) حساب الرصيد السابق
                decimal openingBalance = 0;

                SqlCommand cmdOpening = new SqlCommand(@"
SELECT ISNULL(SUM(Debit - Credit),0)
FROM CashTransactions
WHERE TransDate < @from", con);

                cmdOpening.Parameters.AddWithValue("@from", from);

                openingBalance = Convert.ToDecimal(cmdOpening.ExecuteScalar());

                // 🥇 2) جلب الحركات
                string sql = @"
SELECT 
    CashId,
    TransDate,
    Description,
    Debit,
    Credit,
    Balance,
 RefType,
    RefId
FROM CashTransactions
WHERE TransDate BETWEEN @from AND @to
ORDER BY CashId";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                da.SelectCommand.Parameters.AddWithValue("@from", from);
                da.SelectCommand.Parameters.AddWithValue("@to", to);

                DataTable dt = new DataTable();
                da.Fill(dt);

                // 🥇 3) إضافة صف الرصيد السابق
                DataRow row = dt.NewRow();
                row["TransDate"] = from;
                row["Description"] = "رصيد سابق";
                row["Debit"] = 0;
                row["Credit"] = 0;
                row["Balance"] = openingBalance;

                dt.Rows.InsertAt(row, 0); // 🔥 أول سطر

                gridControl1.DataSource = dt;

                // 🥇 4) عرض الرصيد النهائي
                if (dt.Rows.Count > 0)
                {
                    decimal balance = Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["Balance"]);
                    lblBalance.Text = "الرصيد الحالي: " + balance.ToString("N3");
                }
                else
                {
                    lblBalance.Text = "الرصيد الحالي: 0";
                }

             
            }
        }
        private void frm_CashLedger_Load(object sender, EventArgs e)
        {
            dateFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dateTo.Value = DateTime.Today;

            // 🥇 أولاً: تحميل البيانات
            LoadCash(dateFrom.Value, dateTo.Value);

            // 🥇 ثانياً: إنشاء الأعمدة
            gridView1.PopulateColumns();

            // 🥇 ثالثاً: التعديل على الأعمدة
            if (gridView1.Columns["CashId"] != null)
                gridView1.Columns["CashId"].Visible = false;

            if (gridView1.Columns["TransDate"] != null)
                gridView1.Columns["TransDate"].Caption = "التاريخ";

            if (gridView1.Columns["Description"] != null)
                gridView1.Columns["Description"].Caption = "البيان";

            if (gridView1.Columns["Debit"] != null)
                gridView1.Columns["Debit"].Caption = "مدين";

            if (gridView1.Columns["Credit"] != null)
                gridView1.Columns["Credit"].Caption = "دائن";

            if (gridView1.Columns["Balance"] != null)
                gridView1.Columns["Balance"].Caption = "الرصيد";

            dateFrom.Format = DateTimePickerFormat.Custom;
            dateFrom.CustomFormat = "dd/MM/yyyy";
            dateTo.Format = DateTimePickerFormat.Custom;
            dateTo.CustomFormat = "dd/MM/yyyy";
            gridView1.Columns["RefType"].Visible = false;
            gridView1.Columns["RefId"].Visible = false;
            gridView1.BestFitColumns();
            CustomizeGridView(gridView1);
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCash(dateFrom.Value.Date, dateTo.Value.Date);
          
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            string desc = gridView1.GetRowCellValue(e.RowHandle, "Description")?.ToString();

            if (desc == "رصيد سابق")
            {
                e.Appearance.BackColor = Color.LightGray;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }
   

        private void CustomizeGridView(DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            // General appearance
            gridView.Appearance.Empty.BackColor = Color.White;
            gridView.Appearance.Row.BackColor = Color.White;
            gridView.Appearance.Row.ForeColor = Color.Black;
            gridView.Appearance.FocusedRow.BackColor = Color.FromArgb(173, 216, 230); // Light blue
            gridView.Appearance.FocusedRow.ForeColor = Color.Black;
            gridView.Appearance.SelectedRow.BackColor = Color.FromArgb(173, 216, 230);
            gridView.Appearance.SelectedRow.ForeColor = Color.Black;

            // Header appearance
            gridView.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 8.18868f, FontStyle.Bold);
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView.Appearance.HeaderPanel.BackColor = Color.LightGray;

            // Cell appearance
            gridView.Appearance.Row.Font = new Font("Noto Kufi Arabic", 8.18868f);
            gridView.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center; // Right alignment for Arabic

            // Alternating row colors (if needed)
            gridView.OptionsView.EnableAppearanceEvenRow = true;
            gridView.OptionsView.EnableAppearanceOddRow = true;
            gridView.Appearance.EvenRow.BackColor = Color.FromArgb(240, 248, 255); // AliceBlue
            gridView.Appearance.OddRow.BackColor = Color.White;

            // Grid lines
            gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            gridView.GridControl.LookAndFeel.UseDefaultLookAndFeel = false; // Allow custom styling

            // Row height
            gridView.RowHeight = 30;

        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            string refType = gridView1.GetFocusedRowCellValue("RefType")?.ToString();
            object refIdObj = gridView1.GetFocusedRowCellValue("RefId");

            if (string.IsNullOrEmpty(refType) || refIdObj == null || refIdObj == DBNull.Value)
                return;

            int refId = Convert.ToInt32(refIdObj);

            // 🔥 التوجيه حسب النوع
            if (refType == "Invoice")
            {
                frm_NewInvoice frm = new frm_NewInvoice(refId);
                frm.ShowDialog();
            }
            else if (refType == "Expense")
            {
                frm_masrouf frm = new frm_masrouf(refId);
                frm.ShowDialog();
            }
            else if (refType == "Payment")
            {
                frm_BuyPayment frm = new frm_BuyPayment(refId);
                frm.ShowDialog();
            }
            else if (refType == "Receipt")
            {
                frm_Receipt frm = new frm_Receipt(refId);
                frm.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = gridControl1.DataSource as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة");
                return;
            }

            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (DataRow row in dt.Rows)
            {
                string desc = row["Description"]?.ToString();

                decimal debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0;
                decimal credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0;

                // حتى لا يدخل "رصيد سابق" ضمن المجاميع
                if (desc != "رصيد سابق")
                {
                    totalDebit += debit;
                    totalCredit += credit;
                }
            }

            decimal finalBalance = 0;
            if (dt.Rows.Count > 0 && dt.Rows[dt.Rows.Count - 1]["Balance"] != DBNull.Value)
            {
                finalBalance = Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["Balance"]);
            }

            string tafqeet = "فقط: " + NumberConverter.ConvertToArabicWords(finalBalance) + " لا غير";

            rptCashLedger rpt = new rptCashLedger();
            rpt.SetDataSource(dt);

            rpt.SetParameterValue("PFromDate", dateFrom.Value.Date);
            rpt.SetParameterValue("PToDate", dateTo.Value.Date);
            rpt.SetParameterValue("PTotalDebit", totalDebit);
            rpt.SetParameterValue("PTotalCredit", totalCredit);
            rpt.SetParameterValue("PFinalBalance", finalBalance);
            rpt.SetParameterValue("PTafqeet", tafqeet);

            frm_ReportViewer frm = new frm_ReportViewer(rpt);
            frm.ShowDialog();
        }
        private void TransferToBank(decimal amount)
        {
            if (amount <= 0)
            {
                MessageBox.Show("أدخل مبلغ صحيح");
                return;
            }

            // 🔴 خصم من الصندوق
            AddCashTransaction(
                DateTime.Now,
                "تحويل إلى البنك",
                0,            // Debit
                amount,       // Credit
                "Transfer",
                null
            );

            // 🟢 إضافة إلى البنك
            AddBankTransaction(
                DateTime.Now,
                "تحويل من الصندوق",
                amount,       // Debit
                0,
                "Transfer",
                null
            );

            MessageBox.Show("تم التحويل بنجاح ✔️");
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

        private void btnTransferToBank_Click(object sender, EventArgs e)
        {
            frm_TransferMoney frm = new frm_TransferMoney();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                TransferToBank(frm.Amount);
            }
            AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
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
