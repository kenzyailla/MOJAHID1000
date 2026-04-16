namespace Accounting.Core.Forms
{
    partial class frm_SalesReturn
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbxInvoicecomboBox1 = new System.Windows.Forms.ComboBox();
            this.gridLines = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "ارجاع فاتورة مبيعات";
            // 
            // cbxInvoicecomboBox1
            // 
            this.cbxInvoicecomboBox1.FormattingEnabled = true;
            this.cbxInvoicecomboBox1.Location = new System.Drawing.Point(286, 109);
            this.cbxInvoicecomboBox1.Name = "cbxInvoicecomboBox1";
            this.cbxInvoicecomboBox1.Size = new System.Drawing.Size(193, 41);
            this.cbxInvoicecomboBox1.TabIndex = 1;
            this.cbxInvoicecomboBox1.SelectedIndexChanged += new System.EventHandler(this.cbxInvoicecomboBox1_SelectedIndexChanged);
            this.cbxInvoicecomboBox1.SelectionChangeCommitted += new System.EventHandler(this.cbxInvoicecomboBox1_SelectionChangeCommitted);
            // 
            // gridLines
            // 
            this.gridLines.Location = new System.Drawing.Point(24, 156);
            this.gridLines.MainView = this.gridView1;
            this.gridLines.Name = "gridLines";
            this.gridLines.Size = new System.Drawing.Size(1138, 510);
            this.gridLines.TabIndex = 2;
            this.gridLines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridLines;
            this.gridView1.Name = "gridView1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(923, 690);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "الاجمالي";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1027, 687);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 39);
            this.button1.TabIndex = 4;
            this.button1.Text = "حفظ المرتجع";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(516, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 33);
            this.label3.TabIndex = 5;
            this.label3.Text = "ارجاع فاتورة مبيعات";
            // 
            // frm_SalesReturn
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1262, 738);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gridLines);
            this.Controls.Add(this.cbxInvoicecomboBox1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_SalesReturn";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_SalesReturn";
            this.Load += new System.EventHandler(this.frm_SalesReturn_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxInvoicecomboBox1;
        private DevExpress.XtraGrid.GridControl gridLines;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
    }
}