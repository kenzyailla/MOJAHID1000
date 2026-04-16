namespace Accounting.Core.Forms
{
    partial class frm_InvoiceEditor
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
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.dtInvoiceDate = new DevExpress.XtraEditors.DateEdit();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.gridControlLines = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txt_tax = new System.Windows.Forms.TextBox();
            this.txt_Total = new System.Windows.Forms.TextBox();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.btnSendToTax = new System.Windows.Forms.Button();
            this.btnSendReturnToTax = new System.Windows.Forms.Button();
            this.cbxCustomer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvoiceNumber.Location = new System.Drawing.Point(321, 38);
            this.txtInvoiceNumber.Multiline = true;
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(90, 37);
            this.txtInvoiceNumber.TabIndex = 1;
            // 
            // dtInvoiceDate
            // 
            this.dtInvoiceDate.EditValue = null;
            this.dtInvoiceDate.Location = new System.Drawing.Point(791, 12);
            this.dtInvoiceDate.Name = "dtInvoiceDate";
            this.dtInvoiceDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInvoiceDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInvoiceDate.Size = new System.Drawing.Size(174, 22);
            this.dtInvoiceDate.TabIndex = 2;
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(553, 38);
            this.txtTotal.Multiline = true;
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(80, 37);
            this.txtTotal.TabIndex = 3;
            // 
            // gridControlLines
            // 
            this.gridControlLines.Location = new System.Drawing.Point(59, 91);
            this.gridControlLines.MainView = this.gridView1;
            this.gridControlLines.Name = "gridControlLines";
            this.gridControlLines.Size = new System.Drawing.Size(906, 616);
            this.gridControlLines.TabIndex = 4;
            this.gridControlLines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControlLines;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(55, 35);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(147, 38);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "حفظ التعديلات";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // txt_tax
            // 
            this.txt_tax.Location = new System.Drawing.Point(608, 640);
            this.txt_tax.Name = "txt_tax";
            this.txt_tax.Size = new System.Drawing.Size(171, 40);
            this.txt_tax.TabIndex = 9;
            // 
            // txt_Total
            // 
            this.txt_Total.Location = new System.Drawing.Point(785, 640);
            this.txt_Total.Name = "txt_Total";
            this.txt_Total.Size = new System.Drawing.Size(171, 40);
            this.txt_Total.TabIndex = 8;
            // 
            // txtSubTotal
            // 
            this.txtSubTotal.Location = new System.Drawing.Point(431, 640);
            this.txtSubTotal.Name = "txtSubTotal";
            this.txtSubTotal.Size = new System.Drawing.Size(171, 40);
            this.txtSubTotal.TabIndex = 7;
            // 
            // btnSendToTax
            // 
            this.btnSendToTax.Location = new System.Drawing.Point(806, 49);
            this.btnSendToTax.Name = "btnSendToTax";
            this.btnSendToTax.Size = new System.Drawing.Size(72, 36);
            this.btnSendToTax.TabIndex = 10;
            this.btnSendToTax.Text = "إرسال للفوترة";
            this.btnSendToTax.UseVisualStyleBackColor = true;
            this.btnSendToTax.Click += new System.EventHandler(this.btnSendToTax_Click);
            // 
            // btnSendReturnToTax
            // 
            this.btnSendReturnToTax.Location = new System.Drawing.Point(884, 48);
            this.btnSendReturnToTax.Name = "btnSendReturnToTax";
            this.btnSendReturnToTax.Size = new System.Drawing.Size(72, 36);
            this.btnSendReturnToTax.TabIndex = 11;
            this.btnSendReturnToTax.Text = "مرتجع";
            this.btnSendReturnToTax.UseVisualStyleBackColor = true;
            // 
            // cbxCustomer
            // 
            this.cbxCustomer.FormattingEnabled = true;
            this.cbxCustomer.Location = new System.Drawing.Point(723, -1);
            this.cbxCustomer.Name = "cbxCustomer";
            this.cbxCustomer.Size = new System.Drawing.Size(22, 45);
            this.cbxCustomer.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 37);
            this.label1.TabIndex = 13;
            this.label1.Text = "رقم الفاتورة";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(417, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 37);
            this.label2.TabIndex = 14;
            this.label2.Text = "اجمالي الفاتورة";
            // 
            // frm_InvoiceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 722);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxCustomer);
            this.Controls.Add(this.btnSendReturnToTax);
            this.Controls.Add(this.btnSendToTax);
            this.Controls.Add(this.txt_tax);
            this.Controls.Add(this.txt_Total);
            this.Controls.Add(this.txtSubTotal);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gridControlLines);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.dtInvoiceDate);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            this.Name = "frm_InvoiceEditor";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_InvoiceEditor";
            this.Load += new System.EventHandler(this.frm_InvoiceEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private DevExpress.XtraEditors.DateEdit dtInvoiceDate;
        private System.Windows.Forms.TextBox txtTotal;
        private DevExpress.XtraGrid.GridControl gridControlLines;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private System.Windows.Forms.TextBox txt_tax;
        private System.Windows.Forms.TextBox txt_Total;
        private System.Windows.Forms.TextBox txtSubTotal;
        private System.Windows.Forms.Button btnSendToTax;
        private System.Windows.Forms.Button btnSendReturnToTax;
        private System.Windows.Forms.ComboBox cbxCustomer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}