namespace Accounting.Core.Forms
{
    partial class frm_NewInvoice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchLookUpEdit1 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.dateEdit1 = new System.Windows.Forms.DateTimePicker();
            this.cbxPaymentType = new System.Windows.Forms.ComboBox();
            this.cbxCustomer = new System.Windows.Forms.ComboBox();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.cbxInvoiceTypee = new System.Windows.Forms.ComboBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.gridControlLines = new DevExpress.XtraGrid.GridControl();
            this.GridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.txt_Total = new System.Windows.Forms.TextBox();
            this.txt_tax = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.searchLookUpEdit1);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.dateEdit1);
            this.panel1.Controls.Add(this.cbxPaymentType);
            this.panel1.Controls.Add(this.cbxCustomer);
            this.panel1.Controls.Add(this.txtInvoiceNumber);
            this.panel1.Controls.Add(this.cbxInvoiceTypee);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1107, 124);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // searchLookUpEdit1
            // 
            this.searchLookUpEdit1.Location = new System.Drawing.Point(830, 57);
            this.searchLookUpEdit1.Name = "searchLookUpEdit1";
            this.searchLookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEdit1.Properties.PopupView = this.searchLookUpEdit1View;
            this.searchLookUpEdit1.Size = new System.Drawing.Size(227, 22);
            this.searchLookUpEdit1.TabIndex = 6;
            this.searchLookUpEdit1.EditValueChanged += new System.EventHandler(this.searchLookUpEdit1_EditValueChanged);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(65, 65);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 36);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dateEdit1
            // 
            this.dateEdit1.Location = new System.Drawing.Point(65, 23);
            this.dateEdit1.Name = "dateEdit1";
            this.dateEdit1.Size = new System.Drawing.Size(200, 36);
            this.dateEdit1.TabIndex = 4;
            // 
            // cbxPaymentType
            // 
            this.cbxPaymentType.FormattingEnabled = true;
            this.cbxPaymentType.Location = new System.Drawing.Point(462, 60);
            this.cbxPaymentType.Name = "cbxPaymentType";
            this.cbxPaymentType.Size = new System.Drawing.Size(153, 41);
            this.cbxPaymentType.TabIndex = 2;
            this.cbxPaymentType.SelectedIndexChanged += new System.EventHandler(this.cbxInvoiceType_SelectedIndexChanged);
            // 
            // cbxCustomer
            // 
            this.cbxCustomer.FormattingEnabled = true;
            this.cbxCustomer.Location = new System.Drawing.Point(621, 60);
            this.cbxCustomer.Name = "cbxCustomer";
            this.cbxCustomer.Size = new System.Drawing.Size(174, 41);
            this.cbxCustomer.TabIndex = 3;
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Location = new System.Drawing.Point(907, 10);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(178, 36);
            this.txtInvoiceNumber.TabIndex = 1;
            // 
            // cbxInvoiceTypee
            // 
            this.cbxInvoiceTypee.FormattingEnabled = true;
            this.cbxInvoiceTypee.Location = new System.Drawing.Point(291, 60);
            this.cbxInvoiceTypee.Name = "cbxInvoiceTypee";
            this.cbxInvoiceTypee.Size = new System.Drawing.Size(165, 41);
            this.cbxInvoiceTypee.TabIndex = 1;
            this.cbxInvoiceTypee.SelectedIndexChanged += new System.EventHandler(this.cbxInvoiceTypee_SelectedIndexChanged);
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(698, 702);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(171, 36);
            this.txtTotal.TabIndex = 2;
            // 
            // gridControlLines
            // 
            this.gridControlLines.Location = new System.Drawing.Point(23, 132);
            this.gridControlLines.MainView = this.GridView;
            this.gridControlLines.Name = "gridControlLines";
            this.gridControlLines.Size = new System.Drawing.Size(1085, 622);
            this.gridControlLines.TabIndex = 1;
            this.gridControlLines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GridView});
            // 
            // GridView
            // 
            this.GridView.GridControl = this.gridControlLines;
            this.GridView.Name = "GridView";
            this.GridView.OptionsView.ShowGroupPanel = false;
            this.GridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.GridView_CellValueChanged);
            // 
            // txtSubTotal
            // 
            this.txtSubTotal.Location = new System.Drawing.Point(521, 702);
            this.txtSubTotal.Name = "txtSubTotal";
            this.txtSubTotal.Size = new System.Drawing.Size(171, 36);
            this.txtSubTotal.TabIndex = 3;
            // 
            // txt_Total
            // 
            this.txt_Total.Location = new System.Drawing.Point(875, 702);
            this.txt_Total.Name = "txt_Total";
            this.txt_Total.Size = new System.Drawing.Size(171, 36);
            this.txt_Total.TabIndex = 5;
            // 
            // txt_tax
            // 
            this.txt_tax.Location = new System.Drawing.Point(698, 702);
            this.txt_tax.Name = "txt_tax";
            this.txt_tax.Size = new System.Drawing.Size(171, 36);
            this.txt_tax.TabIndex = 6;
            // 
            // frm_NewInvoice
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1120, 778);
            this.Controls.Add(this.txt_tax);
            this.Controls.Add(this.txt_Total);
            this.Controls.Add(this.txtSubTotal);
            this.Controls.Add(this.gridControlLines);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtTotal);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "frm_NewInvoice";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_NewInvoice";
            this.Load += new System.EventHandler(this.frm_NewInvoice_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dateEdit1;
        private System.Windows.Forms.ComboBox cbxPaymentType;
        private System.Windows.Forms.ComboBox cbxCustomer;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private System.Windows.Forms.ComboBox cbxInvoiceTypee;
        private DevExpress.XtraGrid.GridControl gridControlLines;
        private DevExpress.XtraGrid.Views.Grid.GridView GridView;
        private System.Windows.Forms.TextBox txtSubTotal;
        private System.Windows.Forms.TextBox txt_Total;
        private System.Windows.Forms.TextBox txt_tax;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
    }
}