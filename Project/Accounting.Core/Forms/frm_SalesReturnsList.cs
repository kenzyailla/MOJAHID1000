using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Accounting.Core.Services;

namespace Accounting.Core.Forms
{
    

    public partial class frm_SalesReturnsList : Form
    {
        private InvoiceService _service;

        private string _connectionString =
    @"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        public frm_SalesReturnsList()
        {
            InitializeComponent();
            _service = new InvoiceService(_connectionString);
           
            LoadData();
        }

        private void frm_SalesReturnsList_Load(object sender, EventArgs e)
        {
            CustomizeGrid();

            CustomizeGridView(gridView1);
        }
        private void CustomizeGrid()
        {
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowGroupPanel = false;

            gridView1.Appearance.HeaderPanel.Font =
                new Font("Noto Kufi Arabic", 9F, FontStyle.Bold);

            gridView1.Appearance.Row.Font =
                new Font("Noto Kufi Arabic", 9F);

            gridView1.RowHeight = 30;
        }


        private void FormatGridColumns(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            gv.OptionsView.ColumnAutoWidth = false;

            // 🔹 رقم المرتجع
            SetColumn(gv, "SalesReturnId", "رقم المرتجع", 100);

            // 🔹 رقم الفاتورة
            SetColumn(gv, "OriginalInvoiceId", "رقم الفاتورة", 120);

            // 🔹 اسم العميل
            SetColumn(gv, "CustomerName", "العميل", 200);

            // 🔹 التاريخ
            if (gv.Columns["ReturnDate"] != null)
            {
                gv.Columns["ReturnDate"].Caption = "التاريخ";
                gv.Columns["ReturnDate"].Width = 120;
                gv.Columns["ReturnDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                gv.Columns["ReturnDate"].DisplayFormat.FormatString = "dd/MM/yyyy";
            }

            // 🔹 الإجمالي
            if (gv.Columns["TotalAfterTax"] != null)
            {
                gv.Columns["TotalAfterTax"].Caption = "الإجمالي";
                gv.Columns["TotalAfterTax"].Width = 140;
                gv.Columns["TotalAfterTax"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gv.Columns["TotalAfterTax"].DisplayFormat.FormatString = "N2";
            }

            // 🔹 الملاحظات
            SetColumn(gv, "Notes", "ملاحظات", 250);
        }
        private void SetColumn(DevExpress.XtraGrid.Views.Grid.GridView gv,
                      string name,
                      string caption,
                      int width)
        {
            if (gv.Columns[name] != null)
            {
                gv.Columns[name].Caption = caption;
                gv.Columns[name].Width = width;
            }
        }

        private void LoadData()
        {
            gridView1.OptionsView.ColumnAutoWidth = false;

            DataTable dt = _service.GetSalesReturns();
            gridControl1.DataSource = dt;
            FormatGridColumns(gridView1);
            if (gridView1.Columns["SalesReturnId"] != null)
            {
                gridView1.Columns["SalesReturnId"].Caption = "رقم المرتجع";
                gridView1.Columns["SalesReturnId"].Width = 100;
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

   
