using Accounting.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting.Core.Forms
{
    public partial class frm_CreditNote : Form
    {
        private string connectionString =
    @"Data Source=.\SQLEXPRESS;Initial Catalog=AccountingCoreDB;Integrated Security=True";

        private int originalInvoiceId;
        private int? creditNoteId; // إذا كان موجوداً للتعديل
        private int originalCustomerId;

        private string SellerName { get; set; }
        private string SellerTaxNo { get; set; }

        public frm_CreditNote(int originalInvoiceId, int? creditNoteId = null)
        {
            InitializeComponent();
            gridView1.CellValueChanged += gridView1_CellValueChanged;
            this.originalInvoiceId = originalInvoiceId;
            this.creditNoteId = creditNoteId;

            LoadOriginalInvoice(this.originalInvoiceId);

            if (creditNoteId.HasValue)
                LoadCreditNote(creditNoteId.Value);
        }

        private void LoadOriginalInvoice(int originalInvoiceId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // ========== 1. تحميل بيانات رأس الفاتورة الأصلية ==========
                SqlCommand cmdHeader = new SqlCommand(@"
            SELECT 
                i.InvoiceNumber,
                i.InvoiceDate,
                i.CustomerId,
                c.Name AS CustomerName
            FROM Invoices i
            LEFT JOIN Customers c ON i.CustomerId = c.CustomerId
            WHERE i.InvoiceId = @Id", con);
                cmdHeader.Parameters.AddWithValue("@Id", originalInvoiceId);

                using (SqlDataReader dr = cmdHeader.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtOriginalInvoiceNumber.Text = dr["InvoiceNumber"].ToString();
                        dateOriginal.Value = Convert.ToDateTime(dr["InvoiceDate"]);
                        txtCustomerName.Text = dr["CustomerName"].ToString();
                        originalCustomerId = Convert.ToInt32(dr["CustomerId"]);
                    }
                    else
                    {
                        MessageBox.Show("الفاتورة الأصلية غير موجودة.");
                        return;
                    }
                }

                // ========== 2. تحميل بنود الفاتورة الأصلية (فقط إذا كان مرتجع جديد) ==========
                if (!creditNoteId.HasValue)  // مرتجع جديد
                {
                    SqlCommand cmdLines = new SqlCommand(@"
                SELECT 
                    L.ProductId,
                    P.Name AS ProductName,
                    L.Quantity AS OriginalQuantity,
                    L.UnitPrice,
                    L.Discount,
                    L.TaxRate,
                    L.TotalBeforeTax,
                    L.TotalTax,
                    L.TotalAfterTax
                FROM InvoiceLines L
                INNER JOIN Products P ON L.ProductId = P.ProductId
                WHERE L.InvoiceId = @OriginalId
                ORDER BY L.LineId", con);
                    cmdLines.Parameters.AddWithValue("@OriginalId", originalInvoiceId);

                    DataTable originalLines = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmdLines))
                    {
                        da.Fill(originalLines);
                    }

                    // إنشاء جدول البنود الخاص بالمرتجع
                    DataTable dtCreditLines = CreateLinesTable();

                    if (originalLines.Rows.Count == 0)
                    {
                        MessageBox.Show("الفاتورة الأصلية لا تحتوي على بنود. لا يمكن إنشاء مرتجع.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // نترك الجدول فارغاً
                    }
                    else
                    {
                        foreach (DataRow origRow in originalLines.Rows)
                        {
                            DataRow newRow = dtCreditLines.NewRow();
                            newRow["ProductId"] = origRow["ProductId"];
                            newRow["ProductName"] = origRow["ProductName"];
                            newRow["Quantity"] = 0; // الكمية المرتجعة تبدأ بـ 0
                            newRow["UnitPrice"] = origRow["UnitPrice"];
                            newRow["Discount"] = origRow["Discount"];
                            newRow["TaxRate"] = origRow["TaxRate"];
                            newRow["TotalBeforeTax"] = 0;
                            newRow["TotalTax"] = 0;
                            newRow["TotalAfterTax"] = 0;
                            dtCreditLines.Rows.Add(newRow);
                        }
                    }

                    // ربط البيانات بالشبكة
                    gridControl1.DataSource = dtCreditLines;

                    // إعدادات الأعمدة
                    gridView1.PopulateColumns();
                    gridView1.Columns["ProductId"].Visible = false; // إخفاء رقم المنتج
                    gridView1.Columns["ProductName"].Caption = "الصنف";
                    gridView1.Columns["Quantity"].Caption = "الكمية المرتجعة";
                    gridView1.Columns["UnitPrice"].Caption = "سعر الوحدة";
                    gridView1.Columns["Discount"].Caption = "الخصم";
                    gridView1.Columns["TaxRate"].Caption = "نسبة الضريبة %";
                    gridView1.Columns["TotalBeforeTax"].Caption = "قبل الضريبة";
                    gridView1.Columns["TotalTax"].Caption = "الضريبة";
                    gridView1.Columns["TotalAfterTax"].Caption = "الإجمالي";

                    // جعل الأعمدة قابلة للتحرير
                    gridView1.Columns["Quantity"].OptionsColumn.AllowEdit = true;
                    gridView1.Columns["Quantity"].OptionsColumn.ReadOnly = false;

                    // إضافة معالج الحدث لتغيير القيم (إذا لم يكن مضافاً)
                    gridView1.CellValueChanged -= gridView1_CellValueChanged; // إزالة أي تكرار
                    gridView1.CellValueChanged += gridView1_CellValueChanged;

                    // حساب المجاميع الأولية (ستكون صفر)
                    CalculateTotals();
                }
            }
        }

        // دالة مساعدة لإنشاء هيكل جدول البنود (إذا لم تكن موجودة بالفعل)
        private DataTable CreateLinesTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LineId", typeof(int));
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("TaxRate", typeof(decimal));
            dt.Columns.Add("TotalBeforeTax", typeof(decimal));
            dt.Columns.Add("TotalTax", typeof(decimal));
            dt.Columns.Add("TotalAfterTax", typeof(decimal));
            return dt;
        }



        private void LoadCreditNote(int creditNoteId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // جلب رأس المرتجع
                SqlCommand cmdHeader = new SqlCommand(@"
            SELECT 
                cn.CreditNoteNumber,
                cn.CreditNoteDate,
                cn.CustomerId,
                cn.OriginalInvoiceId,
                cn.TotalBeforeTax,
                cn.TotalTax,
                cn.TotalAfterTax,
                i.InvoiceNumber AS OriginalInvoiceNumber,
                i.InvoiceDate AS OriginalInvoiceDate,
                c.Name AS CustomerName
            FROM CreditNotes cn
            INNER JOIN Invoices i ON cn.OriginalInvoiceId = i.InvoiceId
            INNER JOIN Customers c ON cn.CustomerId = c.CustomerId
            WHERE cn.CreditNoteId = @Id", con);
                cmdHeader.Parameters.AddWithValue("@Id", creditNoteId);

                using (SqlDataReader dr = cmdHeader.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtOriginalInvoiceNumber.Text = dr["CreditNoteNumber"].ToString();
                        dateOriginal.Value = Convert.ToDateTime(dr["CreditNoteDate"]);
                        txtOriginalInvoiceNumber.Text = dr["OriginalInvoiceNumber"].ToString();
                        dateOriginal.Value = Convert.ToDateTime(dr["OriginalInvoiceDate"]);
                        txtCustomerName.Text = dr["CustomerName"].ToString();
                        txtSubTotal.Text = Convert.ToDecimal(dr["TotalBeforeTax"]).ToString("0.000");
                        txtTax.Text = Convert.ToDecimal(dr["TotalTax"]).ToString("0.000");
                        txtTotal.Text = Convert.ToDecimal(dr["TotalAfterTax"]).ToString("0.000");

                        // تخزين القيم الهامة
                        originalInvoiceId = Convert.ToInt32(dr["OriginalInvoiceId"]);
                        originalCustomerId = Convert.ToInt32(dr["CustomerId"]);
                    }
                }

                // جلب بنود المرتجع
                SqlCommand cmdLines = new SqlCommand(@"
            SELECT 
                cl.LineId,
                cl.ProductId,
                p.Name AS ProductName,
                cl.Quantity,
                cl.UnitPrice,
                cl.Discount,
                cl.TaxRate,
                cl.TotalBeforeTax,
                cl.TotalTax,
                cl.TotalAfterTax
            FROM CreditNoteLines cl
            INNER JOIN Products p ON cl.ProductId = p.ProductId
            WHERE cl.CreditNoteId = @Id
            ORDER BY cl.LineId", con);
                cmdLines.Parameters.AddWithValue("@Id", creditNoteId);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmdLines))
                {
                    da.Fill(dt);
                }

                // ربط البيانات بالـ grid
                gridControl1.DataSource = null;
                gridControl1.DataSource = dt;

                // إعدادات الأعمدة (اختياري)
                gridView1.PopulateColumns();
                // يمكن إخفاء بعض الأعمدة مثل LineId, ProductId إذا أردت
                gridView1.Columns["LineId"].Visible = false;
                gridView1.Columns["ProductId"].Visible = false;
                // تسمية الأعمدة
                gridView1.Columns["ProductName"].Caption = "الصنف";
                gridView1.Columns["Quantity"].Caption = "الكمية";
                gridView1.Columns["UnitPrice"].Caption = "سعر الوحدة";
                gridView1.Columns["Discount"].Caption = "الخصم";
                gridView1.Columns["TaxRate"].Caption = "نسبة الضريبة";
                gridView1.Columns["TotalBeforeTax"].Caption = "المجموع قبل الضريبة";
                gridView1.Columns["TotalTax"].Caption = "الضريبة";
                gridView1.Columns["TotalAfterTax"].Caption = "الإجمالي";
            }
        }

        private async void btnSend_ClickAsync(object sender, EventArgs e)
        {
            if (!creditNoteId.HasValue)
            {
                MessageBox.Show("يجب حفظ المرتجع أولاً قبل الإرسال.");
                return;
            }

            CreditNoteProcessor processor = new CreditNoteProcessor(connectionString);
            processor.SellerName = "مؤسسة يزن للتجهيزات العلمية";
            processor.SellerTaxNo = "1160788";

            var result = await processor.SendCreditNoteToTaxAsync(creditNoteId.Value);

            if (result != null && !string.IsNullOrEmpty(result.Uuid))
            {
                MessageBox.Show($"تم إرسال المرتجع بنجاح.\nUUID: {result.Uuid}");
            }
            else
            {
                MessageBox.Show("فشل الإرسال: " + result?.Message);
            }
        }

        private async void btnSave_ClickAsync(object sender, EventArgs e)
        {
         
                try
                {
                    // التحقق من وجود بنود
                    DataTable dtLines = gridControl1.DataSource as DataTable;
                    if (dtLines == null || dtLines.Rows.Count == 0)
                    {
                        MessageBox.Show("يجب إضافة بند واحد على الأقل.");
                        return;
                    }

                    // التحقق من أن الكميات أكبر من صفر
                    bool hasValidQuantity = false;
                    foreach (DataRow row in dtLines.Rows)
                    {
                        decimal qty = Convert.ToDecimal(row["Quantity"]);
                        if (qty > 0)
                        {
                            hasValidQuantity = true;
                            break;
                        }
                    }

                    if (!hasValidQuantity)
                    {
                        MessageBox.Show("يجب أن تحتوي البنود على كميات أكبر من صفر.");
                        return;
                    }

                    // التحقق من وجود سبب الإرجاع (إذا كان لديك حقل له)
                    if (string.IsNullOrWhiteSpace(txtReasonNote.Text))
                    {
                        MessageBox.Show("الرجاء إدخال سبب الإرجاع.");
                        return;
                    }

                    // حساب المجاميع
                    decimal totalBeforeTax = 0, totalTax = 0, totalAfterTax = 0;
                    foreach (DataRow row in dtLines.Rows)
                    {
                        totalBeforeTax += Convert.ToDecimal(row["TotalBeforeTax"]);
                        totalTax += Convert.ToDecimal(row["TotalTax"]);
                        totalAfterTax += Convert.ToDecimal(row["TotalAfterTax"]);
                    }

                    int savedCreditNoteId = 0;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlTransaction transaction = con.BeginTransaction();

                        try
                        {
                            if (creditNoteId.HasValue) // تحديث
                            {
                                // تحديث رأس المرتجع
                                SqlCommand cmdUpdate = new SqlCommand(@"
                        UPDATE CreditNotes
                        SET CreditNoteDate = @Date,
                            TotalBeforeTax = @TotalBeforeTax,
                            TotalTax = @TotalTax,
                            TotalAfterTax = @TotalAfterTax
                        WHERE CreditNoteId = @Id", con, transaction);
                                cmdUpdate.Parameters.AddWithValue("@Date", dateOriginal.Value.Date);
                                cmdUpdate.Parameters.AddWithValue("@TotalBeforeTax", totalBeforeTax);
                                cmdUpdate.Parameters.AddWithValue("@TotalTax", totalTax);
                                cmdUpdate.Parameters.AddWithValue("@TotalAfterTax", totalAfterTax);
                                cmdUpdate.Parameters.AddWithValue("@Id", creditNoteId.Value);
                                cmdUpdate.ExecuteNonQuery();

                                // حذف البنود القديمة
                                SqlCommand cmdDeleteLines = new SqlCommand("DELETE FROM CreditNoteLines WHERE CreditNoteId = @Id", con, transaction);
                                cmdDeleteLines.Parameters.AddWithValue("@Id", creditNoteId.Value);
                                cmdDeleteLines.ExecuteNonQuery();

                                savedCreditNoteId = creditNoteId.Value;
                            }
                            else // إضافة جديدة
                            {
                                string creditNoteNumber = txtCreditNoteNumber.Text.Trim();
                                if (string.IsNullOrEmpty(creditNoteNumber))
                                    creditNoteNumber = GenerateCreditNoteNumber();

                                SqlCommand cmdInsert = new SqlCommand(@"
                        INSERT INTO CreditNotes (
                            CreditNoteNumber, CreditNoteDate, CustomerId, OriginalInvoiceId,
                            TotalBeforeTax, TotalTax, TotalAfterTax, CreatedAt, PostedToTax
                        ) VALUES (
                            @Number, @Date, @CustomerId, @OriginalInvoiceId,
                            @TotalBeforeTax, @TotalTax, @TotalAfterTax, GETDATE(), 0
                        );
                        SELECT SCOPE_IDENTITY();", con, transaction);
                                cmdInsert.Parameters.AddWithValue("@Number", creditNoteNumber);
                                cmdInsert.Parameters.AddWithValue("@Date", dateOriginal.Value.Date);
                                cmdInsert.Parameters.AddWithValue("@CustomerId", originalCustomerId);
                                cmdInsert.Parameters.AddWithValue("@OriginalInvoiceId", originalInvoiceId);
                                cmdInsert.Parameters.AddWithValue("@TotalBeforeTax", totalBeforeTax);
                                cmdInsert.Parameters.AddWithValue("@TotalTax", totalTax);
                                cmdInsert.Parameters.AddWithValue("@TotalAfterTax", totalAfterTax);

                                savedCreditNoteId = Convert.ToInt32(cmdInsert.ExecuteScalar());
                            }

                            // إدراج بنود المرتجع (لكل صف)
                            foreach (DataRow row in dtLines.Rows)
                            {
                                int productId = Convert.ToInt32(row["ProductId"]);
                                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                                decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
                                decimal discount = row.Table.Columns.Contains("Discount") ? Convert.ToDecimal(row["Discount"]) : 0;
                                decimal taxRate = Convert.ToDecimal(row["TaxRate"]);
                                decimal totalBefore = Convert.ToDecimal(row["TotalBeforeTax"]);
                                decimal totalTaxLine = Convert.ToDecimal(row["TotalTax"]);
                                decimal totalAfter = Convert.ToDecimal(row["TotalAfterTax"]);

                                SqlCommand cmdLine = new SqlCommand(@"
                        INSERT INTO CreditNoteLines (
                            CreditNoteId, ProductId, Quantity, UnitPrice, Discount,
                            TaxRate, TotalBeforeTax, TotalTax, TotalAfterTax
                        ) VALUES (
                            @CreditNoteId, @ProductId, @Quantity, @UnitPrice, @Discount,
                            @TaxRate, @TotalBeforeTax, @TotalTax, @TotalAfterTax
                        )", con, transaction);
                                cmdLine.Parameters.AddWithValue("@CreditNoteId", savedCreditNoteId);
                                cmdLine.Parameters.AddWithValue("@ProductId", productId);
                                cmdLine.Parameters.AddWithValue("@Quantity", quantity);
                                cmdLine.Parameters.AddWithValue("@UnitPrice", unitPrice);
                                cmdLine.Parameters.AddWithValue("@Discount", discount);
                                cmdLine.Parameters.AddWithValue("@TaxRate", taxRate);
                                cmdLine.Parameters.AddWithValue("@TotalBeforeTax", totalBefore);
                                cmdLine.Parameters.AddWithValue("@TotalTax", totalTaxLine);
                                cmdLine.Parameters.AddWithValue("@TotalAfterTax", totalAfter);

                                cmdLine.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            creditNoteId = savedCreditNoteId; // تحديث المتغير العام

                            MessageBox.Show("تم حفظ المرتجع بنجاح.");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("خطأ في الحفظ: " + ex.Message);
                            return; // نخرج من الدالة إذا فشل الحفظ
                        }
                    } // هنا يتم إغلاق الاتصال والمعاملة تلقائياً

                    // بعد نجاح الحفظ وخارج using، نسأل المستخدم عن الإرسال
                    DialogResult result = MessageBox.Show("هل تريد إرسال المرتجع إلى نظام الفوترة الآن؟", "تأكيد", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        CreditNoteProcessor processor = new CreditNoteProcessor(connectionString);
                        processor.SellerName = "مؤسسة يزن للتجهيزات العلمية";
                        processor.SellerTaxNo = "1160788";
                        processor.ReasonNote = txtReasonNote.Text.Trim();

                        var response = await processor.SendCreditNoteToTaxAsync(creditNoteId.Value);
                        if (response != null && !string.IsNullOrEmpty(response.Uuid))
                        {
                            MessageBox.Show($"تم إرسال المرتجع بنجاح.\nرقم: {response.InvoiceNumber}\nUUID: {response.Uuid}");
                        }
                        else
                        {
                            MessageBox.Show("فشل الإرسال: " + response?.Message);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                AppEvents.RefreshDashboard(); // 🔥 سطر واحد فقط
            }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }

            
        }

        private string GenerateCreditNoteNumber()
        {
            string prefix = "CN";
            string datePart = DateTime.Now.ToString("yyyyMMdd");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(RIGHT(CreditNoteNumber,4) AS INT)),0)
FROM CreditNotes
WHERE CreditNoteNumber LIKE @Prefix + '%'", con);

                cmd.Parameters.AddWithValue("@Prefix", prefix + "-" + datePart + "-");

                int lastNumber = Convert.ToInt32(cmd.ExecuteScalar());

                int newNumber = lastNumber + 1;

                return $"{prefix}-{datePart}-{newNumber:D4}";
            }
        }



        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            // إذا تم تغيير الكمية أو سعر الوحدة أو الخصم أو نسبة الضريبة
            if (e.Column.FieldName == "Quantity" || e.Column.FieldName == "UnitPrice" ||
                e.Column.FieldName == "Discount" || e.Column.FieldName == "TaxRate")
            {
                DataRow row = gridView1.GetDataRow(e.RowHandle);
                if (row == null) return;

                decimal qty = Convert.ToDecimal(row["Quantity"]);
                decimal price = Convert.ToDecimal(row["UnitPrice"]);
                decimal discount = row["Discount"] == DBNull.Value
      ? 0
      : Convert.ToDecimal(row["Discount"]);
                decimal taxRate = Convert.ToDecimal(row["TaxRate"]);

                decimal beforeTax = (qty * price) - discount;
                if (beforeTax < 0) beforeTax = 0;

                decimal tax = beforeTax * taxRate / 100;
                decimal afterTax = beforeTax + tax;

                row["TotalBeforeTax"] = beforeTax;
                row["TotalTax"] = tax;
                row["TotalAfterTax"] = afterTax;

                // إعادة حساب المجاميع الكلية
                CalculateTotals();
            }
        }
        private void CalculateTotals()
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null) return;

            decimal totalBefore = 0, totalTax = 0, totalAfter = 0;
            foreach (DataRow row in dt.Rows)
            {
                totalBefore += Convert.ToDecimal(row["TotalBeforeTax"]);
                totalTax += Convert.ToDecimal(row["TotalTax"]);
                totalAfter += Convert.ToDecimal(row["TotalAfterTax"]);
            }
            txtSubTotal.Text = totalBefore.ToString("0.000");
            txtTax.Text = totalTax.ToString("0.000");
            txtTotal.Text = totalAfter.ToString("0.000");
        }

        private void frm_CreditNote_Load(object sender, EventArgs e)
        {
            dateOriginal.Format = DateTimePickerFormat.Custom;
            dateOriginal.CustomFormat = "dd/MM/yyyy";
        }
    }
}
