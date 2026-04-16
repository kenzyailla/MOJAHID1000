using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml.Linq;

namespace Accounting.Core.Helpers
{
    public class UBLCreditNoteGenerator
    {
        private readonly string _cs;
        private readonly CultureInfo _ci = CultureInfo.InvariantCulture;

        public UBLCreditNoteGenerator(string connectionString)
        {
            _cs = connectionString;
        }

        public string GenerateCreditNoteXml(int salesReturnId,
            string sellerName,
            string sellerTaxNo,
            string currency = "JOD",
            string creditNoteTypeCode = "381",
            string creditNoteTypeName = "اشعار دائن")
        {
            DataTable dtHead = new DataTable();
            DataTable dtLines = new DataTable();
            DataTable dtOrig = new DataTable();

            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                string sqlHead = @"
SELECT 
    r.SalesReturnId, r.OriginalInvoiceId, r.ReturnDate, r.CustomerId,
    r.TotalBeforeTax, r.TotalTax, r.TotalAfterTax,
    c.Name AS CustomerName, ISNULL(c.TaxNumber,'') AS CustomerTaxNumber
FROM SalesReturns r
JOIN Customers c ON c.CustomerId = r.CustomerId
WHERE r.SalesReturnId = @Id;
";
                var da = new SqlDataAdapter(sqlHead, con);
                da.SelectCommand.Parameters.AddWithValue("@Id", salesReturnId);
                da.Fill(dtHead);

                if (dtHead.Rows.Count == 0)
                    throw new Exception("لم يتم العثور على المرتجع.");

                int originalInvoiceId = Convert.ToInt32(dtHead.Rows[0]["OriginalInvoiceId"]);

                string sqlOrig = @"
SELECT InvoiceId, InvoiceNumber, ISNULL(UUID,'') AS UUID
FROM Invoices
WHERE InvoiceId = @InvId;
";
                var daOrig = new SqlDataAdapter(sqlOrig, con);
                daOrig.SelectCommand.Parameters.AddWithValue("@InvId", originalInvoiceId);
                daOrig.Fill(dtOrig);

                if (dtOrig.Rows.Count == 0)
                    throw new Exception("لم يتم العثور على الفاتورة الأصلية المرتبطة بالمرتجع.");

                string sqlLines = @"
SELECT
    l.LineId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.TaxRate,
    l.LineBeforeTax,
    l.LineTax,
    l.LineAfterTax
FROM SalesReturnLines l
JOIN Products p ON p.ProductId = l.ProductId
WHERE l.SalesReturnId = @Id
ORDER BY l.LineId;
";
                var da2 = new SqlDataAdapter(sqlLines, con);
                da2.SelectCommand.Parameters.AddWithValue("@Id", salesReturnId);
                da2.Fill(dtLines);
            }

            DataRow h = dtHead.Rows[0];
            DataRow o = dtOrig.Rows[0];

            DateTime issueDate = Convert.ToDateTime(h["ReturnDate"]);
            string creditNoteId = "CRN" + h["SalesReturnId"].ToString();

            decimal totalBeforeTax = Convert.ToDecimal(h["TotalBeforeTax"]);
            decimal totalTax = Convert.ToDecimal(h["TotalTax"]);
            decimal totalAfterTax = Convert.ToDecimal(h["TotalAfterTax"]);

            XNamespace ns = "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2";
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            XNamespace ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

