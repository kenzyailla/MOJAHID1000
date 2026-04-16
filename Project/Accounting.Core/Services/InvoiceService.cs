using System;
using System.Data.SqlClient;
using Accounting.Core.Models;
using Accounting.Core.Enums;
using System.Collections.Generic;
using Accounting.Core.Repositories;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Accounting.Core.Services
{



    public class InvoiceService
    {
        private readonly string _connectionString;
        private readonly JournalService _journalService;
        private readonly InventoryService _inventoryService;

        private readonly InvoiceRepository _invoiceRepo;
        private readonly BuyInvoiceRepository _buyInvoiceRepo;

        public InvoiceService(string connectionString)
        {
           
            _connectionString = connectionString;

            _journalService = new JournalService(connectionString);
            _inventoryService = new InventoryService(connectionString);

            _invoiceRepo = new InvoiceRepository(connectionString);
            _buyInvoiceRepo = new BuyInvoiceRepository(connectionString);

        }
       
      

        public DataTable GetInvoices()
        {
            return _invoiceRepo.GetInvoices();
        }

        public void UpdateInvoice(int invoiceId, Invoice newInvoice)
        {
            if (newInvoice.InvoiceType == 0)
            {
                MessageBox.Show("خطأ: نوع الفاتورة = 0");
                return;
            }

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // ======================
                    // تجهيز الحركات الجديدة
                    // ======================

                    var adjustments = new List<StockAdjustment>();

                    foreach (var line in newInvoice.Lines)
                    {
                        decimal qty = line.Quantity;

                        if (newInvoice.InvoiceType == (int)InvoiceType.Sales ||
                            newInvoice.InvoiceType == (int)InvoiceType.PurchaseReturn)
                            qty = -qty;

                        adjustments.Add(new StockAdjustment
                        {
                            ProductId = line.ProductId,
                            Quantity = qty
                        });
                    }

                    // ======================
                    // 1️⃣ Reverse old effects
                    // ======================

                    _inventoryService.RemoveInvoiceStockTransactions(con, trans, invoiceId);

                    _journalService.DeleteJournalByReference(con, trans, "Invoice", invoiceId);

                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM InvoiceLines WHERE InvoiceId=@InvId", con, trans))
                    {
                        cmd.Parameters.AddWithValue("@InvId", invoiceId);
                        cmd.ExecuteNonQuery();
                    }

                    // ======================
                    // 2️⃣ Validate Stock
                    // ======================

                    if (newInvoice.InvoiceType == (int)InvoiceType.Sales)
                    {
                        _inventoryService.ValidateStockAvailability(con, trans, adjustments);
                    }

                    // ======================
                    // 3️⃣ Update Header
                    // ======================

                    string updateHeader = @"
                UPDATE Invoices
                SET InvoiceNumber=@Number,
                    InvoiceDate=@Date,
                    CustomerId=@Customer,
                    TotalBeforeTax=@Before,
                    TotalTax=@Tax,
                    TotalAfterTax=@After,
                    InvoiceType=@Type,
                    PaymentType=@Payment
                WHERE InvoiceId=@InvId
            ";

                    using (SqlCommand cmd = new SqlCommand(updateHeader, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@InvId", invoiceId);
                        cmd.Parameters.AddWithValue("@Number", newInvoice.InvoiceNumber);
                        cmd.Parameters.AddWithValue("@Date", newInvoice.InvoiceDate);
                        cmd.Parameters.AddWithValue("@Customer", newInvoice.CustomerId);
                        cmd.Parameters.AddWithValue("@Before", newInvoice.TotalBeforeTax);
                        cmd.Parameters.AddWithValue("@Tax", newInvoice.TotalTax);
                        cmd.Parameters.AddWithValue("@After", newInvoice.TotalAfterTax);
                        cmd.Parameters.AddWithValue("@Type", newInvoice.InvoiceType);
                        cmd.Parameters.AddWithValue("@Payment", newInvoice.PaymentType);

                        cmd.ExecuteNonQuery();
                    }

                    // ======================
                    // 4️⃣ Insert Lines
                    // ======================

                    foreach (var line in newInvoice.Lines)
                    {
                        if (line.LineTotal == 0)
                            line.LineTotal = line.TotalBeforeTax;
                      

                        string insertLine = @"
                    INSERT INTO InvoiceLines
(InvoiceId,ProductId,Quantity,UnitPrice,
 Discount,TaxRate,
 TotalBeforeTax,TotalTax,TotalAfterTax,LineTotal)
VALUES
(@InvId,@Prod,@Qty,@Price,
 @Disc,@TaxRate,
 @Before,@Tax,@After,@LineTotal)
                ";
                        if (line.LineTotal == 0)
                        {
                          
                        }

                      

                        using (SqlCommand cmd = new SqlCommand(insertLine, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@InvId", invoiceId);
                            cmd.Parameters.AddWithValue("@Prod", line.ProductId);
                            cmd.Parameters.AddWithValue("@Qty", line.Quantity);
                            cmd.Parameters.AddWithValue("@Price", line.UnitPrice);
                            cmd.Parameters.AddWithValue("@Disc", line.Discount);
                            cmd.Parameters.AddWithValue("@TaxRate", line.TaxRate);
                            cmd.Parameters.AddWithValue("@Before", line.TotalBeforeTax);
                            cmd.Parameters.AddWithValue("@Tax", line.TotalTax);
                            cmd.Parameters.AddWithValue("@After", line.TotalAfterTax);
                            cmd.Parameters.AddWithValue("@LineTotal", line.LineTotal);
                            cmd.ExecuteNonQuery();
                        }
                       

                    }

                    // ======================
                    // 5️⃣ Add New Stock
                    // ======================

                    //              int transactionType =
                    //newInvoice.InvoiceType == (int)InvoiceType.Sales ? 2 : 1;

                    int transactionType = MapInventoryTransactionType(newInvoice.InvoiceType);

                    _inventoryService.AddStockAdjustments(
                        con,
                        trans,
                        adjustments,
                        invoiceId,
                        transactionType
                    );

                    // ======================
                    // 6️⃣ Accounting Entry
                    // ======================

                    if (newInvoice.InvoiceType == (int)InvoiceType.Sales)
                    {
                        _journalService.CreateSalesEntry(
                            con, trans,
                            newInvoice.TotalBeforeTax,
                            newInvoice.TotalTax,
                            newInvoice.TotalAfterTax,
                            newInvoice.PaymentType,
                            invoiceId
                        );
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                
                    throw;
                }
            }
        }

        public DataRow GetInvoiceHeader(int invoiceId)
        {
            return _invoiceRepo.GetInvoiceHeader(invoiceId);
        }

        public DataTable GetInvoiceLines(int invoiceId)
        {
            return _invoiceRepo.GetInvoiceLines(invoiceId);
        }

      
            public async Task<int> AddInvoice(Invoice invoice)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // ================= Insert Header =================

                    string insertHeader = @"
            INSERT INTO Invoices
            (InvoiceNumber,InvoiceDate,CustomerId,
             TotalBeforeTax,TotalTax,TotalAfterTax,
             InvoiceType,PaymentType)
            VALUES
            (@Number,@Date,@Customer,
             @Before,@Tax,@After,
             @Type,@Payment);
             SELECT SCOPE_IDENTITY();
            ";

                    int invoiceId;

                    using (SqlCommand cmd = new SqlCommand(insertHeader, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Number", invoice.InvoiceNumber);
                        cmd.Parameters.AddWithValue("@Date", invoice.InvoiceDate);
                        cmd.Parameters.AddWithValue("@Customer", invoice.CustomerId);
                        cmd.Parameters.AddWithValue("@Before", invoice.TotalBeforeTax);
                        cmd.Parameters.AddWithValue("@Tax", invoice.TotalTax);
                        cmd.Parameters.AddWithValue("@After", invoice.TotalAfterTax);
                        cmd.Parameters.AddWithValue("@Type", invoice.InvoiceType);
                        cmd.Parameters.AddWithValue("@Payment", invoice.PaymentType);

                        invoiceId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // ================= Insert Lines =================

                    foreach (var line in invoice.Lines)
                    {
                        decimal beforeTax = (line.Quantity * line.UnitPrice) - line.Discount;
                        decimal tax = beforeTax * line.TaxRate / 100m;
                        decimal afterTax = beforeTax + tax;

                        string insertLine = @"
    INSERT INTO InvoiceLines
    (InvoiceId,ProductId,Quantity,UnitPrice,
     Discount,TaxRate,
     TotalBeforeTax,TotalTax,TotalAfterTax)
    VALUES
    (@InvId,@Prod,@Qty,@Price,
     @Disc,@TaxRate,
     @Before,@Tax,@After)
    ";

                        using (SqlCommand cmd = new SqlCommand(insertLine, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@InvId", invoiceId);
                            cmd.Parameters.AddWithValue("@Prod", line.ProductId);
                            cmd.Parameters.AddWithValue("@Qty", line.Quantity);
                            cmd.Parameters.AddWithValue("@Price", line.UnitPrice);
                            cmd.Parameters.AddWithValue("@Disc", line.Discount);
                            cmd.Parameters.AddWithValue("@TaxRate", line.TaxRate);

                            cmd.Parameters.AddWithValue("@Before", beforeTax);
                            cmd.Parameters.AddWithValue("@Tax", tax);
                            cmd.Parameters.AddWithValue("@After", afterTax);

                            cmd.ExecuteNonQuery();
                        }
                    }

                   
                    //الجديدة
                    var adjustments = new List<StockAdjustment>();
                    int transactionType = MapInventoryTransactionType(invoice.InvoiceType);
                    foreach (var line in invoice.Lines)
                    {
                        decimal qty = line.Quantity;
                      
                        if (invoice.InvoiceType == (int)InvoiceType.Sales ||
                            invoice.InvoiceType == (int)InvoiceType.PurchaseReturn)
                            qty = -qty;

                        adjustments.Add(new StockAdjustment
                        {
                            ProductId = line.ProductId,
                            Quantity = qty
                        });
                        MessageBox.Show("TransactionType = " + transactionType);
                    }

                    // ✅ هنا نستخدم الدالة
                    //int transactionType = MapInventoryTransactionType(invoice.InvoiceType);
                    //MessageBox.Show("TransactionType = " + transactionType);
                    _inventoryService.AddStockAdjustments(
                       con,
                       trans,
                       adjustments,
                       invoiceId,
                       transactionType
                    );
                    // ================= Journal =================

                    if (invoice.InvoiceType == (int)InvoiceType.Sales)
                    {
                        _journalService.CreateSalesEntry(
                            con, trans,
                            invoice.TotalBeforeTax,
                            invoice.TotalTax,
                            invoice.TotalAfterTax,
                            invoice.PaymentType,
                            invoiceId
                        );
                    }

                    trans.Commit();
                   

                    return invoiceId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void DeleteInvoice(int invoiceId)
        {
            _invoiceRepo.DeleteInvoice(invoiceId);
        }

        public DataTable GetSalesReportAdvanced(
      int? productId,
      int? customerId,
      string invoiceNumber,
      decimal? taxRate,
      DateTime fromDate,
      DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"

-- ===== المبيعات =====
SELECT 
    i.InvoiceId,
    i.InvoiceNumber,
    i.InvoiceDate,
    c.CustomerId,
    c.Name AS CustomerName,
    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TotalBeforeTax,
    l.TotalTax,
    l.TotalAfterTax
FROM Invoices i
INNER JOIN InvoiceLines l ON i.InvoiceId = l.InvoiceId
INNER JOIN Products p ON l.ProductId = p.ProductId
INNER JOIN Customers c ON i.CustomerId = c.CustomerId
WHERE
    (@ProductId IS NULL OR l.ProductId = @ProductId)
AND (@CustomerId IS NULL OR c.CustomerId = @CustomerId)
AND (@InvoiceNumber IS NULL OR i.InvoiceNumber = @InvoiceNumber)
AND i.InvoiceDate BETWEEN @FromDate AND @ToDate

UNION ALL

SELECT 
    r.SalesReturnId,
    CAST(r.SalesReturnId AS NVARCHAR(50)),
    r.ReturnDate,
    c.CustomerId,
    c.Name,
    rl.ProductId,
    p.Name,
    -rl.Quantity,
    rl.UnitPrice,
    -rl.LineBeforeTax,
    -rl.LineTax,
    -rl.LineAfterTax
FROM SalesReturns r
INNER JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
INNER JOIN Products p ON rl.ProductId = p.ProductId
INNER JOIN Customers c ON r.CustomerId = c.CustomerId
WHERE
    (@ProductId IS NULL OR rl.ProductId = @ProductId)
AND (@CustomerId IS NULL OR c.CustomerId = @CustomerId)
AND r.ReturnDate BETWEEN @FromDate AND @ToDate

ORDER BY 3 DESC
";
            
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@ProductId", (object)productId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CustomerId", (object)customerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceNumber",
                    string.IsNullOrWhiteSpace(invoiceNumber)
                        ? DBNull.Value
                        : (object)invoiceNumber);

                cmd.Parameters.AddWithValue("@TaxRate", (object)taxRate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }
        }
        public int CreateSalesReturn(
       int originalInvoiceId,
       Dictionary<int, decimal> returnQtyByProduct,
       string notes = "")
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    DataRow inv = _invoiceRepo.GetInvoiceHeader(originalInvoiceId);
                    if (inv == null)
                        throw new Exception("الفاتورة الأصلية غير موجودة");

                    int customerId = Convert.ToInt32(inv["CustomerId"]);
                    int paymentType = Convert.ToInt32(inv["PaymentType"]);

                    DataTable lines = _invoiceRepo.GetInvoiceLines(originalInvoiceId);

                    decimal totalBefore = 0m;
                    decimal totalTax = 0m;
                    decimal totalAfter = 0m;

                    int salesReturnId;

                    // 1️⃣ إدخال رأس المرتجع
                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO SalesReturns
(OriginalInvoiceId, ReturnDate, CustomerId,
 TotalBeforeTax, TotalTax, TotalAfterTax,
 Notes, CreatedAt)
VALUES
(@Orig, GETDATE(), @Cust,
 0,0,0,@N,GETDATE());
SELECT SCOPE_IDENTITY();", con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Orig", originalInvoiceId);
                        cmd.Parameters.AddWithValue("@Cust", customerId);
                        cmd.Parameters.AddWithValue("@N", notes ?? "");

                        salesReturnId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    var adjustments = new List<StockAdjustment>();

                    // 2️⃣ إدخال السطور
                    foreach (DataRow line in lines.Rows)
                    {
                        int productId = Convert.ToInt32(line["ProductId"]);

                        if (!returnQtyByProduct.ContainsKey(productId))
                            continue;

                        decimal qty = returnQtyByProduct[productId];
                        if (qty <= 0) continue;

                        decimal unitPrice = Convert.ToDecimal(line["UnitPrice"]);
                        decimal taxRate = Convert.ToDecimal(line["TaxRate"]);

                        decimal before = qty * unitPrice;
                        decimal tax = before * taxRate / 100m;
                        decimal after = before + tax;

                        totalBefore += before;
                        totalTax += tax;
                        totalAfter += after;

                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO SalesReturnLines
(SalesReturnId, ProductId, Quantity,
 UnitPrice, TaxRate,
 LineBeforeTax, LineTax, LineAfterTax)
VALUES
(@Rid,@Prod,@Qty,@Price,@Rate,@B,@T,@A);", con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Rid", salesReturnId);
                            cmd.Parameters.AddWithValue("@Prod", productId);
                            cmd.Parameters.AddWithValue("@Qty", qty);
                            cmd.Parameters.AddWithValue("@Price", unitPrice);
                            cmd.Parameters.AddWithValue("@Rate", taxRate);
                            cmd.Parameters.AddWithValue("@B", before);
                            cmd.Parameters.AddWithValue("@T", tax);
                            cmd.Parameters.AddWithValue("@A", after);
                            cmd.ExecuteNonQuery();
                        }

                        // ✅ المخزون يزيد
                        adjustments.Add(new StockAdjustment
                        {
                            ProductId = productId,
                            Quantity = qty   // موجب
                        });
                    }

                    // 3️⃣ تحديث الإجماليات
                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE SalesReturns
SET TotalBeforeTax=@B,
    TotalTax=@T,
    TotalAfterTax=@A
WHERE SalesReturnId=@Id;", con, trans))
                    {
                        cmd.Parameters.AddWithValue("@B", totalBefore);
                        cmd.Parameters.AddWithValue("@T", totalTax);
                        cmd.Parameters.AddWithValue("@A", totalAfter);
                        cmd.Parameters.AddWithValue("@Id", salesReturnId);
                        cmd.ExecuteNonQuery();
                    }

                    // 4️⃣ المخزون (بدون تكرار كود)
                    _inventoryService.AddStockAdjustments(
                        con,
                        trans,
                        adjustments,
                        salesReturnId,
                        3 // SalesReturn
                    );

                    // 5️⃣ القيد المحاسبي (ننقله إلى JournalService)
                    _journalService.CreateSalesReturnEntry(
                        con,
                        trans,
                        salesReturnId,
                        totalBefore,
                        totalTax,
                        totalAfter,
                        paymentType,
                        customerId
                    );

                    trans.Commit();
                    return salesReturnId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        private void CreateSalesReturnJournal(
      SqlConnection con,
      SqlTransaction trans,
      int salesReturnId,
      int originalInvoiceId,
      decimal totalBefore,
      decimal totalTax,
      decimal totalAfter,
      int paymentType,
      int customerId)
        {
            // حساباتك حسب جدول Accounts عندك
            int salesAccountId = GetAccountIdByCode(con, trans, "301"); // المبيعات (Revenue)
            int taxOutAccountId = GetAccountIdByCode(con, trans, "401"); // ضريبة مخرجات (Liability)

            int cashAccountId = GetAccountIdByCode(con, trans, "101"); // الصندوق
            int bankAccountId = GetAccountIdByCode(con, trans, "104"); // البنك
            int arAccountId = GetAccountIdByCode(con, trans, "102"); // ذمم عملاء

            // ✅ اختيار حساب الدائن تلقائيًا حسب نوع الدفع
            int creditAccountId;

            // عدّل الأرقام حسب نظامك إن كانت مختلفة
            // مثال شائع:
            // 1=Cash  2=Bank  3=OnAccount
            switch (paymentType)
            {
                case 1: // Cash
                    creditAccountId = cashAccountId;
                    break;

                case 2: // Bank
                    creditAccountId = bankAccountId;
                    break;

                case 3: // OnAccount
                    creditAccountId = arAccountId;
                    break;

                default:
                    // احتياط: اعتبرها ذمم عملاء
                    creditAccountId = arAccountId;
                    break;
            }

            int journalId;

            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalEntries
(EntryDate, ReferenceType, ReferenceId, Description)
VALUES
(GETDATE(), 'SalesReturn', @RefId, @Desc);
SELECT SCOPE_IDENTITY();", con, trans))
            {
                cmd.Parameters.AddWithValue("@RefId", salesReturnId);
                cmd.Parameters.AddWithValue("@Desc", "مرتجع مبيعات للفاتورة رقم " + originalInvoiceId);
                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // مدين: المبيعات (عكس الإيراد)
            InsertJournalLine(con, trans, journalId, salesAccountId, totalBefore, 0m);

            // مدين: ضريبة المخرجات (عكس الضريبة المستحقة)
            InsertJournalLine(con, trans, journalId, taxOutAccountId, totalTax, 0m);

            // دائن: الصندوق/البنك/ذمم العملاء حسب PaymentType
            InsertJournalLine(con, trans, journalId, creditAccountId, 0m, totalAfter);
        }

        internal object SearchByCustomerId(int customerId)
        {
            throw new NotImplementedException();
        }

        private void InsertJournalLine(
    SqlConnection con,
    SqlTransaction trans,
    int journalId,
    int accountId,
    decimal debit,
    decimal credit)
        {
            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalLines
(JournalId, AccountId, Debit, Credit)
VALUES
(@J, @A, @D, @C);", con, trans))
            {
                cmd.Parameters.AddWithValue("@J", journalId);
                cmd.Parameters.AddWithValue("@A", accountId);
                cmd.Parameters.AddWithValue("@D", debit);
                cmd.Parameters.AddWithValue("@C", credit);
                cmd.ExecuteNonQuery();
            }
        }
        private decimal GetAlreadyReturnedQty(SqlConnection con, SqlTransaction trans, int originalInvoiceId, int productId)
        {
            using (SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(rl.Quantity),0)
FROM SalesReturns r
JOIN SalesReturnLines rl ON r.SalesReturnId = rl.SalesReturnId
WHERE r.OriginalInvoiceId = @InvId
AND rl.ProductId = @Prod;", con, trans))
            {
                cmd.Parameters.AddWithValue("@InvId", originalInvoiceId);
                cmd.Parameters.AddWithValue("@Prod", productId);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
        private int GetCustomerAccountId(SqlConnection con, SqlTransaction trans, int customerId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Customers WHERE CustomerId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", customerId);

                object result = cmd.ExecuteScalar();
                if (result == null)
                    throw new Exception("العميل غير مرتبط بحساب محاسبي");

                return Convert.ToInt32(result);
            }
        }
        private void InsertInventoryTransaction(SqlConnection con, SqlTransaction trans,
            int productId, decimal qty, int transactionType, int referenceId, string notes)
        {
            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO InventoryTransactions
(ProductId, Quantity, TransactionType, ReferenceId, TransactionDate, Notes)
VALUES
(@P, @Q, @T, @R, GETDATE(), @N);", con, trans))
            {
                cmd.Parameters.AddWithValue("@P", productId);
                cmd.Parameters.AddWithValue("@Q", qty);
                cmd.Parameters.AddWithValue("@T", transactionType);
                cmd.Parameters.AddWithValue("@R", referenceId);
                cmd.Parameters.AddWithValue("@N", notes ?? "");
                cmd.ExecuteNonQuery();
            }
        }
        private int GetAccountIdByCode(SqlConnection con, SqlTransaction trans, string code)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Accounts WHERE AccountCode=@Code AND IsActive=1",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Code", code);

                object r = cmd.ExecuteScalar();
                if (r == null)
                    throw new Exception("الحساب غير موجود: " + code);

                return Convert.ToInt32(r);
            }
        }
        public int CreateBuyReturn(
          int originalBuyInvoiceId,
          Dictionary<int, decimal> returnQtyByProduct,
          string notes = "")
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 1️⃣ جلب رأس الفاتورة الأصلية

                    DataRow inv = _buyInvoiceRepo.GetBuyInvoiceHeader(originalBuyInvoiceId);

                    if (inv == null)
                        throw new Exception("فاتورة المشتريات الأصلية غير موجودة");

                    int supplierId = Convert.ToInt32(inv["SupplierId"]);

                    // 2️⃣ جلب السطور الأصلية

                    DataTable lines = _buyInvoiceRepo.GetBuyInvoiceLines(originalBuyInvoiceId);
                    decimal totalBefore = 0m;
                    decimal totalTax = 0m;
                    decimal totalAfter = 0m;

                    var returnLines = new List<BuyReturnLineTemp>();

                    foreach (DataRow line in lines.Rows)
                    {
                        int productId = Convert.ToInt32(line["ProductId"]);

                        decimal reqQty = returnQtyByProduct.ContainsKey(productId)
                            ? returnQtyByProduct[productId]
                            : 0m;

                        if (reqQty <= 0) continue;

                        decimal unitPrice = Convert.ToDecimal(line["UnitPrice"]);
                        decimal taxRate = Convert.ToDecimal(line["TaxRate"]);

                        decimal before = reqQty * unitPrice;
                        decimal tax = before * taxRate / 100m;
                        decimal after = before + tax;

                        totalBefore += before;
                        totalTax += tax;
                        totalAfter += after;

                        returnLines.Add(new BuyReturnLineTemp
                        {
                            ProductId = productId,
                            Quantity = reqQty,
                            UnitPrice = unitPrice,
                            TaxRate = taxRate,
                            BeforeTax = before,
                            Tax = tax,
                            AfterTax = after
                        });
                    }

                    if (totalAfter <= 0)
                        throw new Exception("لم يتم إدخال كميات مرتجعة");

                    // 3️⃣ إدخال رأس المرتجع
                    int buyReturnId;

                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO BuyReturns
(OriginalBuyInvoiceId, ReturnDate, SupplierId,
 TotalBeforeTax, TotalTax, TotalAfterTax, Notes, CreatedAt)
VALUES
(@Orig, GETDATE(), @Supp, @B, @T, @A, @N, GETDATE());
SELECT SCOPE_IDENTITY();", con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Orig", originalBuyInvoiceId);
                        cmd.Parameters.AddWithValue("@Supp", supplierId);
                        cmd.Parameters.AddWithValue("@B", totalBefore);
                        cmd.Parameters.AddWithValue("@T", totalTax);
                        cmd.Parameters.AddWithValue("@A", totalAfter);
                        cmd.Parameters.AddWithValue("@N", notes ?? "");

                        buyReturnId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 4️⃣ إدخال السطور + المخزون
                    foreach (var rl in returnLines)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO BuyReturnLines
