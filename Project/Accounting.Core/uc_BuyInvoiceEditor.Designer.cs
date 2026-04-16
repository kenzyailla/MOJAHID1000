using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    partial class uc_BuyInvoiceEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.gridItems = new DevExpress.XtraGrid.GridControl();
            this.gridViewItems = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.cbxPaymentStatus = new System.Windows.Forms.ComboBox();
            this.cbxSupplier = new System.Windows.Forms.ComboBox();
            this.dateDue = new System.Windows.Forms.DateTimePicker();
            this.dateInvoice = new System.Windows.Forms.DateTimePicker();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewItems)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(340, 687);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(118, 22);
            this.txtTotal.TabIndex = 15;
            // 
            // txtTax
            // 
            this.txtTax.Location = new System.Drawing.Point(216, 687);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(118, 22);
            this.txtTax.TabIndex = 14;
            // 
            // txtSubTotal
            // 
            this.txtSubTotal.Location = new System.Drawing.Point(95, 687);
            this.txtSubTotal.Name = "txtSubTotal";
            this.txtSubTotal.Size = new System.Drawing.Size(115, 22);
            this.txtSubTotal.TabIndex = 13;
            // 
            // gridItems
            // 
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Location = new System.Drawing.Point(0, 120);
            this.gridItems.MainView = this.gridViewItems;
            this.gridItems.Margin = new System.Windows.Forms.Padding(0);
            this.gridItems.Name = "gridItems";
            this.gridItems.Size = new System.Drawing.Size(1257, 640);
            this.gridItems.TabIndex = 16;
            this.gridItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewItems});
            // 
            // gridViewItems
            // 
            this.gridViewItems.GridControl = this.gridItems;
            this.gridViewItems.Name = "gridViewItems";
            this.gridViewItems.OptionsView.ShowGroupPanel = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.txtNotes);
            this.panel1.Controls.Add(this.cbxPaymentStatus);
            this.panel1.Controls.Add(this.cbxSupplier);
            this.panel1.Controls.Add(this.dateDue);
            this.panel1.Controls.Add(this.dateInvoice);
            this.panel1.Controls.Add(this.txtInvoiceNumber);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(1257, 120);
            this.panel1.TabIndex = 17;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(44, 62);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(131, 36);
            this.btnNew.TabIndex = 12;
            this.btnNew.Text = "جديد";
            this.btnNew.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(181, 61);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(131, 36);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(318, 61);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(171, 22);
            this.txtNotes.TabIndex = 7;
            // 
            // cbxPaymentStatus
            // 
            this.cbxPaymentStatus.FormattingEnabled = true;
            this.cbxPaymentStatus.Location = new System.Drawing.Point(510, 57);
            this.cbxPaymentStatus.Name = "cbxPaymentStatus";
            this.cbxPaymentStatus.Size = new System.Drawing.Size(245, 24);
            this.cbxPaymentStatus.TabIndex = 5;
            // 
            // cbxSupplier
            // 
            this.cbxSupplier.FormattingEnabled = true;
            this.cbxSupplier.Location = new System.Drawing.Point(761, 56);
            this.cbxSupplier.Name = "cbxSupplier";
            this.cbxSupplier.Size = new System.Drawing.Size(355, 24);
            this.cbxSupplier.TabIndex = 4;
            // 
            // dateDue
            // 
            this.dateDue.Location = new System.Drawing.Point(541, 15);
            this.dateDue.Name = "dateDue";
            this.dateDue.Size = new System.Drawing.Size(169, 22);
            this.dateDue.TabIndex = 6;
            // 
            // dateInvoice
            // 
            this.dateInvoice.Location = new System.Drawing.Point(745, 15);
            this.dateInvoice.Name = "dateInvoice";
            this.dateInvoice.Size = new System.Drawing.Size(171, 22);
            this.dateInvoice.TabIndex = 5;
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Location = new System.Drawing.Point(938, 15);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(178, 22);
            this.txtInvoiceNumber.TabIndex = 2;
            // 
            // uc_BuyInvoiceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtTax);
            this.Controls.Add(this.txtSubTotal);
            this.Controls.Add(this.gridItems);
            this.Controls.Add(this.panel1);
            this.Name = "uc_BuyInvoiceEditor";
            this.Size = new System.Drawing.Size(1257, 760);
            this.Load += new System.EventHandler(this.uc_BuyInvoiceEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewItems)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtSubTotal;
        private DevExpress.XtraGrid.GridControl gridItems;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewItems;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.ComboBox cbxPaymentStatus;
        private System.Windows.Forms.ComboBox cbxSupplier;
        private System.Windows.Forms.DateTimePicker dateDue;
        private System.Windows.Forms.DateTimePicker dateInvoice;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
    }
}
