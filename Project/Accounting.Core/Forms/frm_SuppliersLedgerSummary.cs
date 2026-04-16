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
    public partial class frm_SuppliersLedgerSummary : Form
    {

        public frm_SuppliersLedgerSummary()
        {
            InitializeComponent();
            
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

      
        private void frm_SuppliersLedgerSummary_Load(object sender, EventArgs e)
        {
            LoadSuppliersSummary();
            CustomizeGridView(gridView1);

            dtFrom.Format = DateTimePickerFormat.Custom;
            dtFrom.CustomFormat = "dd/MM/yyyy";

            dtTo.Format = DateTimePickerFormat.Custom;
            dtTo.CustomFormat = "dd/MM/yyyy";

        }

        private void LoadSuppliersSummary()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                DateTime fromDate = dtFrom.Value.Date;
                DateTime toDate = dtTo.Value.Date;
                string sql = @"
SELECT 
    s.SupplierId,
    s.Name,

    ISNULL(SUM(CASE WHEN je.EntryDate < @From 
                    THEN jl.Debit - jl.Credit END),0) AS OpeningBalance,

    ISNULL(SUM(CASE WHEN je.EntryDate BETWEEN @From AND @To 
                    THEN jl.Debit END),0) AS PeriodDebit,

    ISNULL(SUM(CASE WHEN je.EntryDate BETWEEN @From AND @To 
                    THEN jl.Credit END),0) AS PeriodCredit,

    ISNULL(SUM(jl.Debit - jl.Credit),0) AS ClosingBalance

FROM Suppliers s
LEFT JOIN JournalLines jl ON jl.AccountId = s.AccountId
LEFT JOIN JournalEntries je ON jl.JournalId = je.JournalId

WHERE s.IsActive = 1
GROUP BY s.SupplierId, s.Name

HAVING ISNULL(SUM(jl.Debit - jl.Credit),0) <> 0

ORDER BY s.Name
";


                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@From", fromDate);
                cmd.Parameters.AddWithValue("@To", toDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //-----------------------------------
                // إضافة الرصيد النهائي
                //-----------------------------------
                if (!dt.Columns.Contains("ClosingBalance"))
                    dt.Columns.Add("ClosingBalance", typeof(decimal));

                foreach (DataRow row in dt.Rows)
                {
                    decimal opening = Convert.ToDecimal(row["OpeningBalance"]);
                    decimal debit = Convert.ToDecimal(row["PeriodDebit"]);
                    decimal credit = Convert.ToDecimal(row["PeriodCredit"]);

                    row["ClosingBalance"] = opening + debit - credit;
                }

                gridControl1.DataSource = dt;

                gridView1.Columns["OpeningBalance"].Caption = "رصيد قبل الفترة";
                gridView1.Columns["PeriodDebit"].Caption = "مدين الفترة";
                gridView1.Columns["PeriodCredit"].Caption = "دائن الفترة";
                gridView1.Columns["ClosingBalance"].Caption = "الرصيد النهائي";
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
    }
}
