using Accounting.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;
using System.Configuration;
namespace Accounting.Core.Forms
{
    public partial class frm_CustomerStatement : Form
    {
        CustomerService service;

        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_CustomerStatement()
        {
            InitializeComponent();
            service = new CustomerService(connectionString);
        }
        CustomerService cs;
        bool isSelecting = false;

        //Database db = new Database();
        private void frm_CustomerStatement_Load(object sender, EventArgs e)
        {


            dateFrom.EditValue =
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            dateTo.EditValue = DateTime.Today;



           cs = new CustomerService(connectionString);
           

            // ================= العملاء =================
            var customers = cs.GetAllCustomers();

            searchLookUpEdit1.Properties.DataSource = customers;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "CustomerId";

            searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            searchLookUpEdit1.Properties.View.PopulateColumns();
            // 🔥 إخفاء كل الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();

            // 🔥 عرض اسم العميل فقط
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم العميل");
            // الخط داخل القائمة
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Regular);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            // الخط داخل مربع الإدخال
            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            // محاذاة
            searchLookUpEdit1.Properties.Appearance.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Near;

            // ارتفاع الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;

            // 🔥 ربط الحدث
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChanged;



            gridView1.CustomColumnDisplayText += gridView1_CustomColumnDisplayText;
            CustomizeGridView(gridView1);
            LoadCustomers();
            cbxCustomer.DropDownStyle = ComboBoxStyle.DropDown;

        }
        private void LoadCustomers()
        {
            cbxCustomer.DataSource = service.GetAllCustomers();

            cbxCustomer.DisplayMember = "Name";
            cbxCustomer.ValueMember = "CustomerId";
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (cbxCustomer.SelectedValue == null)
                return;
            int customerId = 0;

            if (cbxCustomer.SelectedValue != null && cbxCustomer.SelectedValue is int)
            {
                customerId = (int)cbxCustomer.SelectedValue;
            }
            else if (cbxCustomer.SelectedItem is DataRowView row)
            {
                customerId = Convert.ToInt32(row["CustomerId"]);
            }
            else
            {
                MessageBox.Show("اختر عميل صحيح");
                return;
            }



            DateTime fromDate = Convert.ToDateTime(dateFrom.EditValue).Date;
            DateTime toDate = Convert.ToDateTime(dateTo.EditValue).Date;

            LoadStatement(customerId, fromDate, toDate);


        }

