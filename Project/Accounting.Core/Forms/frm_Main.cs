using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Accounting.Core.Forms;


namespace Accounting.Core.Forms
{
    public partial class frm_Main : Form
    {

        public static Action DashboardNeedsRefresh;
        private Panel panelSidebar;
        private Panel panelMenu;
        private Panel panelContent;
        private Form currentForm;
        private bool isCollapsed = false;
        private string _connectionString =
@"Data Source=.\SQLEXPRESS;
Initial Catalog=AccountingCoreDB;
Integrated Security=True";

        private readonly string connectionString =
    @"Data Source=.\SQLEXPRESS;
      Initial Catalog=AccountingCoreDB;
      Integrated Security=True";

        public frm_Main()
        {
            InitializeComponent();
            BuildLayout();
            // 🔥 تشغيل التنبيه
            this.Load += frm_Main_Load;
        }
        private void RestoreBackup()
        {
            var result = MessageBox.Show(
                "⚠️ سيتم استبدال قاعدة البيانات الحالية\nهل تريد المتابعة؟",
                "تأكيد الاسترجاع",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Backup Files (*.bak)|*.bak";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = ofd.FileName;

                using (SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True"))
                {
                    con.Open();

                    // 🔥 اغلاق كل الاتصالات
                    string setSingleUser = @"
ALTER DATABASE AccountingCoreDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    new SqlCommand(setSingleUser, con).ExecuteNonQuery();

                    // 🔥 الاسترجاع
                    string restore = $@"
RESTORE DATABASE AccountingCoreDB 
FROM DISK = '{filePath}' 
WITH REPLACE";
                    new SqlCommand(restore, con).ExecuteNonQuery();

                    // 🔥 رجوع Multi User
                    string setMultiUser = @"
ALTER DATABASE AccountingCoreDB SET MULTI_USER";
                    new SqlCommand(setMultiUser, con).ExecuteNonQuery();
                }

                MessageBox.Show("تم استرجاع النسخة بنجاح ✔️");

                // 🔥 الآن أغلق البرنامج بعد النجاح
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BackupNow()
        {
            try
            {
                string projectPath = @"C:\Users\HP\source\repos\Accounting.Core";
                //string backupRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\MyBackups";
                string backupRoot = @"C:\Backups";
                if (!Directory.Exists(backupRoot))
                    Directory.CreateDirectory(backupRoot);
                string today = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                string backupFolder = Path.Combine(backupRoot, "Backup_" + today);

                Directory.CreateDirectory(backupFolder);

                CopyDirectory(projectPath, Path.Combine(backupFolder, "Project"));

                using (SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True"))
                {
                    con.Open();

                    string dbPath = Path.Combine(backupFolder, "AccountingCoreDB.bak");

                    string sql = $@"
BACKUP DATABASE AccountingCoreDB 
TO DISK = '{dbPath}' 
WITH INIT";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("تم إنشاء النسخة الاحتياطية بنجاح ✔️");
                MessageBox.Show("تم الحفظ في:\n" + backupFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);

                // 🔥 تجاهل الملفات المقفلة أو غير المهمة
                if (fileName.EndsWith(".lock") ||
                    fileName.EndsWith(".suo") ||
                    fileName.EndsWith(".db"))
                    continue;

                try
                {
                    string dest = Path.Combine(destinationDir, fileName);
                    File.Copy(file, dest, true);
                }
                catch
                {
                    // تجاهل أي ملف مقفول
                }
            }

            foreach (string folder in Directory.GetDirectories(sourceDir))
            {
                string folderName = Path.GetFileName(folder);

                // 🔥 تجاهل مجلدات ثقيلة
                if (folderName == ".vs" ||
                    folderName == "bin" ||
                    folderName == "obj")
                    continue;

                string dest = Path.Combine(destinationDir, folderName);
                CopyDirectory(folder, dest);
            }
        }

        private void CheckChequesReminder()
        {
            // 🔥 لا تعرضه إذا ظهر اليوم
            if (Properties.Settings.Default.LastChequeReminderDate.Date == DateTime.Today)
                return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
        SELECT ChequeNumber, SupplierName, DueDate
        FROM SupplierCheques
        WHERE IsCleared = 0
        AND DueDate <= @today";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@today", DateTime.Today);

                SqlDataReader dr = cmd.ExecuteReader();

                string msg = "";

                while (dr.Read())
                {
                    msg += "🔸 شيك رقم: " + dr["ChequeNumber"] +
                           "\nالمورد: " + dr["SupplierName"] +
                           "\nالتاريخ: " + Convert.ToDateTime(dr["DueDate"]).ToString("dd/MM/yyyy") +
                           "\n------------------------\n";
                }

                if (msg != "")
                {
                    MessageBox.Show("🔔 شيكات مستحقة:\n\n" + msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // 🔥 حفظ تاريخ اليوم
                    Properties.Settings.Default.LastChequeReminderDate = DateTime.Today;
                    Properties.Settings.Default.Save();
                }
            }
        }
        private void AddMenuButton(Panel parent, string text, Action action)
        {
          
            Button btn = new Button();
            btn.Text = text;
            btn.Dock = DockStyle.Top;
            btn.Height = 40;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.Gainsboro;
            btn.BackColor = Color.FromArgb(30, 41, 59);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(35, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(0, 87, 231);

            btn.MouseLeave += (s, e) =>
                btn.BackColor = Color.FromArgb(30, 41, 59);

            btn.Click += (s, e) => action?.Invoke();

            parent.Controls.Add(btn);
            parent.Controls.SetChildIndex(btn, 0);
        }
        private void OpenForm(Form form)
        {
            
        
            if (currentForm != null)
            {
                currentForm.Close();
                currentForm.Dispose();
            }

            currentForm = form;

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelContent.Controls.Clear();
            panelContent.Controls.Add(form);

            form.BringToFront();
            form.Show();

        }
        private void BuildLayout()
        {
            this.Text = "Accounting.Core ERP";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.White;
            this.Controls.Clear();

            Font appFont = new Font("Noto Kufi Arabic", 9F);
            this.Font = appFont;

            // ================= SIDEBAR =================
            panelSidebar = new Panel();
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Width = 300;
            panelSidebar.BackColor = Color.FromArgb(30, 41, 59);
            panelSidebar.Margin = new Padding(0);
            panelSidebar.Padding = new Padding(0);
            this.Controls.Add(panelSidebar);

            // ===== Collapse Button =====
            Button btnMenu = new Button();
            btnMenu.Text = "☰";
            btnMenu.Dock = DockStyle.Top;
            btnMenu.Height = 50;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.ForeColor = Color.White;
            btnMenu.BackColor = Color.FromArgb(214, 45, 32);
            btnMenu.Font = new Font("Noto Kufi Arabic", 12F, FontStyle.Bold);
            panelSidebar.Controls.Add(btnMenu);

            // ===== Scrollable Menu Panel =====
            panelMenu = new Panel();
            panelMenu.Dock = DockStyle.Fill;
            panelMenu.AutoScroll = true;
            panelMenu.BackColor = panelSidebar.BackColor;
            panelSidebar.Controls.Add(panelMenu);
            panelSidebar.Controls.SetChildIndex(panelMenu, 1);

            // ================= CONTENT =================
            panelContent = new Panel();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BackColor = Color.White;
            panelContent.Padding = new Padding(10);
            this.Controls.Add(panelContent);

            // ================= MENU BUTTONS =================
            // ================= Dashboard =================
            var dash = AddMenuSection("لوحة التحكم");
            AddMenuButton(dash, "📊 الرئيسية", () => OpenForm(new frm_Dashboard()));
            // ================= HEADER =================
            Panel panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 65;
            panelHeader.BackColor = Color.FromArgb(15, 23, 42); // Dark modern
            panelHeader.Padding = new Padding(20, 0, 20, 0);
            this.Controls.Add(panelHeader);



            Label lblTitle = new Label();
            lblTitle.Text = "Accounting.Core ERP System";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Noto Kufi Arabic", 14F, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 18);

            panelHeader.Controls.Add(lblTitle);

            Label lblDateTime = new Label();
            lblDateTime.ForeColor = Color.Gainsboro;
            lblDateTime.Font = new Font("Noto Kufi Arabic", 9F);
            lblDateTime.AutoSize = true;
            lblDateTime.Location = new Point(800, 22); // عدل حسب عرض الشاشة

            panelHeader.Controls.Add(lblDateTime);

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
            };
            timer.Start();

            Label lblUser = new Label();
            lblUser.Text = "👤 Admin";
            lblUser.ForeColor = Color.White;
            lblUser.Font = new Font("Noto Kufi Arabic", 10F);
            lblUser.AutoSize = true;
            lblUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUser.Location = new Point(panelHeader.Width - 120, 22);

            panelHeader.Controls.Add(lblUser);

            Button btnLogout = new Button();
            btnLogout.Text = "🏦";
            btnLogout.Width = 40;
            btnLogout.Height = 35;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.BackColor = Color.FromArgb(220, 38, 38);
            btnLogout.ForeColor = Color.White;
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.Location = new Point(panelHeader.Width - 50, 15);

            btnLogout.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    "هل تريد تسجيل الخروج؟",
                    "تأكيد",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };

            panelHeader.Controls.Add(btnLogout);



            // ================= Sales =================
            var sales = AddMenuSection("المبيعات");
            AddMenuButton(sales, "💰 الفواتير", () => OpenForm(new frm_Invoices()));
            AddMenuButton(sales, "💰 قائمة الفواتير", () => OpenForm(new frm_InvoicesList()));
            AddMenuButton(sales, "📈 تقارير المبيعات", () => OpenForm(new frm_SalesReportAdvanced()));
            //AddMenuButton(sales, "📈 مرتجع مبيعات فوترة", () => OpenForm(new frm_CreditNote()));
            // إذا عندك مرتجع مبيعات لاحقاً:
            AddMenuButton(sales, "↩️ مرتجع مبيعات",
        () => OpenForm(new frm_SalesReturn()));
            AddMenuButton(sales, "📋 تقارير مرتجعات مبيعات",
    () => OpenForm(new frm_SalesReturnsList()));
            // ================= Purchases =================
            var buys = AddMenuSection("المشتريات");
            AddMenuButton(buys, "🛒 فواتير المشتريات", () => OpenForm(new frm_BuyInvoices()));
            AddMenuButton(buys, "📑 تقرير المشتريات", () => OpenForm(new frm_BuyReportAdvanced()));
           
            AddMenuButton(buys, "💵 مرتجع مشتريات", () => OpenForm(new frm_BuyReturn()));
            AddMenuButton(buys, "💵 تقارير مرتجعات مشتريات", () => OpenForm(new frm_BuyReturnsReport()));
          

            // ================= Customers =================
            var customers = AddMenuSection("العملاء");
            AddMenuButton(customers, "👥 إدارة العملاء", () => OpenForm(new frm_Customers()));
            AddMenuButton(customers, "💵 سند قبض", () => OpenForm(new frm_Receipt()));
            AddMenuButton(customers, "👥 ذمم العملاء", () => OpenForm(new frm_CustomersBalances()));
            AddMenuButton(customers, "👥 تقسيم الديون حسب العمر", () => OpenForm(new frm_CustomersAging()));
            AddMenuButton(customers, "📄 كشف حساب عميل (مختصر)", () => OpenForm(new frm_CustomerStatement()));
            // ================= Suppliers =================
            var suppliers = AddMenuSection("الموردين");
            AddMenuButton(suppliers, "🏬 إدارة الموردين", () => OpenForm(new frm_Suppliers()));
            AddMenuButton(suppliers, "💵 سند صرف", () => OpenForm(new frm_payment()));
            AddMenuButton(suppliers, "📄 كشف حساب مورد", () => OpenForm(new frm_SupplierLedger()));
            AddMenuButton(suppliers, "📄 كشف حساب جميع الموردين", () => OpenForm(new frm_SuppliersLedgerSummary()));
            // ================= Inventory =================
            var inv = AddMenuSection("المخزون");
            AddMenuButton(inv, "📦 المنتجات", () => OpenForm(new frm_Products()));
            AddMenuButton(inv, "🏷️ كارت الصنف", () => OpenForm(new frm_ProductCard()));
            AddMenuButton(inv, "📊 تقرير المخزون", () => OpenForm(new frm_InventoryMasterReport()));
            AddMenuButton(inv, "🟢 رصيد افتتاحي", () => OpenForm(new frm_OpeningStock()));
            AddMenuButton(inv, "📊 الأرصدة حسب تاريخ", () => OpenForm(new frm_StockBalanceByDate()));
            AddMenuButton(inv, "📊 حركة مادة تفصيلية", () => OpenForm(new frm_ProductLedger()));
            AddMenuButton(inv, "📊 تقرير كشف حركة مادة", () => OpenForm(new frm_ProductMovement()));
            AddMenuButton(inv, "📊 استهلاك مواد للتصنيع", () => OpenForm(new frm_MaterialConsumption()));

            AddMenuButton(inv, "➕ تعديلات المخزون", () => OpenForm(new frm_StockAdjustmentcs()));
            AddMenuButton(inv, "📦 ملخص المخزون", () => OpenForm(new frm_StockSummary()));
            // ================= Accounting =================
            var acc = AddMenuSection("المحاسبة");
            AddMenuButton(acc, "💾 نسخة احتياطية", () => BackupNow());
            AddMenuButton(acc, "📂 استرجاع نسخة", () => RestoreBackup());
            AddMenuButton(acc, "📊 ميزان المراجعة", () => OpenForm(new frm_TrialBalance()));
            AddMenuButton(acc, "📊 ضريبة مبيعات", () => OpenForm(new frm_SalesVatDeclaration(connectionString)));
            AddMenuButton(acc, "🧾 ضريبة مشتريات", () => OpenForm(new frm_BuyVatDeclaration()));
            AddMenuButton(acc, "🧾 شاشة البورد", () => OpenForm(new frm_Dashboard()));
            AddMenuButton(acc, "🧾 ملخص الضريبة", () => OpenForm(new frm_VatSummary()));

            var expe = AddMenuSection("المصاريف");
            AddMenuButton(expe, "🏦 ادخال المصاريف", () => OpenForm(new frm_masrouf()));
            AddMenuButton(expe, "🧾 عرض المصروفات ", () => OpenForm(new frm_Expenses()));
            //AddMenuButton(cheq, "🧾  ادخال الشيكات قبل العمل على النظام", () => OpenForm(new frm_AddCheque()));
            //AddMenuButton(cheq, "🧾   الشيكات قبل العمل على النظام", () => OpenForm(new frm_SupplierCheques()));


            // ================= Cheques (عام) =================
            var cheq = AddMenuSection("الشيكات");
           AddMenuButton(cheq, "🏦 الشيكات", () => OpenForm(new frm_Cheques()));
            AddMenuButton(cheq, "🏦 البنك", () => OpenForm(new frm_Bank()));
            AddMenuButton(cheq, "🧾 الشيكات الصادرة", () => OpenForm(new frm_OutgoingCheques()));
            AddMenuButton(cheq, "🧾  ادخال الشيكات قبل العمل على النظام", () => OpenForm(new frm_AddCheque()));
            AddMenuButton(cheq, "🧾   الشيكات قبل العمل على النظام", () => OpenForm(new frm_SupplierCheques()));
            AddMenuButton(cheq, "🧾   التحويل من البنك للصندوق والعكس", () => OpenForm(new frm_TransferBetweenAccounts()));
            AddMenuButton(cheq, "🧾   الصندوق", () => OpenForm(new frm_CashLedger()));

            // ===== Collapse Logic =====
            btnMenu.Click += (s, e) =>
            {
                panelSidebar.Width = isCollapsed ? 300 : 65;
                isCollapsed = !isCollapsed;
                foreach (Control section in panelMenu.Controls)
                {
                    if (section is Panel secPanel)
                    {
                        foreach (Control ctrl in secPanel.Controls)
                        {
                            if (ctrl is Button b)
                            {
                                if (isCollapsed)
                                {
                                    b.TextAlign = ContentAlignment.MiddleCenter;
                                    b.Padding = new Padding(0);
                                    b.Font = new Font("Noto Kufi Arabic", 8F);

                                    var t = b.Tag as string;
                                    if (t != null)
                                        b.Text = t.Split(' ')[0];
                                }
                                else
                                {
                                    var t = b.Tag as string;
                                    if (t != null)
                                        b.Text = t;

                                    b.TextAlign = ContentAlignment.MiddleLeft;
                                    b.Padding = new Padding(15, 0, 0, 0);
                                    b.Font = new Font("Noto Kufi Arabic", 9F);
                                }
                            }
                        }
                    }
                }
            };
        }
        private Panel AddMenuSection(string title)
        {
            Panel section = new Panel();
            section.Dock = DockStyle.Top;
            section.AutoSize = true;
            section.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            section.BackColor = Color.FromArgb(30, 41, 59);

            // Header Button
            Button header = new Button();
            header.Text = "▸ " + title;
            header.Dock = DockStyle.Top;
            header.Height = 45;
            header.FlatStyle = FlatStyle.Flat;
            header.FlatAppearance.BorderSize = 0;
            header.ForeColor = Color.White;
            header.BackColor = Color.FromArgb(22, 32, 50);
            header.TextAlign = ContentAlignment.MiddleLeft;
            header.Padding = new Padding(15, 0, 0, 0);

            Panel itemsPanel = new Panel();
            itemsPanel.Dock = DockStyle.Top;
            itemsPanel.AutoSize = true;
            itemsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            itemsPanel.Visible = false;

            header.Click += (s, e) =>
            {
                itemsPanel.Visible = !itemsPanel.Visible;
                header.Text = (itemsPanel.Visible ? "▾ " : "▸ ") + title;
            };

            section.Controls.Add(itemsPanel);
            section.Controls.Add(header);

            panelMenu.Controls.Add(section);
            panelMenu.Controls.SetChildIndex(section, 0);

            return itemsPanel;
        }


        private void frm_Main_Load(object sender, EventArgs e)
        {
            //BuildLayout();
            CheckChequesReminder();
            // 🔥 افتح Dashboard مباشرة
            OpenForm(new frm_Dashboard());
            CheckChequeAlerts();
        }

        private void CheckChequeAlerts()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
SELECT 
    ChequeNumber,
    DueDate,
    Amount
FROM SupplierCheques
WHERE ISNULL(IsCleared,0)=0
AND DueDate <= DATEADD(DAY,3,GETDATE())";

                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader rd = cmd.ExecuteReader();

                string msg = "";

                while (rd.Read())
                {
                    DateTime due = Convert.ToDateTime(rd["DueDate"]);
                    string number = rd["ChequeNumber"].ToString();
                    decimal amount = Convert.ToDecimal(rd["Amount"]);

                    if (due < DateTime.Now)
                    {
                        msg += $"🔴 شيك متأخر: {number} - {amount}\n";
                    }
                    else
                    {
                        msg += $"🟡 شيك قريب: {number} - {amount}\n";
                    }
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "تنبيهات الشيكات");
                }
            }
        }

    }


}

