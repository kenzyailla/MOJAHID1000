namespace Accounting.Core
{
    partial class frm_Dashboard
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
            this.lblCash = new System.Windows.Forms.Label();
            this.lblBank = new System.Windows.Forms.Label();
            this.lblSales = new System.Windows.Forms.Label();
            this.lblProfit = new System.Windows.Forms.Label();
            this.lblCheques = new System.Windows.Forms.Label();
            this.lblInvoices = new System.Windows.Forms.Label();
            this.chartSales = new DevExpress.XtraCharts.ChartControl();
            this.chartTopProducts = new DevExpress.XtraCharts.ChartControl();
            this.chartExpenses = new DevExpress.XtraCharts.ChartControl();
            this.chartStock = new DevExpress.XtraCharts.ChartControl();
            this.chartPurchases = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTopProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartExpenses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPurchases)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCash
            // 
            this.lblCash.AutoSize = true;
            this.lblCash.Location = new System.Drawing.Point(210, 124);
            this.lblCash.Name = "lblCash";
            this.lblCash.Size = new System.Drawing.Size(0, 33);
            this.lblCash.TabIndex = 0;
            // 
            // lblBank
            // 
            this.lblBank.AutoSize = true;
            this.lblBank.Location = new System.Drawing.Point(494, 139);
            this.lblBank.Name = "lblBank";
            this.lblBank.Size = new System.Drawing.Size(0, 33);
            this.lblBank.TabIndex = 1;
            // 
            // lblSales
            // 
            this.lblSales.AutoSize = true;
            this.lblSales.Location = new System.Drawing.Point(624, 124);
            this.lblSales.Name = "lblSales";
            this.lblSales.Size = new System.Drawing.Size(0, 33);
            this.lblSales.TabIndex = 2;
            // 
            // lblProfit
            // 
            this.lblProfit.AutoSize = true;
            this.lblProfit.Location = new System.Drawing.Point(770, 124);
            this.lblProfit.Name = "lblProfit";
            this.lblProfit.Size = new System.Drawing.Size(0, 33);
            this.lblProfit.TabIndex = 3;
            // 
            // lblCheques
            // 
            this.lblCheques.AutoSize = true;
            this.lblCheques.Location = new System.Drawing.Point(865, 124);
            this.lblCheques.Name = "lblCheques";
            this.lblCheques.Size = new System.Drawing.Size(0, 33);
            this.lblCheques.TabIndex = 4;
            // 
            // lblInvoices
            // 
            this.lblInvoices.AutoSize = true;
            this.lblInvoices.Location = new System.Drawing.Point(688, 252);
            this.lblInvoices.Name = "lblInvoices";
            this.lblInvoices.Size = new System.Drawing.Size(0, 33);
            this.lblInvoices.TabIndex = 5;
            // 
            // chartSales
            // 
            this.chartSales.Location = new System.Drawing.Point(56, 183);
            this.chartSales.Name = "chartSales";
            this.chartSales.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartSales.Size = new System.Drawing.Size(344, 305);
            this.chartSales.TabIndex = 6;
            this.chartSales.ObjectSelected += new DevExpress.XtraCharts.HotTrackEventHandler(this.chartSales_ObjectSelected);
            // 
            // chartTopProducts
            // 
            this.chartTopProducts.Location = new System.Drawing.Point(56, 494);
            this.chartTopProducts.Name = "chartTopProducts";
            this.chartTopProducts.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartTopProducts.Size = new System.Drawing.Size(1393, 383);
            this.chartTopProducts.TabIndex = 7;
            this.chartTopProducts.ObjectSelected += new DevExpress.XtraCharts.HotTrackEventHandler(this.chartTopProducts_ObjectSelected);
            // 
            // chartExpenses
            // 
            this.chartExpenses.Location = new System.Drawing.Point(1105, 183);
            this.chartExpenses.Name = "chartExpenses";
            this.chartExpenses.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartExpenses.Size = new System.Drawing.Size(344, 305);
            this.chartExpenses.TabIndex = 8;
            // 
            // chartStock
            // 
            this.chartStock.Location = new System.Drawing.Point(755, 182);
            this.chartStock.Name = "chartStock";
            this.chartStock.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartStock.Size = new System.Drawing.Size(344, 305);
            this.chartStock.TabIndex = 9;
            // 
            // chartPurchases
            // 
            this.chartPurchases.Location = new System.Drawing.Point(405, 182);
            this.chartPurchases.Name = "chartPurchases";
            this.chartPurchases.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartPurchases.Size = new System.Drawing.Size(344, 305);
            this.chartPurchases.TabIndex = 10;
            this.chartPurchases.ObjectSelected += new DevExpress.XtraCharts.HotTrackEventHandler(this.chartPurchases_ObjectSelected);
            // 
            // frm_Dashboard
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1476, 956);
            this.Controls.Add(this.chartPurchases);
            this.Controls.Add(this.chartStock);
            this.Controls.Add(this.chartExpenses);
            this.Controls.Add(this.chartTopProducts);
            this.Controls.Add(this.chartSales);
            this.Controls.Add(this.lblInvoices);
            this.Controls.Add(this.lblCheques);
            this.Controls.Add(this.lblProfit);
            this.Controls.Add(this.lblSales);
            this.Controls.Add(this.lblBank);
            this.Controls.Add(this.lblCash);
            this.Font = new System.Drawing.Font("Noto Kufi Arabic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frm_Dashboard";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_Dashboard";
            this.Load += new System.EventHandler(this.frm_Dashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTopProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartExpenses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPurchases)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCash;
        private System.Windows.Forms.Label lblBank;
        private System.Windows.Forms.Label lblSales;
        private System.Windows.Forms.Label lblProfit;
        private System.Windows.Forms.Label lblCheques;
        private System.Windows.Forms.Label lblInvoices;
        private DevExpress.XtraCharts.ChartControl chartSales;
        private DevExpress.XtraCharts.ChartControl chartTopProducts;
        private DevExpress.XtraCharts.ChartControl chartExpenses;
        private DevExpress.XtraCharts.ChartControl chartStock;
        private DevExpress.XtraCharts.ChartControl chartPurchases;
    }
}