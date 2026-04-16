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
    public partial class frm_CustomerLedger : Form
    {
        public frm_CustomerLedger()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private void frm_CustomerLedger_Load(object sender, EventArgs e)
        {
            LoadCustomers();

            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;

            gridView1.FocusRectStyle =
                DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
        }
        private void LoadCustomers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT CustomerId, Name FROM Customers WHERE IsActive=1", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxCustomer.DataSource = dt;
                cbxCustomer.DisplayMember = "Name";
                cbxCustomer.ValueMember = "CustomerId";
            }
        }
        private void LoadLedger()
        {
            if (cbxCustomer.SelectedValue == null)
                return;

            int customerId = Convert.ToInt32(cbxCustomer.SelectedValue);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                int accountId = 0;

                //---------------------------------
                // جلب AccountId للعميل
                //---------------------------------

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT AccountId FROM Customers WHERE CustomerId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    accountId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                DateTime fromDate = dateFrom.DateTime.Date;
                DateTime toDate = dateTo.DateTime.Date;

                //---------------------------------
                // الرصيد الافتتاحي
                //---------------------------------

                decimal openingBalance = 0;

                using (SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(jl.Debit - jl.Credit),0)
FROM JournalLines jl
JOIN JournalEntries je ON jl.JournalId = je.JournalId
WHERE jl.AccountId=@Acc
AND je.EntryDate < @From", con))
                {
                    cmd.Parameters.AddWithValue("@Acc", accountId);
                    cmd.Parameters.AddWithValue("@From", fromDate);

                    openingBalance = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                //---------------------------------
                // الحركات داخل الفترة
                //---------------------------------

                DataTable dt = new DataTable();

                using (SqlCommand cmd = new SqlCommand(@"
SELECT
    je.EntryDate,
    je.ReferenceType,
    je.ReferenceId,
    je.Description,
    jl.Debit,
    jl.Credit
FROM JournalEntries je
JOIN JournalLines jl
ON je.JournalId = jl.JournalId
WHERE jl.AccountId=@Acc
AND je.EntryDate BETWEEN @From AND @To
ORDER BY je.EntryDate, je.JournalId", con))
                {
                    cmd.Parameters.AddWithValue("@Acc", accountId);
                    cmd.Parameters.AddWithValue("@From", fromDate);
                    cmd.Parameters.AddWithValue("@To", toDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                //---------------------------------
                // إضافة عمود الرصيد
                //---------------------------------

                dt.Columns.Add("Balance", typeof(decimal));

                decimal runningBalance = openingBalance;

                //---------------------------------
                // صف الرصيد الافتتاحي
                //---------------------------------

                DataRow rowOpen = dt.NewRow();

                rowOpen["EntryDate"] = fromDate;
                rowOpen["ReferenceType"] = "Opening";
                rowOpen["Description"] = "رصيد افتتاحي";

                rowOpen["Debit"] = 0;
                rowOpen["Credit"] = 0;
                rowOpen["Balance"] = openingBalance;

                dt.Rows.InsertAt(rowOpen, 0);

                //---------------------------------
                // الرصيد الجاري
                //---------------------------------

                foreach (DataRow row in dt.Rows.Cast<DataRow>().Skip(1))
                {
                    decimal debit = row["Debit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Debit"]);
                    decimal credit = row["Credit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Credit"]);

                    runningBalance += debit - credit;

                    row["Balance"] = runningBalance;
                }

                //---------------------------------

                gridControl1.DataSource = dt;

                lblBalance.Text = $"الرصيد الحالي: {runningBalance:N2}";
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadLedger();
           
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
                return;

            string refType =
                gridView1.GetRowCellValue(
                gridView1.FocusedRowHandle,
                "ReferenceType")?.ToString();

            int refId = Convert.ToInt32(
                gridView1.GetRowCellValue(
                gridView1.FocusedRowHandle,
                "ReferenceId"));

            if (refType == "Invoice")
                new frm_InvoiceEditor(refId).ShowDialog();

            if (refType == "SalesReturn")
                new frm_SalesReturn(refId).ShowDialog();

            if (refType == "Receipt")
                new frm_Receipt(refId).ShowDialog();
        }
    }
}
