namespace Accounting.Core.Forms
{
    partial class frm_BuyPayment
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
            this.cbxSupplier = new System.Windows.Forms.ComboBox();
            this.datePayment = new System.Windows.Forms.DateTimePicker();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSavePayment = new System.Windows.Forms.Button();
            this.txtCashAmount = new System.Windows.Forms.TextBox();
            this.btnAddCheque = new System.Windows.Forms.Button();
            this.gridControlCheques = new DevExpress.XtraGrid.GridControl();
            this.gridViewCheques = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxPaymentMethod = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.searchLookUpEdit1 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCheques)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCheques)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxSupplier
            // 
            this.cbxSupplier.FormattingEnabled = true;
            this.cbxSupplier.Location = new System.Drawing.Point(683, 153);
            this.cbxSupplier.Name = "cbxSupplier";
            this.cbxSupplier.Size = new System.Drawing.Size(139, 36);
            this.cbxSupplier.TabIndex = 0;
            this.cbxSupplier.SelectedIndexChanged += new System.EventHandler(this.cbxSupplier_SelectedIndexChanged);
            this.cbxSupplier.TextChanged += new System.EventHandler(this.cbxSupplier_TextChanged);
            // 
            // datePayment
            // 
            this.datePayment.Location = new System.Drawing.Point(137, 114);
            this.datePayment.Name = "datePayment";
            this.datePayment.Size = new System.Drawing.Size(144, 32);
            this.datePayment.TabIndex = 4;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(48, 194);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(996, 425);
            this.gridControl1.TabIndex = 7;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gridView1_ValidateRow);
            // 
            // btnSavePayment
            // 
            this.btnSavePayment.Font = new System.Drawing.Font("Noto Kufi Arabic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePayment.Location = new System.Drawing.Point(894, 99);
            this.btnSavePayment.Name = "btnSavePayment";
            this.btnSavePayment.Size = new System.Drawing.Size(150, 37);
            this.btnSavePayment.TabIndex = 8;
            this.btnSavePayment.Text = "حفظ السداد";
            this.btnSavePayment.UseVisualStyleBackColor = true;
            this.btnSavePayment.Click += new System.EventHandler(this.btnSavePayment_Click);
            // 
            // txtCashAmount
            // 
            this.txtCashAmount.Location = new System.Drawing.Point(748, 115);
            this.txtCashAmount.Name = "txtCashAmount";
            this.txtCashAmount.Size = new System.Drawing.Size(86, 32);
            this.txtCashAmount.TabIndex = 9;
            // 
            // btnAddCheque
            // 
            this.btnAddCheque.Font = new System.Drawing.Font("Noto Kufi Arabic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCheque.Location = new System.Drawing.Point(894, 142);
            this.btnAddCheque.Name = "btnAddCheque";
            this.btnAddCheque.Size = new System.Drawing.Size(150, 37);
            this.btnAddCheque.TabIndex = 13;
            this.btnAddCheque.Text = "اضافة تفاصيل الشيك";
            this.btnAddCheque.UseVisualStyleBackColor = true;
            this.btnAddCheque.Click += new System.EventHandler(this.btnAddCheque_Click);
            // 
            // gridControlCheques
            // 
            this.gridControlCheques.Location = new System.Drawing.Point(48, 625);
            this.gridControlCheques.MainView = this.gridViewCheques;
            this.gridControlCheques.Name = "gridControlCheques";
            this.gridControlCheques.Size = new System.Drawing.Size(996, 200);
            this.gridControlCheques.TabIndex = 15;
            this.gridControlCheques.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCheques});
            // 
            // gridViewCheques
            // 
            this.gridViewCheques.GridControl = this.gridControlCheques;
            this.gridViewCheques.Name = "gridViewCheques";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(643, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 30);
            this.label1.TabIndex = 16;
            this.label1.Text = "دفعة كاش";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 30);
            this.label2.TabIndex = 17;
            this.label2.Text = "المورد";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(391, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 30);
            this.label3.TabIndex = 18;
            this.label3.Text = "طريقة الدفع";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(69, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 30);
            this.label8.TabIndex = 23;
            this.label8.Text = "التاريخ";
            // 
            // cbxPaymentMethod
            // 
            this.cbxPaymentMethod.FormattingEnabled = true;
            this.cbxPaymentMethod.Location = new System.Drawing.Point(485, 115);
            this.cbxPaymentMethod.Name = "cbxPaymentMethod";
            this.cbxPaymentMethod.Size = new System.Drawing.Size(142, 36);
            this.cbxPaymentMethod.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(469, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 30);
            this.label4.TabIndex = 24;
            this.label4.Text = "سند صرف";
            // 
            // searchLookUpEdit1
            // 
            this.searchLookUpEdit1.Location = new System.Drawing.Point(137, 153);
            this.searchLookUpEdit1.Name = "searchLookUpEdit1";
            this.searchLookUpEdit1.Properties.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchLookUpEdit1.Properties.Appearance.Options.UseFont = true;
            this.searchLookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEdit1.Properties.PopupView = this.searchLookUpEdit1View;
            this.searchLookUpEdit1.Size = new System.Drawing.Size(244, 40);
            this.searchLookUpEdit1.TabIndex = 25;
            this.searchLookUpEdit1.EditValueChanged += new System.EventHandler(this.searchLookUpEdit1_EditValueChanged);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // frm_BuyPayment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1077, 831);
            this.Controls.Add(this.searchLookUpEdit1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridControlCheques);
            this.Controls.Add(this.btnAddCheque);
            this.Controls.Add(this.txtCashAmount);
            this.Controls.Add(this.btnSavePayment);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.datePayment);
            this.Controls.Add(this.cbxPaymentMethod);
            this.Controls.Add(this.cbxSupplier);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_BuyPayment";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_BuyPayment";
            this.Load += new System.EventHandler(this.frm_BuyPayment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCheques)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCheques)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxSupplier;
        private System.Windows.Forms.DateTimePicker datePayment;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Button btnSavePayment;
        private System.Windows.Forms.TextBox txtCashAmount;
        private System.Windows.Forms.Button btnAddCheque;
        private DevExpress.XtraGrid.GridControl gridControlCheques;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCheques;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxPaymentMethod;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
    }
}