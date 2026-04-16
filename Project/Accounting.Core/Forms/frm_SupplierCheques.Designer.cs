namespace Accounting.Core.Forms
{
    partial class frm_SupplierCheques
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
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnMarkCleared = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lilbalance = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(38, 107);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(931, 616);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowFooter = true;
            this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(38, 52);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(236, 38);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnMarkCleared
            // 
            this.btnMarkCleared.Location = new System.Drawing.Point(789, 52);
            this.btnMarkCleared.Name = "btnMarkCleared";
            this.btnMarkCleared.Size = new System.Drawing.Size(180, 38);
            this.btnMarkCleared.TabIndex = 3;
            this.btnMarkCleared.Text = "تم صرف الشيك";
            this.btnMarkCleared.UseVisualStyleBackColor = true;
            this.btnMarkCleared.Click += new System.EventHandler(this.btnMarkCleared_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(420, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "الشيكات قبل هاذا النظام";
            // 
            // lilbalance
            // 
            this.lilbalance.AutoSize = true;
            this.lilbalance.Location = new System.Drawing.Point(719, 726);
            this.lilbalance.Name = "lilbalance";
            this.lilbalance.Size = new System.Drawing.Size(57, 33);
            this.lilbalance.TabIndex = 5;
            this.lilbalance.Text = "label2";
            // 
            // frm_SupplierCheques
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1004, 764);
            this.Controls.Add(this.lilbalance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMarkCleared);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.gridControl1);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_SupplierCheques";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_SupplierCheques";
            this.Load += new System.EventHandler(this.frm_SupplierCheques_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnMarkCleared;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lilbalance;
    }
}