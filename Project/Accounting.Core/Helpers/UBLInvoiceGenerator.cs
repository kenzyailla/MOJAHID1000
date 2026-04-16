using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml.Linq;

namespace Accounting.Core.Helpers
{
    public class UBLInvoiceGenerator
    {
        private readonly string _cs;
        private readonly CultureInfo _ci = CultureInfo.InvariantCulture;

        public UBLInvoiceGenerator(string connectionString)
        {
            _cs = connectionString;
        }

        public string GenerateInvoiceXml(int invoiceId,
            string sellerName,
            string sellerTaxNo,
            string invoiceTypeCode = "388",
            string invoiceTypeName = "فاتورة ضريبية",
            string currency = "JOD")
        {
            DataTable dtInv = new DataTable();
            DataTable dtLines = new DataTable();

            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                string sqlInv = @"
SELECT 
    i.InvoiceId, i.InvoiceNumber, i.InvoiceDate, i.CustomerId,
    i.TotalBeforeTax, i.TotalTax, i.TotalAfterTax,
    c.Name AS CustomerName, ISNULL(c.TaxNumber,'') AS CustomerTaxNumber
FROM Invoices i
JOIN Customers c ON c.CustomerId = i.CustomerId
WHERE i.InvoiceId = @Id;
";
                var da = new SqlDataAdapter(sqlInv, con);
                da.SelectCommand.Parameters.AddWithValue("@Id", invoiceId);
                da.Fill(dtInv);

                if (dtInv.Rows.Count == 0)
                    throw new Exception("لم يتم العثور على الفاتورة.");

                string sqlLines = @"
SELECT
    l.LineId,
    l.ProductId,
    p.Name AS ProductName,
    l.Quantity,
    l.UnitPrice,
    l.Discount,
    l.TaxRate,
    l.LineTotal,
    ISNULL(l.TotalBeforeTax, (l.Quantity*l.UnitPrice) - ISNULL(l.Discount,0)) AS TotalBeforeTax,
    ISNULL(l.TotalTax, 0) AS TotalTax,
    ISNULL(l.TotalAfterTax, 0) AS TotalAfterTax
FROM InvoiceLines l
JOIN Products p ON p.ProductId = l.ProductId
WHERE l.InvoiceId = @Id
ORDER BY l.LineId;
";
                var da2 = new SqlDataAdapter(sqlLines, con);
                da2.SelectCommand.Parameters.AddWithValue("@Id", invoiceId);
                da2.Fill(dtLines);
            }

            DataRow inv = dtInv.Rows[0];

            DateTime issueDate = Convert.ToDateTime(inv["InvoiceDate"]);
            string invoiceNumber = inv["InvoiceNumber"].ToString();

            decimal totalBeforeTax = Convert.ToDecimal(inv["TotalBeforeTax"]);
            decimal totalTax = Convert.ToDecimal(inv["TotalTax"]);
            decimal totalAfterTax = Convert.ToDecimal(inv["TotalAfterTax"]);

            // Namespaces (مثل XML الذي أرسلته)
            XNamespace ns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            XNamespace ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

