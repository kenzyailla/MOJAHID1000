using Accounting.Core.Models;
using Accounting.Core.Repositories;
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
    public partial class frm_BuyPayment : Form
    {
        private SupplierService supplierService;
        private int? _editPaymentId = null;
        private int? _paymentId = null;
        private BuyPaymentService service;
        private string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
        private DataTable dtCheques;
        private int SelectedSupplierId = 0;
        public frm_BuyPayment(int paymentId) : this()
        {
            _editPaymentId = paymentId;
            _paymentId = paymentId;

        }

        public frm_BuyPayment()
        {
            InitializeComponent();
           service = new BuyPaymentService(connectionString);
          

        }
        SupplierService cs;
        private void frm_BuyPayment_Load(object sender, EventArgs e)
        {
            cbxPaymentMethod.Items.Add("Cash");
            cbxPaymentMethod.Items.Add("Bank");
            cbxPaymentMethod.SelectedIndex = 0;
            gridView1.ValidateRow += gridView1_ValidateRow;
            gridView1.OptionsBehavior.EditorShowMode =
    DevExpress.Utils.EditorShowMode.Click;


            LoadSuppliers();
            // جدول الشيكات
            dtCheques = new DataTable();
            dtCheques.Columns.Add("ChequeNumber", typeof(string));
            dtCheques.Columns.Add("BankName", typeof(string));
            dtCheques.Columns.Add("DueDate", typeof(DateTime));
            dtCheques.Columns.Add("Amount", typeof(decimal));

            // اربطه بجريد للشيكات (اعمل GridControl ثاني باسم gridControlCheques و GridView اسمه gridViewCheques)
            gridControlCheques.DataSource = dtCheques;
            gridViewCheques.PopulateColumns();

            gridViewCheques.Columns["ChequeNumber"].Caption = "رقم الشيك";
            gridViewCheques.Columns["BankName"].Caption = "البنك";
            gridViewCheques.Columns["DueDate"].Caption = "تاريخ الاستحقاق";
            gridViewCheques.Columns["Amount"].Caption = "المبلغ";
            if (_editPaymentId.HasValue)
            {
                LoadPaymentForEdit(_editPaymentId.Value);
            }
            if (_paymentId.HasValue)
            {
                LoadPaymentForEdit(_paymentId.Value);
            }
            CustomizeGridView(gridView1);
            cs = new SupplierService(connectionString);
            // ================= الموردين =================
            var suppliers = cs.GetAllSuppliers(); // ✔ الآن يعمل

            //searchLookUpEdit1.Properties.DataSource = suppliers;
            //searchLookUpEdit1.Properties.DisplayMember = "Name";
            //searchLookUpEdit1.Properties.ValueMember = "SupplierId";

            //searchLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            //searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            //searchLookUpEdit1.Properties.View.PopulateColumns();
            //searchLookUpEdit1.Properties.View.Columns["SupplierId"].Visible = false;
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

        }
        private void LoadPaymentForEdit(int paymentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // الرأس
                // الرأس
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM BuyPayments WHERE BuyPaymentId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", paymentId);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            SelectedSupplierId = Convert.ToInt32(dr["SupplierId"]);
                            cbxSupplier.SelectedValue = SelectedSupplierId;

                            // =================== قراءة Notes ===================
                            string notes = dr["Notes"]?.ToString() ?? "";

                            decimal cashAmount = 0;

                            if (notes.Contains("Cash="))
                            {
                                var part = notes.Split(',')
                                                .FirstOrDefault(x => x.Trim().StartsWith("Cash="));

                                if (part != null)
                                {
                                    var value = part.Replace("Cash=", "").Trim();
                                    decimal.TryParse(value, out cashAmount);
                                }
                            }

                            txtCashAmount.Text = cashAmount.ToString("0.00");

                            // تحديد طريقة الدفع
                            if (notes.Contains("CashMethod="))
                            {
                                var part = notes.Split(',')
                                                .FirstOrDefault(x => x.Trim().StartsWith("CashMethod="));

                                if (part != null)
                                {
                                    var methodValue = part.Replace("CashMethod=", "").Trim();
                                    cbxPaymentMethod.Text = methodValue;
                                }
                            }
                        }
                    }
                }
                // توزيع الفواتير
                DataTable dtInvoices = service.GetUnpaidInvoices(SelectedSupplierId);
                
                // ✅ لازم نضيف العمود هنا
                if (!dtInvoices.Columns.Contains("AmountToPay"))
                    dtInvoices.Columns.Add("AmountToPay", typeof(decimal));
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT BuyInvoiceId, PaidAmount FROM BuyPaymentInvoices WHERE BuyPaymentId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", paymentId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtPaid = new DataTable();
                    da.Fill(dtPaid);
                 
                    foreach (DataRow row in dtPaid.Rows)
                    {
                        int invId = Convert.ToInt32(row["BuyInvoiceId"]);
                        decimal paid = Convert.ToDecimal(row["PaidAmount"]);

                        var target = dtInvoices.AsEnumerable()
                            .FirstOrDefault(r => Convert.ToInt32(r["BuyInvoiceId"]) == invId);

                        if (target != null)
                            target["AmountToPay"] = paid;
                    }
                }

                gridControl1.DataSource = dtInvoices;
                gridView1.PopulateColumns();

                gridView1.Columns["AmountToPay"].Caption = "المبلغ المراد دفعه";
                gridView1.Columns["AmountToPay"].OptionsColumn.AllowEdit = true;

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView1.Columns)
                    col.OptionsColumn.AllowEdit = (col.FieldName == "AmountToPay");
                // الشيكات
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ChequeNumber, BankName, DueDate, Amount FROM OutgoingCheques WHERE BuyPaymentId=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", paymentId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    dtCheques.Clear();
                    da.Fill(dtCheques);
                }
            }
        }

        private void LoadSuppliers()
        {
            SupplierService service = new SupplierService(connectionString);
            cbxSupplier.DataSource = service.GetAllSuppliers();
            cbxSupplier.DisplayMember = "Name";
            cbxSupplier.ValueMember = "SupplierId";
        }

        private void cbxSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbxSupplier.SelectedValue == null) return;

            // حل DataRowView
            if (cbxSupplier.SelectedValue is DataRowView) return;

            if (!int.TryParse(cbxSupplier.SelectedValue.ToString(), out int supplierId))
                return;

            SelectedSupplierId = supplierId;

            DataTable dt = service.GetUnpaidInvoices(supplierId);
            if (!dt.Columns.Contains("AmountToPay"))
                dt.Columns.Add("AmountToPay", typeof(decimal));

            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();

            gridView1.Columns["AmountToPay"].Caption = "المبلغ المراد دفعه";
            gridView1.Columns["AmountToPay"].OptionsColumn.AllowEdit = true;

            // اقفل باقي الأعمدة
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView1.Columns)
                col.OptionsColumn.AllowEdit = (col.FieldName == "AmountToPay");

        }

        private void gridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            decimal remaining = 0;
            decimal amountToPay = 0;

            if (view.GetRowCellValue(e.RowHandle, "Remaining") != DBNull.Value)
                remaining = Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, "Remaining"));

            if (view.GetRowCellValue(e.RowHandle, "AmountToPay") != DBNull.Value)
                amountToPay = Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, "AmountToPay"));

            if (amountToPay < 0)
            {
                e.Valid = false;
                view.SetColumnError(view.Columns["AmountToPay"], "لا يمكن إدخال مبلغ سالب");
                return;
            }

            if (amountToPay > remaining)
            {
                e.Valid = false;
                view.SetColumnError(view.Columns["AmountToPay"], "المبلغ أكبر من المتبقي");
            }
        }

        private void btnSavePayment_Click(object sender, EventArgs e)
        {
            gridView1.PostEditor();
            gridView1.UpdateCurrentRow();

            gridViewCheques.PostEditor();
            gridViewCheques.UpdateCurrentRow();
            if (_editPaymentId.HasValue)
            {
                service.DeletePayment(_editPaymentId.Value);
            }


            if (SelectedSupplierId <= 0)
            {
                MessageBox.Show("اختر المورد أولاً");
                return;
            }

            // فواتير
            DataTable dtInvoices = gridControl1.DataSource as DataTable;
            if (dtInvoices == null || dtInvoices.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد فواتير للسداد");
                return;
            }

            // اجمالي التوزيع على الفواتير
            decimal totalToPay = 0;
            foreach (DataRow r in dtInvoices.Rows)
            {
                if (r["AmountToPay"] == DBNull.Value) continue;
                decimal a = Convert.ToDecimal(r["AmountToPay"]);
                if (a > 0) totalToPay += a;
            }

            if (totalToPay <= 0)
            {
                MessageBox.Show("لم يتم إدخال أي مبلغ للسداد داخل الجريد");
                return;
            }

            // كاش
            decimal cashAmount = 0;
            decimal.TryParse((txtCashAmount.Text ?? "0").Trim(), out cashAmount);
            if (cashAmount < 0) cashAmount = 0;

            // شيكات
            decimal chequesTotal = 0;
            foreach (DataRow r in dtCheques.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["Amount"] == DBNull.Value) continue;

                decimal a = Convert.ToDecimal(r["Amount"]);
                if (a > 0) chequesTotal += a;
            }

            // تحقق: كاش + شيكات = اجمالي الموزع
            if (Math.Abs((cashAmount + chequesTotal) - totalToPay) > 0.01m)
            {
                MessageBox.Show($"مجموع الدفع لا يساوي الموزع على الفواتير.\nالموزع: {totalToPay}\nكاش+شيكات: {cashAmount + chequesTotal}");
                return;
            }
            string method;

            // تحديد الطريقة تلقائيًا
            if (cashAmount > 0 && chequesTotal > 0)
            {
                method = "Mixed";
            }
            else if (chequesTotal > 0)
            {
                method = "Cheque";
            }
            else if (cashAmount > 0)
            {
                method = cbxPaymentMethod.Text; // Cash أو Bank
            }
            else
            {
                MessageBox.Show("يجب إدخال كاش أو شيكات على الأقل");
                return;
            }

            // 👇 أضف هذا السطر
            string cashMethod = cbxPaymentMethod.Text;

            // احفظ
           
            int paymentId = service.SavePaymentMixed(
    supplierId: SelectedSupplierId,
    paymentMethod: method,
    cashMethod: cashMethod,
    cashAmount: cashAmount,
    dtInvoices: dtInvoices,
    dtCheques: dtCheques
);
            // 🔥 تسجيل في الصندوق (فقط إذا فيه كاش)
            if (cashAmount > 0 && cashMethod == "Cash")
            {
                AddCashTransaction(
                    DateTime.Now,
                    "سند صرف رقم " + paymentId,
                    0,
                    cashAmount,
                    "Payment",
                    paymentId   // 🔥 بدل 0
                );
            }
            MessageBox.Show("تم حفظ سند الصرف بنجاح");
            AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
            this.Close();
           

        }

        private void btnAddCheque_Click(object sender, EventArgs e)
        {
            gridViewCheques.InitNewRow += (s, eArgs) =>
            {
                gridViewCheques.SetRowCellValue(eArgs.RowHandle, "Amount", 0);
                gridViewCheques.SetRowCellValue(eArgs.RowHandle, "DueDate", DateTime.Today);
            };


            DataRow row = dtCheques.NewRow();

            row["ChequeNumber"] = "";
            row["BankName"] = "";
            row["DueDate"] = DateTime.Today;
            row["Amount"] = 0;

            dtCheques.Rows.Add(row);
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

        private void cbxSupplier_TextChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int supplierId))
                return;

            // 🔥 ربط المورد مع الكمبو
            cbxSupplier.SelectedValue = supplierId;

            // 🔥 أو شغل البحث مباشرة
            //btnSearch.PerformClick();
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
