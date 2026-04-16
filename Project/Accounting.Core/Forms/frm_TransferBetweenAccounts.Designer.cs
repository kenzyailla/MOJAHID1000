namespace Accounting.Core.Forms
{
    partial class frm_TransferBetweenAccounts
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
            this.cbxfrom = new System.Windows.Forms.ComboBox();
            this.cbxTo = new System.Windows.Forms.ComboBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtnote = new System.Windows.Forms.TextBox();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbxfrom
            // 
            this.cbxfrom.FormattingEnabled = true;
            this.cbxfrom.Location = new System.Drawing.Point(94, 79);
            this.cbxfrom.Name = "cbxfrom";
            this.cbxfrom.Size = new System.Drawing.Size(183, 41);
            this.cbxfrom.TabIndex = 0;
            // 
            // cbxTo
            // 
            this.cbxTo.FormattingEnabled = true;
            this.cbxTo.Location = new System.Drawing.Point(372, 79);
            this.cbxTo.Name = "cbxTo";
            this.cbxTo.Size = new System.Drawing.Size(183, 41);
            this.cbxTo.TabIndex = 1;
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(663, 82);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(119, 36);
            this.txtAmount.TabIndex = 2;
            // 
            // txtnote
            // 
            this.txtnote.Location = new System.Drawing.Point(104, 126);
            this.txtnote.Name = "txtnote";
            this.txtnote.Size = new System.Drawing.Size(451, 36);
            this.txtnote.TabIndex = 3;
            // 
            // dtDate
            // 
            this.dtDate.Location = new System.Drawing.Point(896, 82);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(120, 36);
            this.dtDate.TabIndex = 4;
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(894, 130);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(119, 32);
            this.btnTransfer.TabIndex = 5;
            this.btnTransfer.Text = "حول";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(366, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(317, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "شاشة التحويل من البنك الى الصندوق والعكس";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 33);
            this.label2.TabIndex = 7;
            this.label2.Text = "تحويل من ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(283, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 33);
            this.label3.TabIndex = 8;
            this.label3.Text = "تحويل الى";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(561, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 33);
            this.label4.TabIndex = 9;
            this.label4.Text = "مبلع التحويل";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(788, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 33);
            this.label5.TabIndex = 10;
            this.label5.Text = "مبلع التحويل";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 33);
            this.label6.TabIndex = 11;
            this.label6.Text = "سبب التحويل";
            // 
            // frm_TransferBetweenAccounts
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1025, 211);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.dtDate);
            this.Controls.Add(this.txtnote);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.cbxTo);
            this.Controls.Add(this.cbxfrom);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_TransferBetweenAccounts";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "شاشة التحويلات";
            this.Load += new System.EventHandler(this.frm_TransferBetweenAccounts_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxfrom;
        private System.Windows.Forms.ComboBox cbxTo;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.TextBox txtnote;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}