using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using Accounting.Core.Services;
namespace Accounting.Core.Forms
{
    public partial class frm_SelectAccount : Form
    {
        string connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        //public int SelectedAccountId { get; private set; }
        public int SelectedAccountId { get; set; }


        public frm_SelectAccount()
        {
            InitializeComponent();
        }

        private void frm_SelectAccount_Load(object sender, EventArgs e)
        {
            LoadAccounts();

            //gridViewAccounts.OptionsBehavior.Editable = false;
            //gridViewAccounts.OptionsSelection.EnableAppearanceFocusedCell = false;
            //gridViewAccounts.FocusRectStyle =
            //    DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;

            AccountService service = new AccountService(connectionString);

            gridAccounts.DataSource = service.GetAllAccounts();

            gridViewAccounts.PopulateColumns();

            gridViewAccounts.Columns["AccountId"].Visible = false;
            gridViewAccounts.Columns["AccountCode"].Caption = "رقم الحساب";
            gridViewAccounts.Columns["AccountName"].Caption = "اسم الحساب";
            gridViewAccounts.Columns["AccountType"].Caption = "نوع الحساب";

        }

        private void LoadAccounts()
        {
            AccountService service = new AccountService(connectionString);

            gridAccounts.DataSource = service.GetCashAndBankAccounts();

            gridViewAccounts.PopulateColumns();

            gridViewAccounts.Columns["AccountId"].Visible = false;
            gridViewAccounts.Columns["AccountCode"].Caption = "رقم الحساب";
            gridViewAccounts.Columns["AccountName"].Caption = "اسم الحساب";
        }

        private void gridViewAccounts_DoubleClick(object sender, EventArgs e)
        {
            if (gridViewAccounts.FocusedRowHandle < 0)
                return;

            SelectedAccountId = Convert.ToInt32(
                gridViewAccounts.GetFocusedRowCellValue("AccountId")
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (gridViewAccounts.FocusedRowHandle < 0)
                return;

            SelectedAccountId = Convert.ToInt32(
                gridViewAccounts.GetFocusedRowCellValue("AccountId")
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (gridViewAccounts.FocusedRowHandle < 0)
                return;

            SelectedAccountId = Convert.ToInt32(
                gridViewAccounts.GetFocusedRowCellValue("AccountId"));

            DialogResult = DialogResult.OK;
        }
    }
}
