namespace Accounting.Core.Forms
{
    partial class frm_BuyInvoiceEditor
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
            this.btnSave = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.cbxPaymentStatus = new System.Windows.Forms.ComboBox();
            this.dateDue = new System.Windows.Forms.DateTimePicker();
            this.dateInvoice = new System.Windows.Forms.DateTimePicker();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.gridItems = new DevExpress.XtraGrid.GridControl();
            this.gridViewItems = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.cbxSupplier = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchLookUpEdit1 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxPaymentType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewItems)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(23, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 36);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(139, 67);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(232, 36);
            this.txtNotes.TabIndex = 7;
            // 
            // cbxPaymentStatus
            // 
            this.cbxPaymentStatus.FormattingEnabled = true;
            this.cbxPaymentStatus.Location = new System.Drawing.Point(-140, 100);
            this.cbxPaymentStatus.Name = "cbxPaymentStatus";
            this.cbxPaymentStatus.Size = new System.Drawing.Size(115, 41);
            this.cbxPaymentStatus.TabIndex = 5;
            // 
            // dateDue
            // 
            this.dateDue.Location = new System.Drawing.Point(595, 68);
            this.dateDue.Name = "dateDue";
            this.dateDue.Size = new System.Drawing.Size(198, 36);
            this.dateDue.TabIndex = 6;
            // 
            // dateInvoice
            // 
            this.dateInvoice.Location = new System.Drawing.Point(897, 64);
            this.dateInvoice.Name = "dateInvoice";
            this.dateInvoice.Size = new System.Drawing.Size(171, 36);
            this.dateInvoice.TabIndex = 5;
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Location = new System.Drawing.Point(966, 22);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(102, 36);
            this.txtInvoiceNumber.TabIndex = 2;
            // 
            // gridItems
            // 
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Location = new System.Drawing.Point(0, 0);
            this.gridItems.MainView = this.gridViewItems;
            this.gridItems.Name = "gridItems";
            this.gridItems.Size = new System.Drawing.Size(1192, 749);
            this.gridItems.TabIndex = 1;
            this.gridItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewItems});
            // 
            // gridViewItems
            // 
            this.gridViewItems.GridControl = this.gridItems;
            this.gridViewItems.Name = "gridViewItems";
            this.gridViewItems.OptionsView.ShowGroupPanel = false;
            this.gridViewItems.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewItems_CellValueChanged);
            // 
            // txtSubTotal
            // 
            this.txtSubTotal.Location = new System.Drawing.Point(613, 685);
            this.txtSubTotal.Name = "txtSubTotal";
            this.txtSubTotal.Size = new System.Drawing.Size(115, 36);
            this.txtSubTotal.TabIndex = 8;
            // 
            // txtTax
            // 
            this.txtTax.Location = new System.Drawing.Point(734, 685);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(118, 36);
            this.txtTax.TabIndex = 9;
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(858, 685);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(118, 36);
            this.txtTotal.TabIndex = 10;
            // 
            // cbxSupplier
            // 
            this.cbxSupplier.FormattingEnabled = true;
            this.cbxSupplier.Location = new System.Drawing.Point(23, 62);
            this.cbxSupplier.Name = "cbxSupplier";
            this.cbxSupplier.Size = new System.Drawing.Size(110, 41);
            this.cbxSupplier.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbxPaymentType);
            this.panel1.Controls.Add(this.searchLookUpEdit1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cbxSupplier);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtNotes);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dateDue);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dateInvoice);
            this.panel1.Controls.Add(this.txtInvoiceNumber);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1192, 120);
            this.panel1.TabIndex = 13;
            // 
            // searchLookUpEdit1
            // 
            this.searchLookUpEdit1.Location = new System.Drawing.Point(691, 26);
            this.searchLookUpEdit1.Name = "searchLookUpEdit1";
            this.searchLookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEdit1.Properties.PopupView = this.searchLookUpEdit1View;
            this.searchLookUpEdit1.Size = new System.Drawing.Size(186, 22);
            this.searchLookUpEdit1.TabIndex = 19;
            this.searchLookUpEdit1.EditValueChanged += new System.EventHandler(this.searchLookUpEdit1_EditValueChanged);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(377, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 33);
            this.label5.TabIndex = 18;
            this.label5.Text = "ملاحظات";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(799, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 33);
            this.label4.TabIndex = 17;
            this.label4.Text = "الاستحقاق";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1087, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 33);
            this.label3.TabIndex = 16;
            this.label3.Text = "تاريخ الفاتورة";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(883, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 33);
            this.label2.TabIndex = 15;
            this.label2.Text = "المورد";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1087, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 33);
            this.label1.TabIndex = 14;
            this.label1.Text = "رقم الفاتورة";
            // 
            // cbxPaymentType
            // 
            this.cbxPaymentType.FormattingEnabled = true;
            this.cbxPaymentType.Location = new System.Drawing.Point(139, 16);
            this.cbxPaymentType.Name = "cbxPaymentType";
            this.cbxPaymentType.Size = new System.Drawing.Size(232, 41);
            this.cbxPaymentType.TabIndex = 14;
            // 
            // frm_BuyInvoiceEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1192, 749);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtTax);
            this.Controls.Add(this.cbxPaymentStatus);
            this.Controls.Add(this.txtSubTotal);
            this.Controls.Add(this.gridItems);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "frm_BuyInvoiceEditor";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_BuyInvoiceEditor";
            this.Load += new System.EventHandler(this.frm_BuyInvoiceEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewItems)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraGrid.GridControl gridItems;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewItems;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private System.Windows.Forms.DateTimePicker dateDue;
        private System.Windows.Forms.DateTimePicker dateInvoice;
        private System.Windows.Forms.ComboBox cbxPaymentStatus;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtSubTotal;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbxSupplier;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private System.Windows.Forms.ComboBox cbxPaymentType;
    }
}