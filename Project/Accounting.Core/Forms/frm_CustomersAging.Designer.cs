namespace Accounting.Core.Forms
{
    partial class frm_CustomersAging
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
            this.lbl0_30 = new System.Windows.Forms.Label();
            this.lbl31_60 = new System.Windows.Forms.Label();
            this.lbl61_90 = new System.Windows.Forms.Label();
            this.lbl90Plus = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnPrintAging = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(25, 83);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1126, 418);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowFooter = true;
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // lbl0_30
            // 
            this.lbl0_30.AutoSize = true;
            this.lbl0_30.BackColor = System.Drawing.Color.Transparent;
            this.lbl0_30.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl0_30.Location = new System.Drawing.Point(158, 458);
            this.lbl0_30.Name = "lbl0_30";
            this.lbl0_30.Size = new System.Drawing.Size(57, 33);
            this.lbl0_30.TabIndex = 1;
            this.lbl0_30.Text = "label1";
            // 
            // lbl31_60
            // 
            this.lbl31_60.AutoSize = true;
            this.lbl31_60.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl31_60.Location = new System.Drawing.Point(401, 458);
            this.lbl31_60.Name = "lbl31_60";
            this.lbl31_60.Size = new System.Drawing.Size(57, 33);
            this.lbl31_60.TabIndex = 2;
            this.lbl31_60.Text = "label2";
            // 
            // lbl61_90
            // 
            this.lbl61_90.AutoSize = true;
            this.lbl61_90.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl61_90.Location = new System.Drawing.Point(618, 458);
            this.lbl61_90.Name = "lbl61_90";
            this.lbl61_90.Size = new System.Drawing.Size(57, 33);
            this.lbl61_90.TabIndex = 3;
            this.lbl61_90.Text = "label3";
            // 
            // lbl90Plus
            // 
            this.lbl90Plus.AutoSize = true;
            this.lbl90Plus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl90Plus.Location = new System.Drawing.Point(826, 458);
            this.lbl90Plus.Name = "lbl90Plus";
            this.lbl90Plus.Size = new System.Drawing.Size(57, 33);
            this.lbl90Plus.TabIndex = 4;
            this.lbl90Plus.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(1043, 458);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 33);
            this.label5.TabIndex = 5;
            this.label5.Text = "label5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Location = new System.Drawing.Point(41, 458);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "من 0-30 يوم";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.label2.Location = new System.Drawing.Point(260, 458);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 33);
            this.label2.TabIndex = 7;
            this.label2.Text = "من 31-60 يوم";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.label3.Location = new System.Drawing.Point(482, 458);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 33);
            this.label3.TabIndex = 8;
            this.label3.Text = "من 61-90 يوم";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.label4.Location = new System.Drawing.Point(697, 458);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 33);
            this.label4.TabIndex = 9;
            this.label4.Text = "اكثر من 90 يوم";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.label6.Location = new System.Drawing.Point(906, 458);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 33);
            this.label6.TabIndex = 10;
            this.label6.Text = "اجمالي الديون";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label7.Location = new System.Drawing.Point(498, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(168, 33);
            this.label7.TabIndex = 11;
            this.label7.Text = "ذمم العملاء حسب العمر";
            // 
            // btnPrintAging
            // 
            this.btnPrintAging.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrintAging.Appearance.Options.UseFont = true;
            this.btnPrintAging.Location = new System.Drawing.Point(1036, 23);
            this.btnPrintAging.Name = "btnPrintAging";
            this.btnPrintAging.Size = new System.Drawing.Size(115, 30);
            this.btnPrintAging.TabIndex = 12;
            this.btnPrintAging.Text = "طباعة";
            this.btnPrintAging.Click += new System.EventHandler(this.btnPrintAging_Click);
            // 
            // frm_CustomersAging
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1218, 592);
            this.Controls.Add(this.btnPrintAging);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbl90Plus);
            this.Controls.Add(this.lbl61_90);
            this.Controls.Add(this.lbl31_60);
            this.Controls.Add(this.lbl0_30);
            this.Controls.Add(this.gridControl1);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_CustomersAging";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_CustomersAging";
            this.Load += new System.EventHandler(this.frm_CustomersAging_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Label lbl0_30;
        private System.Windows.Forms.Label lbl31_60;
        private System.Windows.Forms.Label lbl61_90;
        private System.Windows.Forms.Label lbl90Plus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private DevExpress.XtraEditors.SimpleButton btnPrintAging;
    }
}