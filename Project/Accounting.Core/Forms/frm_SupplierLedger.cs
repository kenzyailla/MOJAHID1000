using Accounting.Core.Services;
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
    public partial class frm_SupplierLedger : Form
    {
        public frm_SupplierLedger()
        {
            InitializeComponent();
        }
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        SupplierService cs;
        private void frm_SupplierLedger_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            gridView1.OptionsBehavior.Editable = false;

            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;

            gridView1.FocusRectStyle =
            DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            CustomizeGridView(gridView1);

            cs = new SupplierService(connectionString);
            // ================= الموردين =================
            var suppliers = cs.GetAllSuppliers(); // ✔ الآن يعمل

            
            searchLookUpEdit1.Properties.DataSource = suppliers;
            searchLookUpEdit1.Properties.DisplayMember = "Name";
            searchLookUpEdit1.Properties.ValueMember = "SupplierId";

            searchLookUpEdit1.Properties.NullText = "";

            // 🔥 تنظيف الأعمدة
            searchLookUpEdit1.Properties.View.Columns.Clear();
            searchLookUpEdit1.Properties.View.Columns.AddVisible("Name", "اسم المورد");

            // 🔥 تحسين العرض
            searchLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            searchLookUpEdit1.Properties.View.BestFitColumns();

            // 🔥 الخط
            searchLookUpEdit1.Properties.View.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Regular);

            searchLookUpEdit1.Properties.View.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9, FontStyle.Bold);

            searchLookUpEdit1.Properties.Appearance.Font =
                new Font("Noto Kufi Arabic", 10, FontStyle.Regular);

            // 🔥 البحث
            searchLookUpEdit1.Properties.PopupFilterMode =
                DevExpress.XtraEditors.PopupFilterMode.Contains;

            searchLookUpEdit1.Properties.ImmediatePopup = true;

            // 🔥 حجم الصف
            searchLookUpEdit1.Properties.View.RowHeight = 28;
            dtFrom.Format = DateTimePickerFormat.Custom;
            dtFrom.CustomFormat = "dd/MM/yyyy";
            dtTo.Format = DateTimePickerFormat.Custom;
            dtTo.CustomFormat = "dd/MM/yyyy";

        }
        private void LoadSuppliers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT SupplierId, Name FROM Suppliers WHERE IsActive=1", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxSupplier.DataSource = dt;
                cbxSupplier.DisplayMember = "Name";
                cbxSupplier.ValueMember = "SupplierId";
            }
        }
        private void LoadLedger()
        {
            if (cbxSupplier.SelectedValue == null) return;

            int supplierId = Convert.ToInt32(cbxSupplier.SelectedValue);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                int accountId = 0;

                // 1️⃣ جلب AccountId للمورد
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", supplierId);
                    accountId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                DateTime fromDate = dtFrom.Value.Date;
                DateTime toDate = dtTo.Value.Date;

                //------------------------------------------
                // 2️⃣ حساب الرصيد قبل الفترة (Opening Balance)
                //------------------------------------------
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

                //------------------------------------------
                // 3️⃣ جلب الحركات داخل الفترة
                //------------------------------------------
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
            JOIN JournalLines jl ON je.JournalId = jl.JournalId
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

                //------------------------------------------
                // 4️⃣ إضافة عمود الرصيد الجاري
                //------------------------------------------
                dt.Columns.Add("Balance", typeof(decimal));

                decimal runningBalance = openingBalance;

                //------------------------------------------
                // 5️⃣ إضافة صف الرصيد الافتتاحي
                //------------------------------------------
                DataRow openingRow = dt.NewRow();
                openingRow["EntryDate"] = fromDate;
                openingRow["ReferenceType"] = "Opening";
                openingRow["Description"] = "رصيد افتتاحي";
                openingRow["Debit"] = 0;
                openingRow["Credit"] = 0;
                openingRow["Balance"] = openingBalance;

                dt.Rows.InsertAt(openingRow, 0);

                //------------------------------------------
                // 6️⃣ حساب الرصيد الجاري
                //------------------------------------------
                foreach (DataRow row in dt.Rows.Cast<DataRow>().Skip(1))
                {
                    decimal debit = row["Debit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Debit"]);
                    decimal credit = row["Credit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Credit"]);

                    runningBalance += debit - credit;

                    row["Balance"] = runningBalance;
                }

                //------------------------------------------
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
           
            if (gridView1.FocusedRowHandle < 0) return;

            string refType = gridView1.GetRowCellValue(
                gridView1.FocusedRowHandle,
                "ReferenceType")?.ToString();

            int refId = Convert.ToInt32(
                gridView1.GetRowCellValue(
                gridView1.FocusedRowHandle,
                "ReferenceId"));

            if (refType == "BuyInvoice")
                new frm_BuyInvoiceEditor(refId).ShowDialog();

            if (refType == "BuyPayment")
                new frm_BuyPayment(refId).ShowDialog();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.RowHandle < 0)
                return;

            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            // لو السطر فاتورة
            if (view.Columns["BuyInvoiceId"] != null &&
                view.GetRowCellValue(e.RowHandle, "BuyInvoiceId") != null)
            {
                int invoiceId = Convert.ToInt32(
                    view.GetRowCellValue(e.RowHandle, "BuyInvoiceId"));

                frm_BuyInvoiceEditor frm =
                    new frm_BuyInvoiceEditor(invoiceId);

                frm.ShowDialog();
                return;
            }

            // لو السطر سند صرف
            if (view.Columns["BuyPaymentId"] != null &&
                view.GetRowCellValue(e.RowHandle, "BuyPaymentId") != null)
            {
                int paymentId = Convert.ToInt32(
                    view.GetRowCellValue(e.RowHandle, "BuyPaymentId"));

                frm_BuyPayment frm =
                    new frm_BuyPayment(paymentId);

                frm.ShowDialog();
                return;
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

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int supplierId))
                return;

            // 🔥 ربط المورد مع الكمبو
            cbxSupplier.SelectedValue = supplierId;

             //🔥 أو شغل البحث مباشرة
            btnLoad.PerformClick();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
