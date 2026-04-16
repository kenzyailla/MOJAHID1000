using Accounting.Core.Models;
using Accounting.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{


    public partial class frm_Cheques : Form
    {
        private DataTable dtCustomers;
        private ChequeService service;
        private CustomerService cs;
        private bool isSelecting = false;
        public frm_Cheques()
        {
            InitializeComponent();
            chequeService = new ChequeService(connectionString);
        }
        private ChequeService chequeService;
        private string connectionString =
        @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";
     
      

        private void frm_Cheques_Load(object sender, EventArgs e)
        {
            service = new ChequeService(connectionString);
            gridView1.PopulateColumns();
           
            LoadCheques();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd/MM/yyyy";
            CustomizeGridView(gridView1);

            cs = new CustomerService(connectionString);
            dtCustomers = cs.GetAllCustomers();

            CbxCustomer.DataSource = dtCustomers;
            CbxCustomer.DisplayMember = "Name";
            CbxCustomer.ValueMember = "CustomerId";

        }
      

        private void LoadCheques()
        {
            gridControl1.DataSource = service.GetAllCheques();
            gridView1.PopulateColumns();

             }

        private void btnCollect_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRowCellValue("ChequeId") == null)
                return;

            int chequeId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("ChequeId"));

            frm_SelectAccount frm = new frm_SelectAccount();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                chequeService.CollectCheque(chequeId, frm.SelectedAccountId);

                MessageBox.Show("تم تحصيل الشيك بنجاح");

                LoadCheques();
            }


        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (gridView1.GetFocusedRowCellValue("ChequeId") == null)
                return;

            int chequeId = Convert.ToInt32(
                gridView1.GetFocusedRowCellValue("ChequeId"));

            service.UpdateChequeStatus(chequeId, 3); // Returned

            LoadCheques();
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

        private void txtChequeNo_TextChanged(object sender, EventArgs e)
        {
            CbxCustomer.SelectedIndex = -1;

            if (string.IsNullOrWhiteSpace(txtChequeNo.Text))
            {
                LoadCheques();
                return;
            }

            gridControl1.DataSource =
                service.SearchByChequeNumber(txtChequeNo.Text);
           
        }

     

        private void CbxCustomer_TextChanged(object sender, EventArgs e)
        {
            if (isSelecting) return;

            string text = CbxCustomer.Text;

            if (text.Length < 2)
                return;

            isSelecting = true;

            // 🔥 فلترة داخل نفس الداتا
            DataView dv = dtCustomers.DefaultView;
            dv.RowFilter = $"Name LIKE '%{text}%'";

            CbxCustomer.DroppedDown = true;
            CbxCustomer.SelectionStart = CbxCustomer.Text.Length;

            isSelecting = false;

            // 🔥 تحديث الجريد
            gridControl1.DataSource = service.SearchChequesByCustomerName(text);
           
        }
    }
}
