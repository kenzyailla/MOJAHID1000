namespace Accounting.Core.Forms
{
    partial class frm_SupplierEditor
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxBalanceType = new System.Windows.Forms.ComboBox();
            this.txtOpeningBalance = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1004, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 33);
            this.label5.TabIndex = 21;
            this.label5.Text = "نوع الرصيد";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(757, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 33);
            this.label4.TabIndex = 20;
            this.label4.Text = "الرصيد الافتتاحي";
            // 
            // cbxBalanceType
            // 
            this.cbxBalanceType.FormattingEnabled = true;
            this.cbxBalanceType.Location = new System.Drawing.Point(947, 71);
            this.cbxBalanceType.Name = "cbxBalanceType";
            this.cbxBalanceType.Size = new System.Drawing.Size(211, 41);
            this.cbxBalanceType.TabIndex = 19;
            // 
            // txtOpeningBalance
            // 
            this.txtOpeningBalance.Location = new System.Drawing.Point(738, 71);
            this.txtOpeningBalance.Name = "txtOpeningBalance";
            this.txtOpeningBalance.Size = new System.Drawing.Size(203, 36);
            this.txtOpeningBalance.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(551, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 33);
            this.label3.TabIndex = 17;
            this.label3.Text = "رقم ضريبة المورد";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(361, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 33);
            this.label2.TabIndex = 16;
            this.label2.Text = "هاتف المورد";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 33);
            this.label1.TabIndex = 15;
            this.label1.Text = "اسم المورد";
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(982, 151);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(176, 40);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "حفظ";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // txtTax
            // 
            this.txtTax.Location = new System.Drawing.Point(529, 71);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(203, 36);
            this.txtTax.TabIndex = 13;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(320, 71);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(203, 36);
            this.txtPhone.TabIndex = 12;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(17, 71);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(288, 36);
            this.txtName.TabIndex = 11;
            // 
            // frm_SupplierEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1179, 228);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbxBalanceType);
            this.Controls.Add(this.txtOpeningBalance);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtTax);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.txtName);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_SupplierEditor";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_SupplierEditor";
            this.Load += new System.EventHandler(this.frm_SupplierEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxBalanceType;
        private System.Windows.Forms.TextBox txtOpeningBalance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtName;
    }
}