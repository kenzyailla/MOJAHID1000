using Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;
namespace Accounting.Core.EInvoice
{
    public class UBLInvoiceGenerator
    {
        private readonly string _cs;

        public UBLInvoiceGenerator(string connectionString)
        {
            _cs = connectionString;
        }

        private static string F5(decimal value)
        {
            return value.ToString("0.00000", CultureInfo.InvariantCulture);
        }

        public string GenerateInvoiceXml(int invoiceId, string sellerName, string sellerTaxNo)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT 
    i.InvoiceId,
    i.InvoiceNumber,
    i.InvoiceDate,
    i.UUID,
    i.ICV,
    c.Name AS CustomerName
FROM Invoices i
LEFT JOIN Customers c ON i.CustomerId = c.CustomerId
WHERE i.InvoiceId = @Id", con);

                cmd.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    throw new Exception("Invoice not found");

                DataRow r = dt.Rows[0];

                string invoiceNumber = r["InvoiceNumber"].ToString();
                string uuid = r["UUID"].ToString();
                string icv = r["ICV"] == DBNull.Value ? "1" : r["ICV"].ToString();
                string customerName = r["CustomerName"] == DBNull.Value ? "" : r["CustomerName"].ToString();

                SqlCommand cmdLines = new SqlCommand(@"
SELECT
    l.LineId,
    ISNULL(p.Name,'Item') AS ProductName,
    ISNULL(l.Quantity,0) AS Quantity,
    ISNULL(l.UnitPrice,0) AS UnitPrice,
    ISNULL(l.TaxRate,0) AS TaxRate,
    ISNULL(l.Discount,0) AS Discount
FROM InvoiceLines l
LEFT JOIN Products p ON l.ProductId = p.ProductId
WHERE l.InvoiceId = @Id
ORDER BY l.LineId", con);

                cmdLines.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dtLines = new DataTable();
                dtLines.Load(cmdLines.ExecuteReader());

                if (dtLines.Rows.Count == 0)
                    throw new Exception("Invoice has no lines");

                decimal totalBeforeTax = 0m;
                decimal totalTax = 0m;

                foreach (DataRow line in dtLines.Rows)
                {
                    decimal qty = Convert.ToDecimal(line["Quantity"]);
                    decimal price = Convert.ToDecimal(line["UnitPrice"]);
                    decimal discount = Convert.ToDecimal(line["Discount"]);
                    decimal taxRate = Convert.ToDecimal(line["TaxRate"]);

                    decimal lineBefore = (qty * price) - discount;
                    decimal lineTax = lineBefore * taxRate / 100m;

                    totalBeforeTax += lineBefore;
                    totalTax += lineTax;
                }

                decimal totalAfterTax = totalBeforeTax + totalTax;

                XNamespace inv = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

                var root = new XElement(inv + "Invoice",
                    new XAttribute(XNamespace.Xmlns + "cbc", cbc),
                    new XAttribute(XNamespace.Xmlns + "cac", cac),

                    new XElement(cbc + "ProfileID", "reporting:1.0"),
                    new XElement(cbc + "ID", invoiceNumber),
                    new XElement(cbc + "UUID", uuid),
                    new XElement(cbc + "IssueDate", Convert.ToDateTime(r["InvoiceDate"]).ToString("yyyy-MM-dd")),
                    new XElement(cbc + "InvoiceTypeCode", new XAttribute("name", "012"), "388"),
                    new XElement(cbc + "DocumentCurrencyCode", "JOD"),
                    new XElement(cbc + "TaxCurrencyCode", "JOD"),

                    new XElement(cac + "AdditionalDocumentReference",
                        new XElement(cbc + "ID", "ICV"),
                        new XElement(cbc + "UUID", icv)
                    ),

                    new XElement(cac + "AccountingSupplierParty",
                        new XElement(cac + "Party",
                            new XElement(cac + "PostalAddress",
                                new XElement(cac + "Country",
                                    new XElement(cbc + "IdentificationCode", "JO")
                                )
                            ),
                            new XElement(cac + "PartyTaxScheme",
                                new XElement(cbc + "CompanyID", sellerTaxNo),
                                new XElement(cac + "TaxScheme",
                                    new XElement(cbc + "ID", "VAT")
                                )
                            ),
                            new XElement(cac + "PartyLegalEntity",
                                new XElement(cbc + "RegistrationName", sellerName)
                            )
                        )
                    ),

                    new XElement(cac + "AccountingCustomerParty",
                        new XElement(cac + "Party",
                            new XElement(cac + "PartyLegalEntity",
                                new XElement(cbc + "RegistrationName", customerName)
                            )
                        )
                    ),

                    // رقم النشاط الضريبي
                    new XElement(cac + "SellerSupplierParty",
                        new XElement(cac + "Party",
                            new XElement(cac + "PartyIdentification",
                                new XElement(cbc + "ID", "275570")
                            )
                        )
                    ),

                    new XElement(cac + "PaymentMeans",
                        new XElement(cbc + "PaymentMeansCode",
                            new XAttribute("listID", "UN/ECE 4461"),
                            "10"
                        )
                    ),

                    new XElement(cac + "TaxTotal",
                        new XElement(cbc + "TaxAmount",
                            new XAttribute("currencyID", "JOD"),
                            totalTax.ToString("0.00000")
                        )
                    ),

                    new XElement(cac + "LegalMonetaryTotal",
                        new XElement(cbc + "TaxExclusiveAmount",
                            new XAttribute("currencyID", "JOD"),
                            totalBeforeTax.ToString("0.00000")
                        ),
                        new XElement(cbc + "TaxInclusiveAmount",
                            new XAttribute("currencyID", "JOD"),
                            totalAfterTax.ToString("0.00000")
                        ),
                        new XElement(cbc + "PayableAmount",
                            new XAttribute("currencyID", "JOD"),
                            totalAfterTax.ToString("0.00000")
                        )
                    )
                );

                int lineNo = 1;

                foreach (DataRow line in dtLines.Rows)
                {
                    decimal qty = Convert.ToDecimal(line["Quantity"]);
                    decimal price = Convert.ToDecimal(line["UnitPrice"]);
                    decimal discount = Convert.ToDecimal(line["Discount"]);
                    decimal taxRate = Convert.ToDecimal(line["TaxRate"]);
                    string product = line["ProductName"].ToString();

                    decimal lineBefore = (qty * price) - discount;
                    decimal lineTax = lineBefore * taxRate / 100m;
                    decimal lineAfter = lineBefore + lineTax;

                    string taxCategory = taxRate == 0 ? "O" : "S";

                    var xLine = new XElement(cac + "InvoiceLine",
                        new XElement(cbc + "ID", lineNo),
                        new XElement(cbc + "InvoicedQuantity",
                            new XAttribute("unitCode", "PCE"),
                            qty.ToString("0.00000")
                        ),
                        new XElement(cbc + "LineExtensionAmount",
                            new XAttribute("currencyID", "JOD"),
                            lineBefore.ToString("0.00000")
                        ),

                        new XElement(cac + "TaxTotal",
                            new XElement(cbc + "TaxAmount",
                                new XAttribute("currencyID", "JOD"),
                                lineTax.ToString("0.00000")
                            ),
                            new XElement(cbc + "RoundingAmount",
                                new XAttribute("currencyID", "JOD"),
                                lineAfter.ToString("0.00000")
                            ),
                            new XElement(cac + "TaxSubtotal",
                                new XElement(cbc + "TaxAmount",
                                    new XAttribute("currencyID", "JOD"),
                                    lineTax.ToString("0.00000")
                                ),
                                new XElement(cac + "TaxCategory",
                                    new XElement(cbc + "ID",
                                        new XAttribute("schemeAgencyID", "6"),
                                        new XAttribute("schemeID", "UN/ECE 5305"),
                                        taxCategory
                                    ),
                                    new XElement(cbc + "Percent",
                                        taxRate.ToString("0.00", CultureInfo.InvariantCulture)
                                    ),
                                    new XElement(cac + "TaxScheme",
                                        new XElement(cbc + "ID",
                                            new XAttribute("schemeAgencyID", "6"),
                                            new XAttribute("schemeID", "UN/ECE 5153"),
                                            "VAT"
                                        )
                                    )
                                )
                            )
                        ),

                        new XElement(cac + "Item",
                            new XElement(cbc + "Name", product)
                        ),

                        new XElement(cac + "Price",
                            new XElement(cbc + "PriceAmount",
                                new XAttribute("currencyID", "JOD"),
                                price.ToString("0.00000")
                            )
                        )
                    );

                    root.Add(xLine);
                    lineNo++;
                }

                XDocument xml = new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    root
                );

