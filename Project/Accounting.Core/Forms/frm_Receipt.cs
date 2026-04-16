using Accounting.Core.Enums;
using Accounting.Core.Models;
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
   

    public partial class frm_Receipt : Form
    {
        private string connectionString =
   @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
       
        private int _receiptId = 0;
        public frm_Receipt(int receiptId=0)
        {
          
        InitializeComponent();

            _receiptId = receiptId;
          

            customerService = new CustomerService(connectionString);
            supplierService = new SupplierService(connectionString);
            chequeService = new ChequeService(connectionString);
        }
        ReceiptService receiptService;

        CustomerService customerService;
        SupplierService supplierService;
        ChequeService chequeService;
        CustomerService cs;
        bool isSelecting = false;

        private void frm_Receipt_Load(object sender, EventArgs e)
        {
            cbxPartyType.Items.Clear();

            cbxPartyType.Items.Add("عميل");
            cbxPartyType.Items.Add("مورد");

          cbxPartyType.SelectedIndex = 0;

            //cs = new CustomerService(connectionString);
            //cbxParty.DataSource = cs.GetAllCustomers();
            //cbxParty.DisplayMember = "Name";
            //cbxParty.ValueMember = "CustomerId";
            CustomerService cs = new CustomerService(connectionString);

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

            dateReceipt.Value = DateTime.Now;
            //dateReceipt.Format = DateTimePickerFormat.Custom;
            //dateReceipt.CustomFormat = "d/M/yyyy";
            dateReceipt.Format = DateTimePickerFormat.Custom;
            dateReceipt.CustomFormat = "dd/MM/yyyy";

            LoadParties();

            //-------------------------------------------------
            // ⭐ إنشاء جريد الشيكات
            //-------------------------------------------------
            DataTable dt = new DataTable();

            dt.Columns.Add("ChequeId", typeof(int));   // مهم جداً
            dt.Columns.Add("ChequeNumber");
            dt.Columns.Add("BankName");
            dt.Columns.Add("DueDate", typeof(DateTime));
            dt.Columns.Add("ChequeAmount", typeof(decimal));

            gridCheques.DataSource = dt;

            gridViewCheques.OptionsView.NewItemRowPosition =
                DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;

            gridViewCheques.Columns["ChequeId"].Visible = false; // إخفاء العمود

            //-------------------------------------------------
            // ⭐ تحميل البيانات إذا كان تعديل
            //-------------------------------------------------
            if (_receiptId > 0)
                LoadReceipt(_receiptId);

            ReceiptService rs = new ReceiptService(connectionString);
            receiptService = new ReceiptService(connectionString);


        }

        private void LoadReceipt(int receiptId)
        {
            ReceiptService service = new ReceiptService(connectionString);

            Receipt r = service.GetReceiptById(receiptId);

            txtReceiptNumber.Text = r.ReceiptNumber;
            dateReceipt.Value = r.ReceiptDate;

            cbxPartyType.SelectedIndex = (int)r.PartyType - 1;
            LoadParties(); // مهم لإعادة تحميل القائمة

            cbxParty.SelectedValue = r.PartyId;

            txtCashAmount.Text = r.TotalAmount.ToString();
            txtNotes.Text = r.Notes;

            //-------------------------------------------------
            // ⭐ تعبئة الشيكات
            //-------------------------------------------------
            DataTable dt = (DataTable)gridCheques.DataSource;

            dt.Rows.Clear();

            foreach (var detail in r.Details)
            {
                foreach (var ch in detail.Cheques)
                {
                    dt.Rows.Add(
                        ch.ChequeId,
                        ch.ChequeNumber,
                        ch.BankName,
                        ch.DueDate,
                        ch.ChequeAmount
                    );
                }
            }
}


        private void PrepareChequeGrid()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ChequeNumber", typeof(string));
            dt.Columns.Add("BankName", typeof(string));
            dt.Columns.Add("DueDate", typeof(DateTime));
            dt.Columns.Add("ChequeAmount", typeof(decimal));

            gridCheques.DataSource = dt;

            gridViewCheques.PopulateColumns();

            gridViewCheques.Columns["ChequeNumber"].Caption = "رقم الشيك";
            gridViewCheques.Columns["BankName"].Caption = "اسم البنك";
            gridViewCheques.Columns["DueDate"].Caption = "تاريخ الاستحقاق";
            gridViewCheques.Columns["ChequeAmount"].Caption = "قيمة الشيك";

            // ⭐ السماح بإضافة صف جديد
            gridViewCheques.OptionsView.NewItemRowPosition =
                DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
        }

        private void LoadParties()
        {

            cbxParty.DataSource = null;

            if (cbxPartyType.SelectedIndex == 0)
            {
                cbxParty.DataSource = customerService.GetAllCustomers();
                cbxParty.DisplayMember = "Name";
                cbxParty.ValueMember = "CustomerId";
            }
            else
            {
                cbxParty.DataSource = supplierService.GetAllSuppliers();
                cbxParty.DisplayMember = "Name";
                cbxParty.ValueMember = "SupplierId";
            }
        }

        private void cbxPartyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadParties();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                // =========================
                // ✅ التحقق من رقم السند
                // =========================
                if (receiptService.ExistsReceiptNumber(txtReceiptNumber.Text))
                {
                    MessageBox.Show("رقم السند موجود مسبقاً");
                    return;
                }

                // =========================
                // 🧾 إنشاء السند
                // =========================
                Receipt receipt = new Receipt();
                receipt.Details = new List<ReceiptDetail>(); // 🔥 مهم جداً

                receipt.ReceiptNumber = txtReceiptNumber.Text;
                receipt.ReceiptDate = dateReceipt.Value;

                // =========================
                // 👤 تحديد نوع الطرف (حل نهائي)
                // =========================
                if (cbxPartyType.SelectedIndex == 0)
                    receipt.PartyType = PartyType.Customer;
                else if (cbxPartyType.SelectedIndex == 1)
                    receipt.PartyType = PartyType.Supplier;
                else
                {
                    MessageBox.Show("اختر نوع الطرف");
                    return;
                }

                // =========================
                // 👤 تحديد الطرف (عميل / مورد)
                // =========================
                if (cbxParty.SelectedValue is int id)
                {
                    receipt.PartyId = id;
                }
                else if (cbxParty.SelectedItem is DataRowView row)
                {
                    receipt.PartyId = Convert.ToInt32(row["CustomerId"]);
                }
                else
                {
                    MessageBox.Show("اختر عميل / مورد صحيح");
                    return;
                }

                receipt.Notes = txtNotes.Text;

                decimal total = 0;

                // =========================
                // 💰 الدفع النقدي
                // =========================
                if (!string.IsNullOrWhiteSpace(txtCashAmount.Text))
                {
                    decimal cashAmount = Convert.ToDecimal(txtCashAmount.Text);

                    if (cashAmount > 0)
                    {
                        ReceiptDetail cashDetail = new ReceiptDetail
                        {
                            PaymentMethod = PaymentMethod.Cash,
                            Amount = cashAmount
                        };

                        receipt.Details.Add(cashDetail);
                        total += cashAmount;
                    }
                }

                // =========================
                // 🧾 قراءة الشيكات
                // =========================
                if (gridCheques.DataSource != null)
                {
                    DataTable dt = (DataTable)gridCheques.DataSource;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["ChequeNumber"] == DBNull.Value)
                            continue;

                        Cheque cheque = new Cheque
                        {
                            ChequeNumber = row["ChequeNumber"].ToString(),
                            BankName = row["BankName"].ToString(),
                            DueDate = Convert.ToDateTime(row["DueDate"]),
                            ChequeAmount = Convert.ToDecimal(row["ChequeAmount"]),
                            Status = ChequeStatus.UnderCollection
                        };

                        ReceiptDetail detail = new ReceiptDetail
                        {
                            PaymentMethod = PaymentMethod.Cheque,
                            Amount = cheque.ChequeAmount
                        };

                        detail.Cheques.Add(cheque);
                        receipt.Details.Add(detail);

                        total += cheque.ChequeAmount;
                    }
                }

                // =========================
                // 💵 المجموع النهائي
                // =========================
                receipt.TotalAmount = total;

                // =========================
                // 💾 الحفظ
                // =========================
                int receiptId = receiptService.AddReceipt(receipt);
                // 🔥 الكاش (فقط النقدي)
                foreach (var d in receipt.Details)
                {
                    if (d.PaymentMethod == PaymentMethod.Cash)
                    {
                        AddCashTransaction(
                            receipt.ReceiptDate,
                            "سند قبض رقم " + receipt.ReceiptNumber,
                            d.Amount,   // Debit ✔️
                            0,
                            "Receipt",
                            receiptId
                        );
                    }
                }
                // =========================
                // 📊 إنشاء القيد المحاسبي
                // =========================
                JournalService js = new JournalService(connectionString);

                js.CreateReceiptEntry(
                    receiptId,
                    receipt.PartyId,
                    receipt.TotalAmount,
                    receipt.PartyType
                );

                MessageBox.Show("تم حفظ السند بنجاح");
                AppEvents.RefreshDashboard();
                this.Close();

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCollectCheque_Click(object sender, EventArgs e)
        {
            if (gridViewCheques.FocusedRowHandle < 0)
                return;

            int chequeId = Convert.ToInt32(
                gridViewCheques.GetFocusedRowCellValue("ChequeId"));

            frm_SelectAccount frm = new frm_SelectAccount();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                chequeService.CollectCheque(
                    chequeId,
                    frm.SelectedAccountId
                );

                MessageBox.Show("تم تحصيل الشيك");

                LoadReceipt(_receiptId);
            }

        }

        private void btnReturnCheque_Click(object sender, EventArgs e)
        {
            if (gridViewCheques.FocusedRowHandle < 0)
                return;

            int chequeId = Convert.ToInt32(
                gridViewCheques.GetFocusedRowCellValue("ChequeId")
            );

            chequeService.ReturnCheque(chequeId);

            MessageBox.Show("تم إرجاع الشيك");

            LoadReceipt(_receiptId);
        }

        private void txtReceiptNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxParty_TextChanged(object sender, EventArgs e)
        {
            if (isSelecting) return;

            if (cs == null)
                cs = new CustomerService(connectionString);

            string text = cbxParty.Text;

            if (text.Length < 2)
                return;

            isSelecting = true;

            var dt = cs.SearchCustomers(text);
            cbxParty.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                cbxParty.DataSource = dt;
                cbxParty.SelectedIndex = -1;
                cbxParty.DisplayMember = "Name";
                cbxParty.ValueMember = "CustomerId";

                cbxParty.DroppedDown = true;
                cbxParty.SelectionStart = cbxParty.Text.Length;
            }

            isSelecting = false;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
                return;

            if (!int.TryParse(searchLookUpEdit1.EditValue.ToString(), out int customerId))
                return;

            // 🔥 ربط العميل فقط
            cbxParty.SelectedValue = customerId;
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
