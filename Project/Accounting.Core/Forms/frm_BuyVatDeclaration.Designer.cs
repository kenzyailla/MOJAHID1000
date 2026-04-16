namespace Accounting.Core.Forms
{
    partial class frm_BuyVatDeclaration
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
            this.btnCalculate = new System.Windows.Forms.Button();
            this.dateTo = new DevExpress.XtraEditors.DateEdit();
            this.dateFrom = new DevExpress.XtraEditors.DateEdit();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblBuy16Before = new System.Windows.Forms.Label();
            this.lblBuy4Before = new System.Windows.Forms.Label();
            this.lblBuy1Before = new System.Windows.Forms.Label();
            this.lblBuy16Tax = new System.Windows.Forms.Label();
            this.lblBuy16After = new System.Windows.Forms.Label();
            this.lblBuy4After = new System.Windows.Forms.Label();
            this.lblBuy4Tax = new System.Windows.Forms.Label();
            this.lblBuy1Tax = new System.Windows.Forms.Label();
            this.lblBuy1After = new System.Windows.Forms.Label();
            this.lblBuy0After = new System.Windows.Forms.Label();
            this.lblBuy0Tax = new System.Windows.Forms.Label();
            this.lblBuy0Before = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(16, 69);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(838, 449);
            this.gridControl1.TabIndex = 13;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(729, 12);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(125, 51);
            this.btnCalculate.TabIndex = 12;
            this.btnCalculate.Text = "احتساب الإقرار";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // dateTo
            // 
            this.dateTo.EditValue = null;
            this.dateTo.Location = new System.Drawing.Point(195, 38);
            this.dateTo.Name = "dateTo";
            this.dateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateTo.Size = new System.Drawing.Size(142, 22);
            this.dateTo.TabIndex = 11;
            // 
            // dateFrom
            // 
            this.dateFrom.EditValue = null;
            this.dateFrom.Location = new System.Drawing.Point(16, 38);
            this.dateFrom.Name = "dateFrom";
            this.dateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateFrom.Size = new System.Drawing.Size(142, 22);
            this.dateFrom.TabIndex = 10;
            // 
            // groupControl1
            // 
            this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl1.AppearanceCaption.Options.UseFont = true;
            this.groupControl1.Controls.Add(this.tableLayoutPanel1);
            this.groupControl1.Location = new System.Drawing.Point(25, 524);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(436, 189);
            this.groupControl1.TabIndex = 20;
            this.groupControl1.Text = "مشتريات";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy16Before, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy4Before, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy1Before, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy16Tax, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy16After, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy4After, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy4Tax, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy1Tax, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy1After, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy0After, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy0Tax, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblBuy0Before, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label19, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label20, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 34);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(432, 153);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(371, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "النسبة";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(227, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 31);
            this.label2.TabIndex = 1;
            this.label2.Text = "قبل الضريبة";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(108, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 31);
            this.label3.TabIndex = 2;
            this.label3.Text = "قيمة الضريبة";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 31);
            this.label4.TabIndex = 3;
            this.label4.Text = "بعد الضريبة";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(390, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "16%";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(398, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "4%";
            // 
            // lblBuy16Before
            // 
            this.lblBuy16Before.AutoSize = true;
            this.lblBuy16Before.Location = new System.Drawing.Point(276, 33);
            this.lblBuy16Before.Name = "lblBuy16Before";
            this.lblBuy16Before.Size = new System.Drawing.Size(42, 17);
            this.lblBuy16Before.TabIndex = 6;
            this.lblBuy16Before.Text = "label7";
            // 
            // lblBuy4Before
            // 
            this.lblBuy4Before.AutoSize = true;
            this.lblBuy4Before.Location = new System.Drawing.Point(276, 65);
            this.lblBuy4Before.Name = "lblBuy4Before";
            this.lblBuy4Before.Size = new System.Drawing.Size(42, 17);
            this.lblBuy4Before.TabIndex = 7;
            this.lblBuy4Before.Text = "label8";
            // 
            // lblBuy1Before
            // 
            this.lblBuy1Before.AutoSize = true;
            this.lblBuy1Before.Location = new System.Drawing.Point(276, 95);
            this.lblBuy1Before.Name = "lblBuy1Before";
            this.lblBuy1Before.Size = new System.Drawing.Size(42, 17);
            this.lblBuy1Before.TabIndex = 8;
            this.lblBuy1Before.Text = "label9";
            // 
            // lblBuy16Tax
            // 
            this.lblBuy16Tax.AutoSize = true;
            this.lblBuy16Tax.Location = new System.Drawing.Point(158, 33);
            this.lblBuy16Tax.Name = "lblBuy16Tax";
            this.lblBuy16Tax.Size = new System.Drawing.Size(50, 17);
            this.lblBuy16Tax.TabIndex = 9;
            this.lblBuy16Tax.Text = "label10";
            // 
            // lblBuy16After
            // 
            this.lblBuy16After.AutoSize = true;
            this.lblBuy16After.Location = new System.Drawing.Point(49, 33);
            this.lblBuy16After.Name = "lblBuy16After";
            this.lblBuy16After.Size = new System.Drawing.Size(50, 17);
            this.lblBuy16After.TabIndex = 10;
            this.lblBuy16After.Text = "label11";
            // 
            // lblBuy4After
            // 
            this.lblBuy4After.AutoSize = true;
            this.lblBuy4After.Location = new System.Drawing.Point(49, 65);
            this.lblBuy4After.Name = "lblBuy4After";
            this.lblBuy4After.Size = new System.Drawing.Size(50, 17);
            this.lblBuy4After.TabIndex = 11;
            this.lblBuy4After.Text = "label12";
            // 
            // lblBuy4Tax
            // 
            this.lblBuy4Tax.AutoSize = true;
            this.lblBuy4Tax.Location = new System.Drawing.Point(158, 65);
            this.lblBuy4Tax.Name = "lblBuy4Tax";
            this.lblBuy4Tax.Size = new System.Drawing.Size(50, 17);
            this.lblBuy4Tax.TabIndex = 12;
            this.lblBuy4Tax.Text = "label13";
            // 
            // lblBuy1Tax
            // 
            this.lblBuy1Tax.AutoSize = true;
            this.lblBuy1Tax.Location = new System.Drawing.Point(158, 95);
            this.lblBuy1Tax.Name = "lblBuy1Tax";
            this.lblBuy1Tax.Size = new System.Drawing.Size(50, 17);
            this.lblBuy1Tax.TabIndex = 13;
            this.lblBuy1Tax.Text = "label14";
            // 
            // lblBuy1After
            // 
            this.lblBuy1After.AutoSize = true;
            this.lblBuy1After.Location = new System.Drawing.Point(49, 95);
            this.lblBuy1After.Name = "lblBuy1After";
            this.lblBuy1After.Size = new System.Drawing.Size(50, 17);
            this.lblBuy1After.TabIndex = 14;
            this.lblBuy1After.Text = "label15";
            // 
            // lblBuy0After
            // 
            this.lblBuy0After.AutoSize = true;
            this.lblBuy0After.Location = new System.Drawing.Point(49, 122);
            this.lblBuy0After.Name = "lblBuy0After";
            this.lblBuy0After.Size = new System.Drawing.Size(50, 17);
            this.lblBuy0After.TabIndex = 15;
            this.lblBuy0After.Text = "label16";
            // 
            // lblBuy0Tax
            // 
            this.lblBuy0Tax.AutoSize = true;
            this.lblBuy0Tax.Location = new System.Drawing.Point(158, 122);
            this.lblBuy0Tax.Name = "lblBuy0Tax";
            this.lblBuy0Tax.Size = new System.Drawing.Size(50, 17);
            this.lblBuy0Tax.TabIndex = 16;
            this.lblBuy0Tax.Text = "label17";
            // 
            // lblBuy0Before
            // 
            this.lblBuy0Before.AutoSize = true;
            this.lblBuy0Before.Location = new System.Drawing.Point(268, 122);
            this.lblBuy0Before.Name = "lblBuy0Before";
            this.lblBuy0Before.Size = new System.Drawing.Size(50, 17);
            this.lblBuy0Before.TabIndex = 17;
            this.lblBuy0Before.Text = "label18";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(398, 122);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(30, 17);
            this.label19.TabIndex = 18;
            this.label19.Text = "0%";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(398, 95);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(30, 17);
            this.label20.TabIndex = 19;
            this.label20.Text = "1%";
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.Controls.Add(this.tableLayoutPanel2);
            this.groupControl2.Location = new System.Drawing.Point(467, 524);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(387, 189);
            this.groupControl2.TabIndex = 21;
            this.groupControl2.Text = "مرتجعات مشتريات";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label12, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label13, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label14, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label15, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label16, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label17, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label18, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label21, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.label22, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.label23, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.label24, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label25, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.label26, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.label27, 3, 3);
            this.tableLayoutPanel2.Controls.Add(this.label28, 3, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 34);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(383, 153);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(331, 1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "النسبة";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(341, 33);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "16%";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(349, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 17);
            this.label9.TabIndex = 2;
            this.label9.Text = "4%";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(349, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 17);
            this.label10.TabIndex = 3;
            this.label10.Text = "1%";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(349, 122);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(30, 17);
            this.label11.TabIndex = 4;
            this.label11.Text = "0%";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(219, 1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 17);
            this.label12.TabIndex = 5;
            this.label12.Text = "قبل الضريبة";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(126, 1);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 17);
            this.label13.TabIndex = 6;
            this.label13.Text = "قيمة الضريبة";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(243, 33);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(50, 17);
            this.label14.TabIndex = 7;
            this.label14.Text = "label14";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(243, 65);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 17);
            this.label15.TabIndex = 8;
            this.label15.Text = "label15";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(243, 95);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(50, 17);
            this.label16.TabIndex = 9;
            this.label16.Text = "label16";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(243, 122);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 17);
            this.label17.TabIndex = 10;
            this.label17.Text = "label17";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(157, 33);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 17);
            this.label18.TabIndex = 11;
            this.label18.Text = "label18";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(157, 65);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(50, 17);
            this.label21.TabIndex = 12;
            this.label21.Text = "label21";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(157, 95);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(50, 17);
            this.label22.TabIndex = 13;
            this.label22.Text = "label22";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(157, 122);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(50, 17);
            this.label23.TabIndex = 14;
            this.label23.Text = "label23";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(26, 1);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 17);
            this.label24.TabIndex = 15;
            this.label24.Text = "بعد الضريبة";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(48, 33);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(50, 17);
            this.label25.TabIndex = 16;
            this.label25.Text = "label25";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(48, 65);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(50, 17);
            this.label26.TabIndex = 17;
            this.label26.Text = "label26";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(48, 95);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(50, 17);
            this.label27.TabIndex = 18;
            this.label27.Text = "label27";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(48, 122);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(50, 17);
            this.label28.TabIndex = 19;
            this.label28.Text = "label28";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.Color.Blue;
            this.label29.Location = new System.Drawing.Point(407, 13);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(123, 33);
            this.label29.TabIndex = 22;
            this.label29.Text = "ضريبة المشتريات";
            // 
            // frm_BuyVatDeclaration
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(880, 743);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.dateTo);
            this.Controls.Add(this.dateFrom);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_BuyVatDeclaration";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_BuyVatDeclaration";
            this.Load += new System.EventHandler(this.frm_BuyVatDeclaration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Button btnCalculate;
        private DevExpress.XtraEditors.DateEdit dateTo;
        private DevExpress.XtraEditors.DateEdit dateFrom;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblBuy16Before;
        private System.Windows.Forms.Label lblBuy4Before;
        private System.Windows.Forms.Label lblBuy1Before;
        private System.Windows.Forms.Label lblBuy16Tax;
        private System.Windows.Forms.Label lblBuy16After;
        private System.Windows.Forms.Label lblBuy4After;
        private System.Windows.Forms.Label lblBuy4Tax;
        private System.Windows.Forms.Label lblBuy1Tax;
        private System.Windows.Forms.Label lblBuy1After;
        private System.Windows.Forms.Label lblBuy0After;
        private System.Windows.Forms.Label lblBuy0Tax;
        private System.Windows.Forms.Label lblBuy0Before;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
    }
}