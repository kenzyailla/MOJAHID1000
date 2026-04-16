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
    public partial class frm_BuyInvoices : Form
    {
        BuyInvoiceService service;
        private string connectionString = @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_BuyInvoices()
        {
            InitializeComponent();
           

            service = new BuyInvoiceService(connectionString);

            this.Padding = new Padding(0);
            this.Margin = new Padding(0);

            //panel1Top.Dock = DockStyle.Top;
            //gridControl1.Dock = DockStyle.Fill;
          

        }

        private void frm_BuyInvoices_Load(object sender, EventArgs e)
        {
            gridControl1.MainView = gridView1;
            gridView1.GridControl = gridControl1;

            LoadBuyInvoices();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;

            // تعيين حجم النموذج
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Width = (int)(screenWidth * 0.7);
            this.Height = (int)(screenHeight * 0.75);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);
            CustomizeGridView(gridView1);
            SetupButtonsAppearance();
        }
        private void LoadBuyInvoices()
        {
            // أحياناً يكون هناك فلتر/بحث/عرض تفاصيل يمنع ظهور صف جديد
            gridView1.ActiveFilter.Clear();
            gridView1.ClearColumnsFilter();

            // اجلب بيانات جديدة تماماً
            var dt = service.GetAllBuyInvoices();

            // مهم جداً: فكّ الربط ثم أعده (يجبر DevExpress يعيد الرسم)
            gridControl1.BeginUpdate();
            try
            {
                gridControl1.DataSource = null;
                gridControl1.DataSource = dt;
                gridControl1.RefreshDataSource();
                gridView1.RefreshData();
            }
            finally
            {
                gridControl1.EndUpdate();
            }

            gridView1.PopulateColumns();
            if (gridView1.Columns["BuyInvoiceId"] != null)
                gridView1.Columns["BuyInvoiceId"].Visible = false;

            if (gridView1.Columns["InvoiceNumber"] != null)
                gridView1.Columns["InvoiceNumber"].Caption = "رقم الفاتورة";
            if (gridView1.Columns["SupplierName"] != null)
                gridView1.Columns["SupplierName"].Caption = "المورد";
            if (gridView1.Columns["InvoiceDate"] != null)
                gridView1.Columns["InvoiceDate"].Caption = "التاريخ";
            if (gridView1.Columns["TotalAfterTax"] != null)
                gridView1.Columns["TotalAfterTax"].Caption = "الإجمالي";
            if (gridView1.Columns["PaymentStatusName"] != null)
                gridView1.Columns["PaymentStatusName"].Caption = "حالة الدفع";

            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowGroupPanel = false;

            gridView1.BestFitColumns();



        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (frm_BuyInvoiceEditor frm = new frm_BuyInvoiceEditor(null))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        LoadBuyInvoices();
                    }));
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var row = gridView1.GetFocusedDataRow();
            if (row == null) return;

            int id = Convert.ToInt32(row["BuyInvoiceId"]);

            using (frm_BuyInvoiceEditor frm = new frm_BuyInvoiceEditor(id))
            {
                frm.ShowDialog();
            }

            LoadBuyInvoices();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(sender, e);
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

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
            gridView.Appearance.HeaderPanel.Font = new Font("Noto Kufi Arabic", 10.18868f, FontStyle.Bold);
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView.Appearance.HeaderPanel.BackColor = Color.LightGray;

            // Cell appearance
            gridView.Appearance.Row.Font = new Font("Noto Kufi Arabic", 10.18868f);
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

            // Disable row headers and group panel (already set elsewhere)
            // gridView.OptionsView.ShowGroupPanel = false;
            // gridView.OptionsView.RowHeadersWidth = 0; // Optional
        }
        private void SetupButtonsAppearance()
        {
            // تخصيص زر جديد
            btnAdd.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnAdd.Appearance.ForeColor = Color.White;
            btnAdd.Appearance.BackColor = Color.FromArgb(41, 128, 185); // أزرق غامق
            btnAdd.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnAdd.Cursor = Cursors.Hand;

            // زر تعديل
            btnEdit.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnEdit.Appearance.ForeColor = Color.White;
            btnEdit.Appearance.BackColor = Color.FromArgb(39, 174, 96); // أخضر
            btnEdit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnEdit.Cursor = Cursors.Hand;


            btnDelete.Appearance.Font = new Font("Noto Kufi Arabic", 10, FontStyle.Bold);
            btnDelete.Appearance.ForeColor = Color.White;
            btnDelete.Appearance.BackColor = Color.FromArgb(192, 57, 43); // أحمر
            btnDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            btnDelete.Cursor = Cursors.Hand;
        }
        }
}
