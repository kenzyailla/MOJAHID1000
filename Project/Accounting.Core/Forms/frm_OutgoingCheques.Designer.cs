namespace Accounting.Core.Forms
{
    partial class frm_OutgoingCheques
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
            this.cbxFilterStatus = new System.Windows.Forms.ComboBox();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnMarkCollected = new System.Windows.Forms.Button();
            this.btnMarkReturned = new System.Windows.Forms.Button();
            this.txtChequeNo = new System.Windows.Forms.TextBox();
            this.txtSupplierName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.searchLookUpEditSupplier = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnFilterDate = new System.Windows.Forms.Button();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditSupplier.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxFilterStatus
            // 
            this.cbxFilterStatus.FormattingEnabled = true;
            this.cbxFilterStatus.Items.AddRange(new object[] {
            "الكل",
            "مصدر",
            "تم صرفه",
            "مرتجع"});
            this.cbxFilterStatus.Location = new System.Drawing.Point(382, 89);
            this.cbxFilterStatus.Name = "cbxFilterStatus";
            this.cbxFilterStatus.Size = new System.Drawing.Size(186, 41);
            this.cbxFilterStatus.TabIndex = 0;
            this.cbxFilterStatus.SelectedIndexChanged += new System.EventHandler(this.cbxFilterStatus_SelectedIndexChanged);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(12, 136);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1473, 524);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowFooter = true;
            // 
            // btnMarkCollected
            // 
            this.btnMarkCollected.Location = new System.Drawing.Point(1249, 88);
            this.btnMarkCollected.Name = "btnMarkCollected";
            this.btnMarkCollected.Size = new System.Drawing.Size(109, 41);
            this.btnMarkCollected.TabIndex = 2;
            this.btnMarkCollected.Text = "تسجيل صرف";
            this.btnMarkCollected.UseVisualStyleBackColor = true;
            this.btnMarkCollected.Click += new System.EventHandler(this.btnMarkCollected_Click);
            // 
            // btnMarkReturned
            // 
            this.btnMarkReturned.Location = new System.Drawing.Point(1364, 88);
            this.btnMarkReturned.Name = "btnMarkReturned";
            this.btnMarkReturned.Size = new System.Drawing.Size(117, 41);
            this.btnMarkReturned.TabIndex = 3;
            this.btnMarkReturned.Text = "تسجيل مرتجع";
            this.btnMarkReturned.UseVisualStyleBackColor = true;
            this.btnMarkReturned.Click += new System.EventHandler(this.btnMarkReturned_Click);
            // 
            // txtChequeNo
            // 
            this.txtChequeNo.Location = new System.Drawing.Point(710, 89);
            this.txtChequeNo.Name = "txtChequeNo";
            this.txtChequeNo.Size = new System.Drawing.Size(100, 36);
            this.txtChequeNo.TabIndex = 4;
            this.txtChequeNo.TextChanged += new System.EventHandler(this.txtChequeNo_TextChanged);
            // 
            // txtSupplierName
            // 
            this.txtSupplierName.Location = new System.Drawing.Point(203, 669);
            this.txtSupplierName.Name = "txtSupplierName";
            this.txtSupplierName.Size = new System.Drawing.Size(30, 36);
            this.txtSupplierName.TabIndex = 5;
            this.txtSupplierName.TextChanged += new System.EventHandler(this.txtSupplierName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(574, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "البحث برقم الشيك";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(816, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 33);
            this.label2.TabIndex = 7;
            this.label2.Text = "البحث باسم المورد";
            // 
            // searchLookUpEditSupplier
            // 
            this.searchLookUpEditSupplier.Location = new System.Drawing.Point(954, 89);
            this.searchLookUpEditSupplier.Name = "searchLookUpEditSupplier";
            this.searchLookUpEditSupplier.Properties.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchLookUpEditSupplier.Properties.Appearance.Options.UseFont = true;
            this.searchLookUpEditSupplier.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEditSupplier.Properties.PopupView = this.searchLookUpEdit1View;
            this.searchLookUpEditSupplier.Size = new System.Drawing.Size(229, 40);
            this.searchLookUpEditSupplier.TabIndex = 8;
            this.searchLookUpEditSupplier.EditValueChanged += new System.EventHandler(this.searchLookUpEditSupplier_EditValueChanged);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(273, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 33);
            this.label3.TabIndex = 9;
            this.label3.Text = "حالة الشيكات";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(580, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 33);
            this.label4.TabIndex = 10;
            this.label4.Text = "الشيكات الصادرة";
            // 
            // dtFrom
            // 
            this.dtFrom.Location = new System.Drawing.Point(102, 55);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(122, 36);
            this.dtFrom.TabIndex = 11;
            // 
            // dtTo
            // 
            this.dtTo.Location = new System.Drawing.Point(102, 97);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(122, 36);
            this.dtTo.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 33);
            this.label5.TabIndex = 13;
            this.label5.Text = "بداية الفترة";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 33);
            this.label6.TabIndex = 14;
            this.label6.Text = "نهاية الفترة";
            // 
            // btnFilterDate
            // 
            this.btnFilterDate.Location = new System.Drawing.Point(1364, 47);
            this.btnFilterDate.Name = "btnFilterDate";
            this.btnFilterDate.Size = new System.Drawing.Size(117, 34);
            this.btnFilterDate.TabIndex = 15;
            this.btnFilterDate.Text = "بحث بالتاريخ";
            this.btnFilterDate.UseVisualStyleBackColor = true;
            this.btnFilterDate.Click += new System.EventHandler(this.btnFilterDate_Click);
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(1137, 669);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(57, 33);
            this.lblTotalAmount.TabIndex = 16;
            this.lblTotalAmount.Text = "label7";
            // 
            // frm_OutgoingCheques
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1497, 717);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.btnFilterDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtTo);
            this.Controls.Add(this.dtFrom);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.searchLookUpEditSupplier);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSupplierName);
            this.Controls.Add(this.txtChequeNo);
            this.Controls.Add(this.btnMarkReturned);
            this.Controls.Add(this.btnMarkCollected);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.cbxFilterStatus);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_OutgoingCheques";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_OutgoingCheques";
            this.Load += new System.EventHandler(this.frm_OutgoingCheques_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditSupplier.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxFilterStatus;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Button btnMarkCollected;
        private System.Windows.Forms.Button btnMarkReturned;
        private System.Windows.Forms.TextBox txtChequeNo;
        private System.Windows.Forms.TextBox txtSupplierName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEditSupplier;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnFilterDate;
        private System.Windows.Forms.Label lblTotalAmount;
    }
}