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
using Accounting.Core.Reportingcrystal;
using static Accounting.Core.Forms.frm_CustomerStatement;

namespace Accounting.Core.Forms
{
    public partial class frm_Expenses : DevExpress.XtraEditors.XtraForm
    {
        public frm_Expenses()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private void LoadExpenses(DateTime from, DateTime to, string category)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
        SELECT * FROM Expenses
        WHERE ExpenseDate >= @from
        AND ExpenseDate <= @to";

                if (category != "الكل")
                    sql += " AND Category = @cat";

                sql += " ORDER BY ExpenseDate DESC";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);

                if (category != "الكل")
                    cmd.Parameters.AddWithValue("@cat", category);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = dt;
               
            }
          
        }
        private void frm_Expenses_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd/MM/yyyy";

            LoadExpenses(
          dateTimePicker1.Value.Date,
          dateTimePicker2.Value.Date,
          "الكل"
      );
            gridView1.Columns["ExpenseId"].Visible = false;

            gridView1.Columns["ExpenseDate"].Caption = "التاريخ";
            gridView1.Columns["Description"].Caption = "الوصف";
            gridView1.Columns["Amount"].Caption = "المبلغ";
            gridView1.Columns["Category"].Caption = "نوع المصروف";

            gridView1.Columns["Amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["Amount"].DisplayFormat.FormatString = "n2";

            gridView1.Columns["ExpenseDate"].VisibleIndex = 0;
            gridView1.Columns["Description"].VisibleIndex = 1;
            gridView1.Columns["Amount"].VisibleIndex = 2;
            gridView1.Columns["Category"].VisibleIndex = 3;

            CustomizeGridView(gridView1);
            LoadCategories();

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadExpenses(
        dateTimePicker1.Value.Date,
        dateTimePicker2.Value.Date,
        "الكل"
    );
            gridView1.Columns["Amount"].Summary.Clear();

            gridView1.Columns["Amount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum,
                "Amount",
                "الإجمالي: {0:n2}"
            );
            UpdateTotalExpenses();
          
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Amount")
            {
                decimal val = Convert.ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "Amount"));

                if (val > 1000)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
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
        private void LoadExpenses(DateTime from, DateTime to)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
        SELECT * FROM Expenses
        WHERE ExpenseDate >= @from
        AND ExpenseDate <= @to
        ORDER BY ExpenseDate DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridControl1.DataSource = dt;
                
            }
            UpdateTotalExpenses();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadExpenses(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
        }
        private void LoadCategories()
{
    using (SqlConnection con = new SqlConnection(connectionString))
    {
        con.Open();

        string sql = "SELECT DISTINCT Category FROM Expenses";

        SqlDataAdapter da = new SqlDataAdapter(sql, con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        // إضافة خيار "الكل"
        DataRow row = dt.NewRow();
        row["Category"] = "الكل";
        dt.Rows.InsertAt(row, 0);

        cbxFilterCategory.DataSource = dt;
        cbxFilterCategory.DisplayMember = "Category";
    }
}

        private void cbxFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpenses(
       dateTimePicker1.Value.Date,
       dateTimePicker2.Value.Date,
       cbxFilterCategory.Text
   );
            UpdateTotalExpenses();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (gridView1.FocusedRowHandle < 0)
            {
                MessageBox.Show("اختر مصروف");
                return;
            }

            string selectedCategory = gridView1.GetFocusedRowCellValue("Category").ToString();

            DataTable reportTable = new DataTable();
            reportTable.Columns.Add("ExpenseDate", typeof(DateTime));
            reportTable.Columns.Add("Description", typeof(string));
            reportTable.Columns.Add("Amount", typeof(decimal));
            reportTable.Columns.Add("Category", typeof(string));
            reportTable.Columns.Add("PaymentType", typeof(string));

            decimal total = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                int rowHandle = gridView1.GetVisibleRowHandle(i);
                if (rowHandle < 0) continue;

                string category = gridView1.GetRowCellValue(rowHandle, "Category")?.ToString();
                if (category != selectedCategory) continue;

                decimal rowAmount = Convert.ToDecimal(gridView1.GetRowCellValue(rowHandle, "Amount"));
                total += rowAmount;

                reportTable.Rows.Add(
                    Convert.ToDateTime(gridView1.GetRowCellValue(rowHandle, "ExpenseDate")),
                    gridView1.GetRowCellValue(rowHandle, "Description")?.ToString(),
                    rowAmount,
                    category,
                    gridView1.GetRowCellValue(rowHandle, "PaymentType")?.ToString()
                );
            }

            string tafqeet = NumberConverter.ConvertToArabicWords(total);

            rptExpenseVoucher rpt = new rptExpenseVoucher();
            rpt.SetDataSource(reportTable);
            rpt.SetParameterValue("PTotal", total);
            rpt.SetParameterValue("PTafqeet", tafqeet);

            frm_ReportViewer frm = new frm_ReportViewer(rpt);
            frm.ShowDialog();
        }
        private void UpdateTotalExpenses()
        {
            decimal total = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                int rowHandle = gridView1.GetVisibleRowHandle(i);
                if (rowHandle < 0) continue;

                object val = gridView1.GetRowCellValue(rowHandle, "Amount");

                if (val != null && val != DBNull.Value)
                    total += Convert.ToDecimal(val);
            }

            lblTotalExpenses.Text = "مجموع المصاريف: " + total.ToString("N2");
        }
    }
}