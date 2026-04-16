namespace Accounting.Core.Forms
{
    partial class frm_InvoicesList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_InvoicesList));
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnSend = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.btnShowQR = new DevExpress.XtraEditors.SimpleButton();
            this.btnCreditNote = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNew
            // 
            this.btnNew.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Appearance.Options.UseFont = true;
            this.btnNew.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.ImageOptions.Image")));
            this.btnNew.Location = new System.Drawing.Point(26, 32);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(161, 46);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "إنشاء فاتورة";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.ImageOptions.Image")));
            this.btnEdit.Location = new System.Drawing.Point(193, 32);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(161, 46);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "تعديل فاتورة";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSend
            // 
            this.btnSend.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.Appearance.Options.UseFont = true;
            this.btnSend.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSend.ImageOptions.Image")));
            this.btnSend.Location = new System.Drawing.Point(360, 32);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(161, 46);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "إرسال للضريبة";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(527, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(161, 46);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "قائمة الفواتير";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInvoices.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvInvoices.BackgroundColor = System.Drawing.Color.White;
            this.dgvInvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoices.Location = new System.Drawing.Point(26, 84);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.RowTemplate.Height = 24;
            this.dgvInvoices.Size = new System.Drawing.Size(1322, 642);
            this.dgvInvoices.TabIndex = 5;
            this.dgvInvoices.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInvoices_CellDoubleClick);
            // 
            // btnShowQR
            // 
            this.btnShowQR.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowQR.Appearance.Options.UseFont = true;
            this.btnShowQR.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnShowQR.ImageOptions.Image")));
            this.btnShowQR.Location = new System.Drawing.Point(861, 32);
            this.btnShowQR.Name = "btnShowQR";
            this.btnShowQR.Size = new System.Drawing.Size(161, 46);
            this.btnShowQR.TabIndex = 6;
            this.btnShowQR.Text = "عرض QR";
            this.btnShowQR.Click += new System.EventHandler(this.btnShowQR_Click);
            // 
            // btnCreditNote
            // 
            this.btnCreditNote.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreditNote.Appearance.Options.UseFont = true;
            this.btnCreditNote.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCreditNote.ImageOptions.Image")));
            this.btnCreditNote.Location = new System.Drawing.Point(694, 32);
            this.btnCreditNote.Name = "btnCreditNote";
            this.btnCreditNote.Size = new System.Drawing.Size(161, 46);
            this.btnCreditNote.TabIndex = 7;
            this.btnCreditNote.Text = "إنشاء مرتجع";
            this.btnCreditNote.Click += new System.EventHandler(this.btnCreditNote_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.ImageOptions.Image")));
            this.btnPrint.Location = new System.Drawing.Point(1028, 32);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(161, 46);
            this.btnPrint.TabIndex = 8;
            this.btnPrint.Text = "طباعة الفاتورة";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // frm_InvoicesList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1384, 738);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnCreditNote);
            this.Controls.Add(this.btnShowQR);
            this.Controls.Add(this.dgvInvoices);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnNew);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_InvoicesList";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_InvoicesList";
            this.Load += new System.EventHandler(this.frm_InvoicesList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraEditors.SimpleButton btnSend;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private System.Windows.Forms.DataGridView dgvInvoices;
        private DevExpress.XtraEditors.SimpleButton btnShowQR;
        private DevExpress.XtraEditors.SimpleButton btnCreditNote;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
    }
}