            var xCN = new XElement(ns + "CreditNote",
                new XAttribute(XNamespace.Xmlns + "cbc", cbc),
                new XAttribute(XNamespace.Xmlns + "cac", cac),
                new XAttribute(XNamespace.Xmlns + "ext", ext),

                new XElement(cbc + "ProfileID", "reporting:1.0"),
                new XElement(cbc + "ID", creditNoteId),
                new XElement(cbc + "UUID", Guid.NewGuid().ToString()),
                new XElement(cbc + "IssueDate", issueDate.ToString("yyyy-MM-dd")),

                new XElement(cbc + "CreditNoteTypeCode", new XAttribute("name", creditNoteTypeName), creditNoteTypeCode),
                new XElement(cbc + "DocumentCurrencyCode", currency),
                new XElement(cbc + "TaxCurrencyCode", currency),

                // ربط الفاتورة الأصلية
                new XElement(cac + "BillingReference",
                    new XElement(cac + "InvoiceDocumentReference",
                        new XElement(cbc + "ID", o["InvoiceNumber"].ToString()),
                        new XElement(cbc + "UUID", o["UUID"].ToString())
                    )
                ),

                // Supplier
                new XElement(cac + "AccountingSupplierParty",
                    new XElement(cac + "Party",
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

                // Customer
                new XElement(cac + "AccountingCustomerParty",
                    new XElement(cac + "Party",
                        new XElement(cac + "PartyIdentification",
                            new XElement(cbc + "ID", new XAttribute("schemeID", "TN"), h["CustomerTaxNumber"].ToString())
                        ),
                        new XElement(cac + "PartyTaxScheme",
                            new XElement(cbc + "CompanyID", h["CustomerTaxNumber"].ToString()),
                            new XElement(cac + "TaxScheme",
                                new XElement(cbc + "ID", "VAT")
                            )
                        ),
                        new XElement(cac + "PartyLegalEntity",
                            new XElement(cbc + "RegistrationName", h["CustomerName"].ToString())
                        )
                    )
                ),

                // TaxTotal
                new XElement(cac + "TaxTotal",
                    new XElement(cbc + "TaxAmount", new XAttribute("currencyID", currency), totalTax.ToString("0.###", _ci))
                ),

                // MonetaryTotal
                new XElement(cac + "LegalMonetaryTotal",
                    new XElement(cbc + "TaxExclusiveAmount", new XAttribute("currencyID", currency), totalBeforeTax.ToString("0.###", _ci)),
                    new XElement(cbc + "TaxInclusiveAmount", new XAttribute("currencyID", currency), totalAfterTax.ToString("0.###", _ci)),
                    new XElement(cbc + "PayableAmount", new XAttribute("currencyID", currency), totalAfterTax.ToString("0.###", _ci))
                )
            );

            foreach (DataRow r in dtLines.Rows)
            {
                decimal qty = Convert.ToDecimal(r["Quantity"]);
                decimal price = Convert.ToDecimal(r["UnitPrice"]);
                decimal lineBeforeTax = Convert.ToDecimal(r["LineBeforeTax"]);
                decimal lineTax = Convert.ToDecimal(r["LineTax"]);
                decimal taxRate = Convert.ToDecimal(r["TaxRate"]);

                var line = new XElement(cac + "CreditNoteLine",
                    new XElement(cbc + "ID", r["LineId"].ToString()),
                    new XElement(cbc + "CreditedQuantity", new XAttribute("unitCode", "PCE"), qty.ToString("0.###", _ci)),
                    new XElement(cbc + "LineExtensionAmount", new XAttribute("currencyID", currency), lineBeforeTax.ToString("0.###", _ci)),

                    new XElement(cac + "TaxTotal",
                        new XElement(cbc + "TaxAmount", new XAttribute("currencyID", currency), lineTax.ToString("0.###", _ci)),
                        new XElement(cac + "TaxSubtotal",
                            new XElement(cbc + "TaxableAmount", new XAttribute("currencyID", currency), lineBeforeTax.ToString("0.###", _ci)),
                            new XElement(cbc + "TaxAmount", new XAttribute("currencyID", currency), lineTax.ToString("0.###", _ci)),
                            new XElement(cac + "TaxCategory",
                                new XElement(cbc + "ID", new XAttribute("schemeAgencyID", "6"), new XAttribute("schemeID", "UN/ECE 5305"), "S"),
                                new XElement(cbc + "Percent", taxRate.ToString("0.##", _ci)),
                                new XElement(cac + "TaxScheme",
                                    new XElement(cbc + "ID", new XAttribute("schemeAgencyID", "6"), new XAttribute("schemeID", "UN/ECE 5153"), "VAT")
                                )
                            )
                        )
                    ),

                    new XElement(cac + "Item",
                        new XElement(cbc + "Name", r["ProductName"].ToString())
                    ),

                    new XElement(cac + "Price",
                        new XElement(cbc + "PriceAmount", new XAttribute("currencyID", currency), price.ToString("0.###", _ci))
                    )
                );

                xCN.Add(line);
            }

            var doc = new XDocument(xCN);
            return doc.ToString(SaveOptions.DisableFormatting);
        }
    }
}