        private void cbxCustomer_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbxCustomer.SelectedItem == null)
                return;

            DataRowView row = cbxCustomer.SelectedItem as DataRowView;

            if (row == null)
                return;

            int customerId = Convert.ToInt32(row["CustomerId"]);



        }
        private void LoadStatement(int customerId, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = service.GetCustomerStatement(customerId, fromDate, toDate);

            gridControl1.DataSource = dt;

            gridView1.PopulateColumns();

            gridView1.Columns["Description"].Caption = "العملية";

            gridView1.Columns["InvoiceId"].Visible = false;
            gridView1.Columns["ReceiptId"].Visible = false;

            AddSummaries();
        }

        private void ShowReport(DataTable dt)
        {
            var rpt = BuildReport(dt);

            frm_ReportViewer frm = new frm_ReportViewer(rpt);
            frm.ShowDialog();
        }


        Database db = new Database();
        DataTable tbl = new DataTable();

        private void btnPrint_Click(object sender, EventArgs e)
        {

          

            try
            {
                if (searchLookUpEdit1.EditValue == null)
                {
                    MessageBox.Show("اختر عميل");
                    return;
                }

                int customerId = Convert.ToInt32(searchLookUpEdit1.EditValue);
                DateTime fromDate = Convert.ToDateTime(dateFrom.EditValue).Date;
                DateTime toDate = Convert.ToDateTime(dateTo.EditValue).Date;

                // 🔥 نفس الدالة اللي شغالة معك
                DataTable dt = GetCustomerStatement(customerId, fromDate, toDate);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للطباعة");
                    return;
                }

                // 🔥 مهم جداً
                dt.TableName = "CustomerStatement";

                rptkenzy rpt = new rptkenzy();
                rpt.SetDataSource(dt);

                // ===========================
                // 🧾 اسم العميل
                rpt.SetParameterValue("PName", searchLookUpEdit1.Text);

                // 📅 الفترة
                rpt.SetParameterValue("PFromDate", fromDate.ToString("dd/MM/yyyy"));
                rpt.SetParameterValue("PToDate", toDate.ToString("dd/MM/yyyy"));

                // ===========================
                // 💰 الرصيد النهائي
                decimal finalBalance = Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["Balance"]);

                string finalBalanceText;

                if (finalBalance >= 0)
                    finalBalanceText = "الرصيد النهائي: " + finalBalance.ToString("0.00") + " مدين";
                else
                    finalBalanceText = "الرصيد النهائي: " + Math.Abs(finalBalance).ToString("0.00") + " دائن";

                rpt.SetParameterValue("PFinalBalance", finalBalanceText);

                // ===========================
                // 🔤 التفقيط
                string tafqeet = NumberConverter.ConvertToArabicWords(Math.Abs(finalBalance));
                string tafqeetText = "فقط: " + tafqeet + " أردني لا غير";

                rpt.SetParameterValue("PTafqeet", tafqeetText);

                // ===========================
                // 🖨️ الطباعة
                //rpt.PrintToPrinter(1, false, 0, 0);

                frm_ReportViewer frm = new frm_ReportViewer(rpt);

                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message);
            }

        }


        public static class NumberConverter
        {
            private static readonly string[] units =
            {
        "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة",
        "ستة", "سبعة", "ثمانية", "تسعة", "عشرة",
        "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر",
        "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
    };

            private static readonly string[] tens =
            {
        "", "", "عشرون", "ثلاثون", "أربعون",
        "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
    };

            private static readonly string[] hundreds =
            {
        "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة",
        "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
    };

            public static string ConvertToArabicWords(decimal number)
            {
                if (number == 0)
                    return "صفر دينار";

                long dinar = (long)Math.Floor(number);
                int fils = (int)Math.Round((number - dinar) * 100);

                string result = ConvertNumber(dinar) + " دينار";

                if (dinar != 1 && dinar != 2)
                    result += "اً";

                if (fils > 0)
                {
                    result += " و " + ConvertNumber(fils) + " فلس";

                    if (fils != 1 && fils != 2)
                        result += "اً";
                }

                return result;
            }

            private static string ConvertNumber(long number)
            {
                if (number == 0)
                    return "";

                if (number < 20)
                    return units[number];

                if (number < 100)
                {
                    if (number % 10 == 0)
                        return tens[number / 10];

                    return units[number % 10] + " و" + tens[number / 10];
                }

                if (number < 1000)
                {
                    if (number % 100 == 0)
                        return hundreds[number / 100];

                    return hundreds[number / 100] + " و" + ConvertNumber(number % 100);
                }

                if (number < 1000000)
                {
                    long thousands = number / 1000;
                    long remainder = number % 1000;

                    string thousandText = "";

                    if (thousands == 1)
                        thousandText = "ألف";
                    else if (thousands == 2)
                        thousandText = "ألفان";
                    else
                        thousandText = ConvertNumber(thousands) + " آلاف";

                    if (remainder == 0)
                        return thousandText;

                    return thousandText + " و" + ConvertNumber(remainder);
                }

                if (number < 1000000000)
                {
                    long millions = number / 1000000;
                    long remainder = number % 1000000;

                    string millionText = "";

                    if (millions == 1)
                        millionText = "مليون";
                    else if (millions == 2)
                        millionText = "مليونان";
                    else
                        millionText = ConvertNumber(millions) + " ملايين";

                    if (remainder == 0)
                        return millionText;

                    return millionText + " و" + ConvertNumber(remainder);
                }

                return number.ToString();
            }
        }

        private rptCustomerStatement BuildReport(DataTable dt)
        {
            rptCustomerStatement rpt = new rptCustomerStatement();

          

            rpt.SetParameterValue("pCustomerName", cbxCustomer.Text);
            rpt.SetParameterValue("pFromDate", dateFrom.DateTime);
            rpt.SetParameterValue("pToDate", dateTo.DateTime);
            rpt.PrintOptions.PrinterName = "";
            return rpt;
        }
        bool isOpening = false;
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
          
            try
            {
                if (gridView1.FocusedRowHandle < 0)
                    return;

                var receiptVal = gridView1.GetFocusedRowCellValue("ReceiptId");
                var invoiceVal = gridView1.GetFocusedRowCellValue("InvoiceId");

                // ================= سند قبض =================
                if (receiptVal != null && receiptVal != DBNull.Value)
                {
                    if (!int.TryParse(receiptVal.ToString(), out int receiptId))
                        return;

                    frm_Receipt frm = new frm_Receipt(receiptId);
                    frm.ShowDialog();
                }

                // ================= فاتورة =================
                else if (invoiceVal != null && invoiceVal != DBNull.Value)
                {
                    if (!int.TryParse(invoiceVal.ToString(), out int invoiceId))
                        return;

                    frm_InvoiceEditor frm = new frm_InvoiceEditor(invoiceId);
                    frm.ShowDialog();
                }
            }
            finally
            {
                isOpening = false;
            }

        }
        private int GetInvoiceIdByNumber(string number)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = "SELECT InvoiceId FROM Invoices WHERE InvoiceNumber=@No";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@No", number);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        private int GetReceiptIdByNumber(string number)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = "SELECT ReceiptId FROM Receipts WHERE ReceiptNumber=@No";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@No", number);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        private void gridView1_RowCellStyle(object sender,
DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

            var typeObj = gridView1.GetRowCellValue(e.RowHandle, "OperationType");
            string type = typeObj?.ToString() ?? "";

            decimal debit = 0, credit = 0, bal = 0;

            var dObj = gridView1.GetRowCellValue(e.RowHandle, "Debit");
            if (dObj != DBNull.Value && dObj != null) debit = Convert.ToDecimal(dObj);

            var cObj = gridView1.GetRowCellValue(e.RowHandle, "Credit");
            if (cObj != DBNull.Value && cObj != null) credit = Convert.ToDecimal(cObj);

            var bObj = gridView1.GetRowCellValue(e.RowHandle, "Balance");
            if (bObj != DBNull.Value && bObj != null) bal = Convert.ToDecimal(bObj);

            // ⭐ رصيد سابق
            if (type == "رصيد سابق")
            {
                e.Appearance.BackColor = Color.Beige;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                return;
            }

            // ⭐ مدين أزرق / دائن أحمر
            if (debit > 0) e.Appearance.BackColor = Color.LightBlue;
            if (credit > 0) e.Appearance.BackColor = Color.MistyRose;

            // ⭐ الرصيد بالسالب أحمر (فقط في عمود الرصيد)
            if (e.Column.FieldName == "Balance" && bal < 0)
                e.Appearance.ForeColor = Color.Red;

        }

        private void AddSummaries()
        {
            gridView1.OptionsView.ShowFooter = true;

            var debitCol = gridView1.Columns["Debit"];
            var creditCol = gridView1.Columns["Credit"];
            var balanceCol = gridView1.Columns["Balance"];

            if (debitCol != null)
            {
                debitCol.Summary.Clear();

                debitCol.Summary.Add(
                    DevExpress.Data.SummaryItemType.Sum,
                    "Debit",
                    "إجمالي المدين = {0:N2}");
            }

            if (creditCol != null)
            {
                creditCol.Summary.Clear();

                creditCol.Summary.Add(
                    DevExpress.Data.SummaryItemType.Sum,
                    "Credit",
                    "إجمالي الدائن = {0:N2}");
            }

            gridView1.BestFitColumns();
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Balance" && e.Value != null)
            {
                decimal bal = Convert.ToDecimal(e.Value);

                if (bal >= 0)
                    e.DisplayText = bal.ToString("N2") + " مدين";
                else
                    e.DisplayText = Math.Abs(bal).ToString("N2") + " دائن";
            }
        }
        public void LoadCustomer(int customerId)
        {
            cbxCustomer.SelectedValue = customerId;

            DateTime fromDate = Convert.ToDateTime(dateFrom.EditValue);
            DateTime toDate = Convert.ToDateTime(dateTo.EditValue);

            LoadStatement(customerId, fromDate, toDate);
        }

        private void dateFrom_EditValueChanged(object sender, EventArgs e)
        {
            //LoadStatementFromUI();
        }

        private void SearchCustomers(string text)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
        SELECT CustomerId, Name, Phone
        FROM Customers
        WHERE Name LIKE @txt
        OR Phone LIKE @txt
        ORDER BY Name";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@txt", text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxCustomer.DataSource = dt;
                cbxCustomer.DisplayMember = "Name";
                cbxCustomer.ValueMember = "CustomerId";
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

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
          
           

          

        }

        private void cbxCustomer_TextChanged(object sender, EventArgs e)
        {

        }
        private DataTable GetFilteredTable(DataTable dt, DateTime? fromDate, DateTime? toDate)
        {
            DataView dv = dt.DefaultView;

            string filter = "";

            if (fromDate != null)
            {
                filter += $"InvoiceDate >= #{fromDate.Value:MM/dd/yyyy}#";
            }

            if (toDate != null)
            {
                if (filter != "") filter += " AND ";
                filter += $"InvoiceDate <= #{toDate.Value:MM/dd/yyyy}#";
            }

            dv.RowFilter = filter;

            return dv.ToTable();
        }

      

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
            {
                gridControl1.DataSource = null;
                return;
            }

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int customerId))
                return;

            // 🔥 ربط مع الكمبو (اختياري)
            cbxCustomer.SelectedValue = customerId;

            // 🔥 جلب التاريخ
            DateTime fromDate = Convert.ToDateTime(dateFrom.EditValue).Date;
            DateTime toDate = Convert.ToDateTime(dateTo.EditValue).Date;

            // 🔥 تحميل البيانات مباشرة
            LoadStatement(customerId, fromDate, toDate);

        }

        private void cbxCustomer_TextChanged_1(object sender, EventArgs e)
        {
            if (isSelecting) return;

            if (cs == null)
                cs = new CustomerService(connectionString);

            string text = cbxCustomer.Text.Trim();

            if (text.Length < 2)
                return;

            isSelecting = true;

            var dt = cs.SearchCustomers(text);

            if (dt.Rows.Count > 0)
            {
                cbxCustomer.DataSource = dt;
                cbxCustomer.DisplayMember = "Name";
                cbxCustomer.ValueMember = "CustomerId";
                cbxCustomer.SelectedIndex = -1;

                cbxCustomer.DroppedDown = true;
                cbxCustomer.SelectionStart = cbxCustomer.Text.Length;
            }

            isSelecting = false;
        }

        private DataTable CustomerStatement(int customerId, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CustomerStatement", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("dd/MM/yyyy"));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private DataTable GetCustomerStatement(int customerId, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CustomerStatement", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("dd/MM/yyyy"));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
        private void LoadStatementFromUI()
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (dateFrom.EditValue == null || dateTo.EditValue == null)
                return;

            int customerId = Convert.ToInt32(searchLookUpEdit1.EditValue);
            DateTime fromDate = Convert.ToDateTime(dateFrom.EditValue).Date;
            DateTime toDate = Convert.ToDateTime(dateTo.EditValue).Date;

            LoadStatement(customerId, fromDate, toDate);
        }

        private void dateTo_EditValueChanged(object sender, EventArgs e)
        {
            //LoadStatementFromUI();
        }
    }
}
