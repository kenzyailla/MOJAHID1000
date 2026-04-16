namespace Accounting.Core.Forms
{
    partial class frm_CustomerEditor
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOpeningBalance = new System.Windows.Forms.TextBox();
            this.cbxBalanceType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(23, 99);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(288, 40);
            this.txtName.TabIndex = 0;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(326, 99);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(203, 40);
            this.txtPhone.TabIndex = 1;
            // 
            // txtTax
            // 
            this.txtTax.Location = new System.Drawing.Point(535, 99);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(203, 40);
            this.txtTax.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(988, 179);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(176, 40);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "حفظ";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(110, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 37);
            this.label1.TabIndex = 4;
            this.label1.Text = "اسم العميل";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(367, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 37);
            this.label2.TabIndex = 5;
            this.label2.Text = "هاتف العميل";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(557, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 37);
            this.label3.TabIndex = 6;
            this.label3.Text = "رقم ضريبة العميل";
            // 
            // txtOpeningBalance
            // 
            this.txtOpeningBalance.Location = new System.Drawing.Point(744, 99);
            this.txtOpeningBalance.Name = "txtOpeningBalance";
            this.txtOpeningBalance.Size = new System.Drawing.Size(203, 40);
            this.txtOpeningBalance.TabIndex = 3;
            // 
            // cbxBalanceType
            // 
            this.cbxBalanceType.FormattingEnabled = true;
            this.cbxBalanceType.Location = new System.Drawing.Point(953, 99);
            this.cbxBalanceType.Name = "cbxBalanceType";
            this.cbxBalanceType.Size = new System.Drawing.Size(211, 45);
            this.cbxBalanceType.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(763, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 37);
            this.label4.TabIndex = 9;
            this.label4.Text = "الرصيد الافتتاحي";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1010, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 37);
            this.label5.TabIndex = 10;
            this.label5.Text = "نوع الرصيد";
            // 
            // frm_CustomerEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1207, 284);
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
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            this.Name = "frm_CustomerEditor";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_CustomerEditor";
            this.Load += new System.EventHandler(this.frm_CustomerEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtTax;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOpeningBalance;
        private System.Windows.Forms.ComboBox cbxBalanceType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}