namespace Accounting.Core.Forms
{
    partial class frm_TrialBalance
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
            this.datefrom = new DevExpress.XtraEditors.DateEdit();
            this.dateto = new DevExpress.XtraEditors.DateEdit();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnshow = new DevExpress.XtraEditors.SimpleButton();
            this.lblTotalDebit = new System.Windows.Forms.Label();
            this.lblTotalCredit = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.datefrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datefrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateto.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // datefrom
            // 
            this.datefrom.EditValue = null;
            this.datefrom.Location = new System.Drawing.Point(121, 66);
            this.datefrom.Name = "datefrom";
            this.datefrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.datefrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.datefrom.Size = new System.Drawing.Size(125, 22);
            this.datefrom.TabIndex = 0;
            // 
            // dateto
            // 
            this.dateto.EditValue = null;
            this.dateto.Location = new System.Drawing.Point(286, 66);
            this.dateto.Name = "dateto";
            this.dateto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateto.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateto.Size = new System.Drawing.Size(125, 22);
            this.dateto.TabIndex = 1;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(37, 94);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(974, 680);
            this.gridControl1.TabIndex = 2;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowFooter = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // btnshow
            // 
            this.btnshow.Location = new System.Drawing.Point(860, 59);
            this.btnshow.Name = "btnshow";
            this.btnshow.Size = new System.Drawing.Size(151, 29);
            this.btnshow.TabIndex = 5;
            this.btnshow.Text = "عرض الميزان";
            this.btnshow.Click += new System.EventHandler(this.btnshow_Click);
            // 
            // lblTotalDebit
            // 
            this.lblTotalDebit.AutoSize = true;
            this.lblTotalDebit.Location = new System.Drawing.Point(538, 783);
            this.lblTotalDebit.Name = "lblTotalDebit";
            this.lblTotalDebit.Size = new System.Drawing.Size(57, 33);
            this.lblTotalDebit.TabIndex = 6;
            this.lblTotalDebit.Text = "label1";
            // 
            // lblTotalCredit
            // 
            this.lblTotalCredit.AutoSize = true;
            this.lblTotalCredit.Location = new System.Drawing.Point(777, 783);
            this.lblTotalCredit.Name = "lblTotalCredit";
            this.lblTotalCredit.Size = new System.Drawing.Size(57, 33);
            this.lblTotalCredit.TabIndex = 7;
            this.lblTotalCredit.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(487, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 33);
            this.label1.TabIndex = 8;
            this.label1.Text = "ميزان المراجعة";
            // 
            // frm_TrialBalance
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1026, 825);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTotalCredit);
            this.Controls.Add(this.lblTotalDebit);
            this.Controls.Add(this.btnshow);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.dateto);
            this.Controls.Add(this.datefrom);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "frm_TrialBalance";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_TrialBalance";
            this.Load += new System.EventHandler(this.frm_TrialBalance_Load);
            ((System.ComponentModel.ISupportInitialize)(this.datefrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datefrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateto.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.DateEdit datefrom;
        private DevExpress.XtraEditors.DateEdit dateto;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton btnshow;
        private System.Windows.Forms.Label lblTotalDebit;
        private System.Windows.Forms.Label lblTotalCredit;
        private System.Windows.Forms.Label label1;
    }
}