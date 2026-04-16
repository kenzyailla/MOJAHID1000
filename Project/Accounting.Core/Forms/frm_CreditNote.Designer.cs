namespace Accounting.Core.Forms
{
    partial class frm_CreditNote
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
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dateOriginal = new System.Windows.Forms.DateTimePicker();
            this.txtOriginalInvoiceNumber = new System.Windows.Forms.TextBox();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnSend = new DevExpress.XtraEditors.SimpleButton();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.txtCreditNoteNumber = new System.Windows.Forms.TextBox();
            this.txtReasonNote = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(28, 76);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(989, 502);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            // 
            // dateOriginal
            // 
            this.dateOriginal.Location = new System.Drawing.Point(378, 25);
            this.dateOriginal.Name = "dateOriginal";
            this.dateOriginal.Size = new System.Drawing.Size(148, 22);
            this.dateOriginal.TabIndex = 1;
            // 
            // txtOriginalInvoiceNumber
            // 
            this.txtOriginalInvoiceNumber.Location = new System.Drawing.Point(872, 25);
            this.txtOriginalInvoiceNumber.Name = "txtOriginalInvoiceNumber";
            this.txtOriginalInvoiceNumber.Size = new System.Drawing.Size(155, 22);
            this.txtOriginalInvoiceNumber.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(194, 23);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(145, 29);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "حفظ";
            this.btnSave.Click += new System.EventHandler(this.btnSave_ClickAsync);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(28, 23);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(145, 29);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "ارسال المرتجع";
            this.btnSend.Click += new System.EventHandler(this.btnSend_ClickAsync);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(711, 25);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(155, 22);
            this.txtCustomerName.TabIndex = 5;
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(471, 600);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(155, 22);
            this.txtTotal.TabIndex = 6;
            // 
            // txtTax
            // 
            this.txtTax.Location = new System.Drawing.Point(288, 600);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(155, 22);
            this.txtTax.TabIndex = 7;
            // 
            // txtSubTotal
            // 
            this.txtSubTotal.Location = new System.Drawing.Point(77, 600);
            this.txtSubTotal.Name = "txtSubTotal";
            this.txtSubTotal.Size = new System.Drawing.Size(155, 22);
            this.txtSubTotal.TabIndex = 8;
            // 
            // txtCreditNoteNumber
            // 
            this.txtCreditNoteNumber.Location = new System.Drawing.Point(550, 23);
            this.txtCreditNoteNumber.Name = "txtCreditNoteNumber";
            this.txtCreditNoteNumber.Size = new System.Drawing.Size(155, 22);
            this.txtCreditNoteNumber.TabIndex = 9;
            // 
            // txtReasonNote
            // 
            this.txtReasonNote.Location = new System.Drawing.Point(550, 48);
            this.txtReasonNote.Name = "txtReasonNote";
            this.txtReasonNote.Size = new System.Drawing.Size(155, 22);
            this.txtReasonNote.TabIndex = 10;
            // 
            // frm_CreditNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 634);
            this.Controls.Add(this.txtReasonNote);
            this.Controls.Add(this.txtCreditNoteNumber);
            this.Controls.Add(this.txtSubTotal);
            this.Controls.Add(this.txtTax);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtOriginalInvoiceNumber);
            this.Controls.Add(this.dateOriginal);
            this.Controls.Add(this.gridControl1);
            this.Name = "frm_CreditNote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_CreditNote";
            this.Load += new System.EventHandler(this.frm_CreditNote_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.DateTimePicker dateOriginal;
        private System.Windows.Forms.TextBox txtOriginalInvoiceNumber;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnSend;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtSubTotal;
        private System.Windows.Forms.TextBox txtCreditNoteNumber;
        private System.Windows.Forms.TextBox txtReasonNote;
    }
}