(BuyReturnId, ProductId, Quantity, UnitPrice, TaxRate,
 LineBeforeTax, LineTax, LineAfterTax)
VALUES
(@Rid, @Prod, @Qty, @Price, @Rate, @B, @T, @A);", con, trans))
                        {
                            cmd.Parameters.AddWithValue("@Rid", buyReturnId);
                            cmd.Parameters.AddWithValue("@Prod", rl.ProductId);
                            cmd.Parameters.AddWithValue("@Qty", rl.Quantity);
                            cmd.Parameters.AddWithValue("@Price", rl.UnitPrice);
                            cmd.Parameters.AddWithValue("@Rate", rl.TaxRate);
                            cmd.Parameters.AddWithValue("@B", rl.BeforeTax);
                            cmd.Parameters.AddWithValue("@T", rl.Tax);
                            cmd.Parameters.AddWithValue("@A", rl.AfterTax);
                            cmd.ExecuteNonQuery();
                        }

                        // ❗ المخزون ينقص (سالب)
                        _inventoryService.AddStockAdjustments(
                            con,
                            trans,
                            new List<StockAdjustment>
                            {
                        new StockAdjustment
                        {
                            ProductId = rl.ProductId,
                            Quantity = -rl.Quantity
                        }
                            },
                            buyReturnId,
                            4 // TransactionType = BuyReturn
                        );
                    }

                    // 5️⃣ القيد المحاسبي
                    _journalService.CreateBuyReturnEntry(
                        con,
                        trans,
                        buyReturnId,
                        totalBefore,
                        totalTax,
                        totalAfter,
                        supplierId
                    );

                    trans.Commit();
                    return buyReturnId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        private void CreateBuyReturnJournal(
        SqlConnection con,
        SqlTransaction trans,
        int buyReturnId,
        decimal totalBefore,
        decimal totalTax,
        decimal totalAfter,
        int supplierId)
        {
            int purchaseAccountId = GetAccountIdByCode(con, trans, "501");
            int taxInAccountId = GetAccountIdByCode(con, trans, "402");

            // 🔥 هنا التعديل المهم
            int supplierAccountId = GetSupplierAccountId(con, trans, supplierId);

            int journalId;

            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO JournalEntries
(EntryDate, ReferenceType, ReferenceId, Description)
VALUES
(GETDATE(), 'BuyReturn', @RefId, 'مرتجع مشتريات');
SELECT SCOPE_IDENTITY();", con, trans))
            {
                cmd.Parameters.AddWithValue("@RefId", buyReturnId);
                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // دائن المشتريات
            InsertJournalLine(con, trans, journalId, purchaseAccountId, 0m, totalBefore);

            // دائن ضريبة المدخلات
            InsertJournalLine(con, trans, journalId, taxInAccountId, 0m, totalTax);

            // 🔥 مدين المورد الصحيح
            InsertJournalLine(con, trans, journalId, supplierAccountId, totalAfter, 0m);
        }
        public DataTable GetSalesReturns()
        {
            return _invoiceRepo.GetSalesReturns();
        }
        public bool IsInvoiceFullyReturned(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT COUNT(*) 
FROM
(
    SELECT 
        l.ProductId,
        l.Quantity - ISNULL(SUM(rl.Quantity),0) AS RemainingQty
    FROM InvoiceLines l
    LEFT JOIN SalesReturns r ON r.OriginalInvoiceId = l.InvoiceId
    LEFT JOIN SalesReturnLines rl 
           ON rl.SalesReturnId = r.SalesReturnId 
           AND rl.ProductId = l.ProductId
    WHERE l.InvoiceId = @InvoiceId
    GROUP BY l.ProductId, l.Quantity
) X
WHERE RemainingQty > 0
";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count == 0;
            }
        }
        private int GetSupplierAccountId(SqlConnection con, SqlTransaction trans, int supplierId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AccountId FROM Suppliers WHERE SupplierId=@Id",
                con, trans))
            {
                cmd.Parameters.AddWithValue("@Id", supplierId);

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception("المورد غير مرتبط بحساب محاسبي");

                return Convert.ToInt32(result);
            }
        }
        private int MapInventoryTransactionType(int invoiceType)
        {
            if (invoiceType == (int)InvoiceType.Purchase)
                return 1; // شراء

            if (invoiceType == (int)InvoiceType.Sales)
                return 2; // بيع

            if (invoiceType == (int)InvoiceType.SalesReturn)
                return 3; // مرتجع مبيعات

            if (invoiceType == (int)InvoiceType.PurchaseReturn)
                return 4; // مرتجع شراء

            if (invoiceType == (int)InvoiceType.ExemptSales)
                return 2; // مبيعات معفاة تعامل كبيع عادي في المخزون

            throw new Exception($"نوع الفاتورة غير مدعوم في المخزون: {invoiceType}");
        }
    }
}
