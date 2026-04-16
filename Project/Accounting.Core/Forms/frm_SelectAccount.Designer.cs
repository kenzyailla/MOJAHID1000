namespace Accounting.Core.Forms
{
    partial class frm_SelectAccount
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.gridAccounts = new DevExpress.XtraGrid.GridControl();
            this.gridViewAccounts = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(29, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(159, 41);
            this.comboBox1.TabIndex = 0;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(194, 39);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(144, 41);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "اختيار";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // gridAccounts
            // 
            this.gridAccounts.Location = new System.Drawing.Point(29, 86);
            this.gridAccounts.MainView = this.gridViewAccounts;
            this.gridAccounts.Name = "gridAccounts";
            this.gridAccounts.Size = new System.Drawing.Size(822, 364);
            this.gridAccounts.TabIndex = 3;
            this.gridAccounts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewAccounts});
            // 
            // gridViewAccounts
            // 
            this.gridViewAccounts.GridControl = this.gridAccounts;
            this.gridViewAccounts.Name = "gridViewAccounts";
            this.gridViewAccounts.DoubleClick += new System.EventHandler(this.gridViewAccounts_DoubleClick);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(360, 39);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(111, 41);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "موافق";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frm_SelectAccount
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(884, 518);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gridAccounts);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.comboBox1);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "frm_SelectAccount";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_SelectAccount";
            this.Load += new System.EventHandler(this.frm_SelectAccount_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnSelect;
        private DevExpress.XtraGrid.GridControl gridAccounts;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewAccounts;
        private System.Windows.Forms.Button btnOK;
    }
}