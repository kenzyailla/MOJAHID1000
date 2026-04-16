namespace Accounting.Core.Forms
{
    partial class frm_Receipt
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
            this.txtReceiptNumber = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.txtCashAmount = new System.Windows.Forms.TextBox();
            this.cbxPartyType = new System.Windows.Forms.ComboBox();
            this.cbxParty = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReturnCheque = new DevExpress.XtraEditors.SimpleButton();
            this.btnCollectCheque = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.dateReceipt = new System.Windows.Forms.DateTimePicker();
            this.gridCheques = new DevExpress.XtraGrid.GridControl();
            this.gridViewCheques = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.searchLookUpEdit1 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCheques)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCheques)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            this.SuspendLayout();
            // 
            // txtReceiptNumber
            // 
            this.txtReceiptNumber.Location = new System.Drawing.Point(889, 65);
            this.txtReceiptNumber.Multiline = true;
            this.txtReceiptNumber.Name = "txtReceiptNumber";
            this.txtReceiptNumber.Size = new System.Drawing.Size(129, 32);
            this.txtReceiptNumber.TabIndex = 0;
            this.txtReceiptNumber.TextChanged += new System.EventHandler(this.txtReceiptNumber_TextChanged);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(233, 107);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(134, 32);
            this.txtNotes.TabIndex = 5;
            // 
            // txtCashAmount
            // 
            this.txtCashAmount.Location = new System.Drawing.Point(233, 65);
            this.txtCashAmount.Multiline = true;
            this.txtCashAmount.Name = "txtCashAmount";
            this.txtCashAmount.Size = new System.Drawing.Size(134, 33);
            this.txtCashAmount.TabIndex = 4;
            // 
            // cbxPartyType
            // 
            this.cbxPartyType.FormattingEnabled = true;
            this.cbxPartyType.Items.AddRange(new object[] {
            "عميل",
            "مورد"});
            this.cbxPartyType.Location = new System.Drawing.Point(1178, 539);
            this.cbxPartyType.Name = "cbxPartyType";
            this.cbxPartyType.Size = new System.Drawing.Size(19, 41);
            this.cbxPartyType.TabIndex = 2;
            this.cbxPartyType.SelectedIndexChanged += new System.EventHandler(this.cbxPartyType_SelectedIndexChanged);
            // 
            // cbxParty
            // 
            this.cbxParty.FormattingEnabled = true;
            this.cbxParty.Location = new System.Drawing.Point(1062, 655);
            this.cbxParty.Name = "cbxParty";
            this.cbxParty.Size = new System.Drawing.Size(48, 41);
            this.cbxParty.TabIndex = 3;
            this.cbxParty.TextChanged += new System.EventHandler(this.cbxParty_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.searchLookUpEdit1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnReturnCheque);
            this.panel1.Controls.Add(this.btnCollectCheque);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.dateReceipt);
            this.panel1.Controls.Add(this.txtReceiptNumber);
            this.panel1.Controls.Add(this.txtNotes);
            this.panel1.Controls.Add(this.txtCashAmount);
            this.panel1.Location = new System.Drawing.Point(34, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1126, 147);
            this.panel1.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(373, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 33);
            this.label5.TabIndex = 15;
            this.label5.Text = "قيمة الشيكات";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(387, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 33);
            this.label4.TabIndex = 14;
            this.label4.Text = "القيمة الكاش";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1029, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 33);
            this.label3.TabIndex = 14;
            this.label3.Text = "اسم العميل";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(649, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 33);
            this.label2.TabIndex = 13;
            this.label2.Text = "تاريخ السند";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1029, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 33);
            this.label1.TabIndex = 12;
            this.label1.Text = "رقم السند";
            // 
            // btnReturnCheque
            // 
            this.btnReturnCheque.Location = new System.Drawing.Point(52, 105);
            this.btnReturnCheque.Name = "btnReturnCheque";
            this.btnReturnCheque.Size = new System.Drawing.Size(162, 33);
            this.btnReturnCheque.TabIndex = 11;
            this.btnReturnCheque.Text = "إرجاع الشيك";
            this.btnReturnCheque.Click += new System.EventHandler(this.btnReturnCheque_Click);
            // 
            // btnCollectCheque
            // 
            this.btnCollectCheque.Location = new System.Drawing.Point(52, 74);
            this.btnCollectCheque.Name = "btnCollectCheque";
            this.btnCollectCheque.Size = new System.Drawing.Size(162, 24);
            this.btnCollectCheque.TabIndex = 9;
            this.btnCollectCheque.Text = "تحصيل الشيك";
            this.btnCollectCheque.Click += new System.EventHandler(this.btnCollectCheque_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Noto Kufi Arabic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(50, 24);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(154, 38);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "حفظ سند القبض";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dateReceipt
            // 
            this.dateReceipt.Location = new System.Drawing.Point(514, 111);
            this.dateReceipt.Name = "dateReceipt";
            this.dateReceipt.Size = new System.Drawing.Size(129, 36);
            this.dateReceipt.TabIndex = 1;
            this.dateReceipt.Value = new System.DateTime(2026, 4, 5, 0, 0, 0, 0);
            // 
            // gridCheques
            // 
            this.gridCheques.Location = new System.Drawing.Point(34, 165);
            this.gridCheques.MainView = this.gridViewCheques;
            this.gridCheques.Name = "gridCheques";
            this.gridCheques.Size = new System.Drawing.Size(1126, 474);
            this.gridCheques.TabIndex = 10;
            this.gridCheques.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCheques});
            // 
            // gridViewCheques
            // 
            this.gridViewCheques.GridControl = this.gridCheques;
            this.gridViewCheques.Name = "gridViewCheques";
            this.gridViewCheques.OptionsView.ShowFooter = true;
            // 
            // searchLookUpEdit1
            // 
            this.searchLookUpEdit1.Location = new System.Drawing.Point(774, 110);
            this.searchLookUpEdit1.Name = "searchLookUpEdit1";
            this.searchLookUpEdit1.Properties.Appearance.Font = new System.Drawing.Font("Noto Kufi Arabic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchLookUpEdit1.Properties.Appearance.Options.UseFont = true;
            this.searchLookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEdit1.Properties.PopupView = this.searchLookUpEdit1View;
            this.searchLookUpEdit1.Size = new System.Drawing.Size(244, 34);
            this.searchLookUpEdit1.TabIndex = 17;
            this.searchLookUpEdit1.EditValueChanged += new System.EventHandler(this.searchLookUpEdit1_EditValueChanged);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Noto Kufi Arabic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label6.Location = new System.Drawing.Point(559, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 37);
            this.label6.TabIndex = 14;
            this.label6.Text = "سند قبض";
            // 
            // frm_Receipt
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1230, 708);
            this.Controls.Add(this.gridCheques);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbxPartyType);
            this.Controls.Add(this.cbxParty);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "frm_Receipt";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_Receipt";
            this.Load += new System.EventHandler(this.frm_Receipt_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCheques)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCheques)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtReceiptNumber;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtCashAmount;
        private System.Windows.Forms.ComboBox cbxPartyType;
        private System.Windows.Forms.ComboBox cbxParty;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dateReceipt;
        private DevExpress.XtraGrid.GridControl gridCheques;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCheques;
        private DevExpress.XtraEditors.SimpleButton btnReturnCheque;
        private DevExpress.XtraEditors.SimpleButton btnCollectCheque;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private System.Windows.Forms.Label label6;
    }
}