            var xInvoice = new XElement(ns + "Invoice",
                new XAttribute(XNamespace.Xmlns + "cbc", cbc),
                new XAttribute(XNamespace.Xmlns + "cac", cac),
                new XAttribute(XNamespace.Xmlns + "ext", ext),

                new XElement(cbc + "ProfileID", "reporting:1.0"),
                new XElement(cbc + "ID", invoiceNumber),

                // لو كان UUID محفوظ سابقًا استخدمه، وإلا أنشئ جديد
                new XElement(cbc + "UUID",
                    (inv.Table.Columns.Contains("UUID") && inv["UUID"] != DBNull.Value && inv["UUID"].ToString().Trim() != "")
                        ? inv["UUID"].ToString()
                        : Guid.NewGuid().ToString()
                ),

                new XElement(cbc + "IssueDate", issueDate.ToString("yyyy-MM-dd")),
                new XElement(cbc + "InvoiceTypeCode", new XAttribute("name", invoiceTypeName), invoiceTypeCode),
                new XElement(cbc + "Note", ""),
                new XElement(cbc + "DocumentCurrencyCode", currency),
                new XElement(cbc + "TaxCurrencyCode", currency),

                // ICV مثل مثال ملفك
                new XElement(cac + "AdditionalDocumentReference",
                    new XElement(cbc + "ID", "ICV"),
                    new XElement(cbc + "UUID", invoiceNumber)
                ),

                // Supplier
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

                // Customer
                new XElement(cac + "AccountingCustomerParty",
                    new XElement(cac + "Party",
                        new XElement(cac + "PartyIdentification",
                            new XElement(cbc + "ID",
                                new XAttribute("schemeID", "TN"),
                                inv["CustomerTaxNumber"].ToString()
                            )
                        ),
                        new XElement(cac + "PostalAddress",
                            new XElement(cac + "Country",
                                new XElement(cbc + "IdentificationCode", "JO")
                            )
                        ),
                        new XElement(cac + "PartyTaxScheme",
                            new XElement(cbc + "CompanyID", inv["CustomerTaxNumber"].ToString()),
                            new XElement(cac + "TaxScheme",
                                new XElement(cbc + "ID", "VAT")
                            )
                        ),
                        new XElement(cac + "PartyLegalEntity",
                            new XElement(cbc + "RegistrationName", inv["CustomerName"].ToString())
                        )
                    )
                ),

                // Discount header (مثل مثال ملفك)
                new XElement(cac + "AllowanceCharge",
                    new XElement(cbc + "ChargeIndicator", "false"),
                    new XElement(cbc + "AllowanceChargeReason", "discount"),
                    new XElement(cbc + "Amount", new XAttribute("currencyID", currency), "0.000")
                ),

                // Tax Total
                new XElement(cac + "TaxTotal",
                    new XElement(cbc + "TaxAmount", new XAttribute("currencyID", currency), totalTax.ToString("0.###", _ci))
                ),

                // Monetary Total
                new XElement(cac + "LegalMonetaryTotal",
                    new XElement(cbc + "TaxExclusiveAmount", new XAttribute("currencyID", currency), totalBeforeTax.ToString("0.###", _ci)),
                    new XElement(cbc + "TaxInclusiveAmount", new XAttribute("currencyID", currency), totalAfterTax.ToString("0.###", _ci)),
                    new XElement(cbc + "AllowanceTotalAmount", new XAttribute("currencyID", currency), "0.00"),
                    new XElement(cbc + "PayableAmount", new XAttribute("currencyID", currency), totalAfterTax.ToString("0.###", _ci))
                )
            );

            // Lines
            foreach (DataRow r in dtLines.Rows)
            {
                decimal qty = Convert.ToDecimal(r["Quantity"]);
                decimal price = Convert.ToDecimal(r["UnitPrice"]);
                decimal lineBeforeTax = Convert.ToDecimal(r["TotalBeforeTax"]);
                decimal lineTax = Convert.ToDecimal(r["TotalTax"]);
                decimal taxRate = Convert.ToDecimal(r["TaxRate"]);

                var line = new XElement(cac + "InvoiceLine",
                    new XElement(cbc + "ID", r["LineId"].ToString()),
                    new XElement(cbc + "InvoicedQuantity", new XAttribute("unitCode", "PCE"), qty.ToString("0.###", _ci)),
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
                        new XElement(cbc + "PriceAmount", new XAttribute("currencyID", currency), price.ToString("0.###", _ci)),
                        new XElement(cac + "AllowanceCharge",
                            new XElement(cbc + "ChargeIndicator", "false"),
                            new XElement(cbc + "AllowanceChargeReason", "DISCOUNT"),
                            new XElement(cbc + "Amount", new XAttribute("currencyID", currency), "0.00")
                        )
                    )
                );

                xInvoice.Add(line);
            }

            var doc = new XDocument(xInvoice);
            return doc.ToString(SaveOptions.DisableFormatting);
        }
    }
}