                return xml.ToString();
            }
        }




        public string GenerateCreditNoteXml(int creditNoteId, string sellerName, string sellerTaxNo, string reasonNote = "مرتجع بضاعة")
        {
            if (string.IsNullOrWhiteSpace(reasonNote))
                reasonNote = "مرتجع بضاعة";
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                // جلب بيانات رأس الإشعار الدائن مع إجمالي الفاتورة الأصلية وبيانات المشتري كاملة
                SqlCommand cmd = new SqlCommand(@"
SELECT 
    cn.CreditNoteId,
    cn.CreditNoteNumber,
    cn.CreditNoteDate,
    cn.TotalBeforeTax,
    cn.TotalTax,
    cn.TotalAfterTax,
    cn.UUID,
    cn.ICV,
    c.Name AS CustomerName,
    c.TaxNumber AS CustomerTaxNumber,
    i.InvoiceNumber AS OriginalInvoiceNumber,
    i.UUID AS OriginalUUID,
    i.TotalAfterTax AS OriginalTotal,
    cus.PostalCode,
    cus.CountrySubentityCode
FROM CreditNotes cn
LEFT JOIN Customers c ON cn.CustomerId = c.CustomerId
LEFT JOIN Invoices i ON cn.OriginalInvoiceId = i.InvoiceId
LEFT JOIN (
    -- افتراض وجود جدول لعناوين العملاء، إن لم يكن موجوداً نستخدم قيماً افتراضية
    SELECT CustomerId, '13713' AS PostalCode, 'JO-AM' AS CountrySubentityCode
    FROM Customers
) cus ON cn.CustomerId = cus.CustomerId
WHERE cn.CreditNoteId = @Id", con);

                cmd.Parameters.AddWithValue("@Id", creditNoteId);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    throw new Exception("Credit note not found");

                DataRow r = dt.Rows[0];

                // جلب بنود الإشعار الدائن
                SqlCommand cmdLines = new SqlCommand(@"
SELECT
    l.LineId,
    ISNULL(p.Name,'Item') AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TotalBeforeTax,
    l.TotalTax,
    l.TotalAfterTax,
    l.TaxRate,
    ISNULL(l.Discount,0) AS Discount
FROM CreditNoteLines l
LEFT JOIN Products p ON l.ProductId = p.ProductId
WHERE l.CreditNoteId = @Id
ORDER BY l.LineId", con);

                cmdLines.Parameters.AddWithValue("@Id", creditNoteId);

                DataTable dtLines = new DataTable();
                dtLines.Load(cmdLines.ExecuteReader());

                if (dtLines.Rows.Count == 0)
                    throw new Exception("Credit note has no lines");

                //decimal totalBeforeTax = Convert.ToDecimal(r["TotalBeforeTax"]);
                //decimal totalTax = Convert.ToDecimal(r["TotalTax"]);
                //decimal totalAfterTax = Convert.ToDecimal(r["TotalAfterTax"]);
                decimal totalBeforeTax = 0;
                decimal totalTax = 0;

                foreach (DataRow line in dtLines.Rows)
                {
                    totalBeforeTax += Convert.ToDecimal(line["TotalBeforeTax"]);
                    totalTax += Convert.ToDecimal(line["TotalTax"]);
                }

                decimal totalAfterTax = totalBeforeTax + totalTax;

                decimal originalTotal = r["OriginalTotal"] != DBNull.Value ? Convert.ToDecimal(r["OriginalTotal"]) : 0;

                string creditNoteNumber = r["CreditNoteNumber"].ToString();
                string uuid = r["UUID"].ToString();
                string icv = (r["ICV"] == DBNull.Value ? "1" : r["ICV"].ToString());
                string customerName = string.IsNullOrWhiteSpace(r["CustomerName"].ToString()) ? "" : r["CustomerName"].ToString();
                string customerTaxNumber = r["CustomerTaxNumber"]?.ToString() ?? "";
                string originalInvoiceNumber = r["OriginalInvoiceNumber"].ToString();
                string originalUUID = r["OriginalUUID"].ToString();
                string postalCode = r["PostalCode"]?.ToString() ?? "13713";
                string countrySubentity = r["CountrySubentityCode"]?.ToString() ?? "JO-AM";

                XNamespace inv = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                XNamespace ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

                var root = new XElement(inv + "Invoice",
                    new XAttribute(XNamespace.Xmlns + "cbc", cbc),
                    new XAttribute(XNamespace.Xmlns + "cac", cac),
                    new XAttribute(XNamespace.Xmlns + "ext", ext),

                    new XElement(cbc + "ProfileID", "reporting:1.0"),
                    new XElement(cbc + "ID", creditNoteNumber),
                    new XElement(cbc + "UUID", uuid),
                    new XElement(cbc + "IssueDate", Convert.ToDateTime(r["CreditNoteDate"]).ToString("yyyy-MM-dd")),

                    // نوع المستند: 381 مع name="012" (نقدي) أو يمكن جعلها متغيرة حسب طريقة الدفع
                    new XElement(cbc + "InvoiceTypeCode",
                        new XAttribute("name", "012"),
                        "381"
                    ),

                    // نضع سبب الإرجاع في Note (غير فارغ)
                   

                    new XElement(cbc + "DocumentCurrencyCode", "JOD"),
                    new XElement(cbc + "TaxCurrencyCode", "JOD"),

// مرجع الفاتورة الأصلية
new XElement(cac + "BillingReference",
    new XElement(cac + "InvoiceDocumentReference",

        new XElement(cbc + "ID", originalInvoiceNumber),
        new XElement(cbc + "UUID", originalUUID),
        new XElement(cbc + "DocumentDescription", F5(originalTotal))

      

    )
),

                    // ICV
                    new XElement(cac + "AdditionalDocumentReference",
                        new XElement(cbc + "ID", "ICV"),
                        new XElement(cbc + "UUID", icv)
                    ),

                    // البائع
                    new XElement(cac + "AccountingSupplierParty",
                        new XElement(cac + "Party",
                            new XElement(cac + "PostalAddress",
                                new XElement(cac + "Country",
                                    new XElement(cbc + "IdentificationCode", "JO")
                                )
                            ),
                            new XElement(cac + "PartyTaxScheme",
                                new XElement(cbc + "CompanyID", sellerTaxNo),
                                new XElement(cac + "TaxScheme",
                                    new XElement(cbc + "ID", "VAT")
                                )
                            ),
                            new XElement(cac + "PartyLegalEntity",
                                new XElement(cbc + "RegistrationName", sellerName)
                            )
                        )
                    ),

                    // المشتري - بنفس تفاصيل الفاتورة العادية
                    new XElement(cac + "AccountingCustomerParty",
                        new XElement(cac + "Party",
                            // إضافة PartyIdentification إذا كان الرقم الضريبي موجوداً
                            !string.IsNullOrWhiteSpace(customerTaxNumber) && customerTaxNumber != "0" ?
                                new XElement(cac + "PartyIdentification",
                                    new XElement(cbc + "ID",
                                        new XAttribute("schemeID", "TN"),
                                        customerTaxNumber
                                    )
                                ) : null,
                            new XElement(cac + "PostalAddress",
                                new XElement(cbc + "PostalZone", postalCode),
                                new XElement(cbc + "CountrySubentityCode", countrySubentity),
                                new XElement(cac + "Country",
                                    new XElement(cbc + "IdentificationCode", "JO")
                                )
                            ),
                            new XElement(cac + "PartyTaxScheme",
                                // إذا كان الرقم الضريبي موجوداً نضعه، وإلا نكتفي بالعنصر الفارغ
                                !string.IsNullOrWhiteSpace(customerTaxNumber) && customerTaxNumber != "0" ?
                                    new XElement(cbc + "CompanyID", customerTaxNumber) : null,
                                new XElement(cac + "TaxScheme",
                                    new XElement(cbc + "ID", "VAT")
                                )
                            ),
                            new XElement(cac + "PartyLegalEntity",
                                string.IsNullOrWhiteSpace(customerName)
                                    ? null
                                    : new XElement(cbc + "RegistrationName", customerName)
                            )
                        ),
                        // إضافة AccountingContact فارغ (اختياري)
                        new XElement(cac + "AccountingContact",
                            new XElement(cbc + "Telephone", "")
                        )
                    ),

                    // تسلسل مصدر الدخل
                    new XElement(cac + "SellerSupplierParty",
                        new XElement(cac + "Party",
                            new XElement(cac + "PartyIdentification",
                                new XElement(cbc + "ID", "275570")
                            )
                        )
                    ),

                    // طريقة الدفع
                    new XElement(cac + "PaymentMeans",
                        new XElement(cbc + "PaymentMeansCode",
                            new XAttribute("listID", "UN/ECE 4461"),
                            "10"
                        ),
                         new XElement(cbc + "InstructionNote", reasonNote)
                    ),

                    // خصم إجمالي الفاتورة
                    new XElement(cac + "AllowanceCharge",
                        new XElement(cbc + "ChargeIndicator", "false"),
                        new XElement(cbc + "AllowanceChargeReason", "discount"),
                        new XElement(cbc + "Amount",
                            new XAttribute("currencyID", "JO"),
                            "0.00000"
                        )
                    ),

                    // ضريبة الإشعار الدائن
                    new XElement(cac + "TaxTotal",
                        new XElement(cbc + "TaxAmount",
                            new XAttribute("currencyID", "JO"),
                            F5(totalTax)
                        )
                    ),

                    // المجاميع
                    new XElement(cac + "LegalMonetaryTotal",
                        new XElement(cbc + "TaxExclusiveAmount",
                            new XAttribute("currencyID", "JO"),
                            F5(totalBeforeTax)
                        ),
                        new XElement(cbc + "TaxInclusiveAmount",
                            new XAttribute("currencyID", "JO"),
                            F5(totalAfterTax)
                        ),
                        new XElement(cbc + "AllowanceTotalAmount",
                            new XAttribute("currencyID", "JO"),
                            "0.00000"
                        ),
                        new XElement(cbc + "PayableAmount",
                            new XAttribute("currencyID", "JO"),
                            F5(totalAfterTax)
                        )
                    )
                );

                int lineNo = 1;
                foreach (DataRow line in dtLines.Rows)
                {
                    decimal qty = Convert.ToDecimal(line["Quantity"]);
                    // التأكد من أن الكمية أكبر من صفر (يجب أن يكون قد تم التحقق منها في واجهة المستخدم)
                    if (qty <= 0)
                        throw new Exception($"الكمية في البند {lineNo} يجب أن تكون أكبر من صفر.");

                    decimal unitPrice = Convert.ToDecimal(line["UnitPrice"]);
                    decimal lineBeforeTax = Convert.ToDecimal(line["TotalBeforeTax"]);
                    decimal lineTax = Convert.ToDecimal(line["TotalTax"]);
                    //decimal taxRate = Convert.ToDecimal(line["TaxRate"]);
                    decimal taxRate = Math.Round(Convert.ToDecimal(line["TaxRate"]), 2);
                    decimal lineDiscount = Convert.ToDecimal(line["Discount"]);
                    string productName = line["ProductName"].ToString();

                    var xLine = new XElement(cac + "InvoiceLine",
                        new XElement(cbc + "ID", lineNo.ToString()),

                        new XElement(cbc + "InvoicedQuantity",
                            new XAttribute("unitCode", "PCE"),
                            F5(qty)
                        ),

                        new XElement(cbc + "LineExtensionAmount",
                            new XAttribute("currencyID", "JO"),
                            F5(lineBeforeTax)
                        ),

                        new XElement(cac + "TaxTotal",
                            new XElement(cbc + "TaxAmount",
                                new XAttribute("currencyID", "JO"),
                                F5(lineTax)
                            ),
                            new XElement(cbc + "RoundingAmount",
                                new XAttribute("currencyID", "JO"),
                                F5(lineBeforeTax + lineTax)
                            ),
                            new XElement(cac + "TaxSubtotal",
                                new XElement(cbc + "TaxAmount",
                                    new XAttribute("currencyID", "JO"),
                                    F5(lineTax)
                                ),
                                new XElement(cac + "TaxCategory",
                                    new XElement(cbc + "ID",
                                        new XAttribute("schemeAgencyID", "6"),
                                        new XAttribute("schemeID", "UN/ECE 5305"),
                                        taxRate == 0m ? "O" : "S"
                                    ),
                                    new XElement(cbc + "Percent",
                                        taxRate.ToString("0.00", CultureInfo.InvariantCulture)
                                    ),
                                    new XElement(cac + "TaxScheme",
                                        new XElement(cbc + "ID",
                                            new XAttribute("schemeAgencyID", "6"),
                                            new XAttribute("schemeID", "UN/ECE 5153"),
                                            "VAT"
                                        )
                                    )
                                )
                            )
                        ),

                        new XElement(cac + "Item",
                            new XElement(cbc + "Name", productName)
                        ),

                        new XElement(cac + "Price",
                            new XElement(cbc + "PriceAmount",
                                new XAttribute("currencyID", "JO"),
                                F5(unitPrice)
                            ),
                            new XElement(cac + "AllowanceCharge",
                                new XElement(cbc + "ChargeIndicator", "false"),
                                new XElement(cbc + "AllowanceChargeReason", "DISCOUNT"),
                                new XElement(cbc + "Amount",
                                    new XAttribute("currencyID", "JO"),
                                    F5(lineDiscount)
                                )
                            )
                        )
                    );

                    root.Add(xLine);
                    lineNo++;
                }

                XDocument xml = new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    root
                );

                return xml.ToString();
            }
        }
    }
}