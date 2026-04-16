namespace Accounting.Core.Forms
{
    partial class frm_masrouf
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.cbxCategory = new System.Windows.Forms.ComboBox();
            this.cbxPaymentType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtChequeNumber = new System.Windows.Forms.TextBox();
            this.dtpChequeDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxSupplier = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(634, -5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 33);
            this.label5.TabIndex = 19;
            this.label5.Text = "ادخال المصروفات";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(489, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 33);
            this.label4.TabIndex = 18;
            this.label4.Text = "تاريخ الاستحقاق";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(557, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 33);
            this.label3.TabIndex = 17;
            this.label3.Text = "المبلغ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 33);
            this.label2.TabIndex = 16;
            this.label2.Text = "البيان";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 33);
            this.label1.TabIndex = 15;
            this.label1.Text = "نوع المصروف";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1199, 47);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(145, 35);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(615, 108);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(145, 36);
            this.dtpDate.TabIndex = 13;
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(615, 64);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(145, 36);
            this.txtAmount.TabIndex = 12;
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(140, 109);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(330, 36);
            this.txtDesc.TabIndex = 11;
            // 
            // cbxCategory
            // 
            this.cbxCategory.FormattingEnabled = true;
            this.cbxCategory.Location = new System.Drawing.Point(140, 64);
            this.cbxCategory.Name = "cbxCategory";
            this.cbxCategory.Size = new System.Drawing.Size(134, 41);
            this.cbxCategory.TabIndex = 10;
            // 
            // cbxPaymentType
            // 
            this.cbxPaymentType.FormattingEnabled = true;
            this.cbxPaymentType.Location = new System.Drawing.Point(937, 64);
            this.cbxPaymentType.Name = "cbxPaymentType";
            this.cbxPaymentType.Size = new System.Drawing.Size(126, 41);
            this.cbxPaymentType.TabIndex = 20;
            this.cbxPaymentType.SelectedIndexChanged += new System.EventHandler(this.cbxPaymentType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(796, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 33);
            this.label6.TabIndex = 21;
            this.label6.Text = "طريقة الدفع";
            // 
            // txtChequeNumber
            // 
            this.txtChequeNumber.Location = new System.Drawing.Point(937, 109);
            this.txtChequeNumber.Name = "txtChequeNumber";
            this.txtChequeNumber.Size = new System.Drawing.Size(126, 36);
            this.txtChequeNumber.TabIndex = 22;
            // 
            // dtpChequeDate
            // 
            this.dtpChequeDate.Location = new System.Drawing.Point(1223, 108);
            this.dtpChequeDate.Name = "dtpChequeDate";
            this.dtpChequeDate.Size = new System.Drawing.Size(121, 36);
            this.dtpChequeDate.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(765, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(170, 33);
            this.label7.TabIndex = 24;
            this.label7.Text = "رقم شيك دفع المصروف";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1071, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 33);
            this.label8.TabIndex = 25;
            this.label8.Text = "تاريخ شيك المصروف";
            // 
            // cbxSupplier
            // 
            this.cbxSupplier.FormattingEnabled = true;
            this.cbxSupplier.Location = new System.Drawing.Point(394, 64);
            this.cbxSupplier.Name = "cbxSupplier";
            this.cbxSupplier.Size = new System.Drawing.Size(157, 41);
            this.cbxSupplier.TabIndex = 26;
            this.cbxSupplier.Text = "مصروف";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(319, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 33);
            this.label9.TabIndex = 27;
            this.label9.Text = "المورد";
            // 
            // frm_masrouf
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1370, 208);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cbxSupplier);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dtpChequeDate);
            this.Controls.Add(this.txtChequeNumber);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbxPaymentType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.cbxCategory);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_masrouf";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "employees";
            this.Load += new System.EventHandler(this.employees_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.ComboBox cbxCategory;
        private System.Windows.Forms.ComboBox cbxPaymentType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtChequeNumber;
        private System.Windows.Forms.DateTimePicker dtpChequeDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxSupplier;
        private System.Windows.Forms.Label label9;
    }
}