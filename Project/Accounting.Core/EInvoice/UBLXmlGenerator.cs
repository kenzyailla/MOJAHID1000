using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Accounting.Core.EInvoice
{
    public static class UBLXmlGenerator
    {
        public static string GenerateInvoiceXML(
            string connectionString,
            int invoiceId,
            string sellerName,
            string sellerTaxNo)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                //---------------------------------------
                // Header
                //---------------------------------------

                SqlCommand cmd = new SqlCommand(@"
SELECT 
InvoiceNumber,
InvoiceDate,
TotalBeforeTax,
TotalTax,
TotalAfterTax
FROM Invoices
WHERE InvoiceId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dtHeader = new DataTable();
                dtHeader.Load(cmd.ExecuteReader());

                DataRow h = dtHeader.Rows[0];

                //---------------------------------------
                // Lines
                //---------------------------------------

                SqlCommand cmdLines = new SqlCommand(@"
SELECT 
ProductId,
Quantity,
UnitPrice,
TaxRate,
TotalBeforeTax,
TotalTax,
TotalAfterTax
FROM InvoiceLines
WHERE InvoiceId=@Id", con);

                cmdLines.Parameters.AddWithValue("@Id", invoiceId);

                DataTable dtLines = new DataTable();
                dtLines.Load(cmdLines.ExecuteReader());

                //---------------------------------------
                // Namespaces
                //---------------------------------------

                XNamespace ns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

                //---------------------------------------
                // Invoice
                //---------------------------------------

                XElement invoice =
                    new XElement(ns + "Invoice",

                        new XAttribute(XNamespace.Xmlns + "cac", cac),
                        new XAttribute(XNamespace.Xmlns + "cbc", cbc),

                        new XElement(cbc + "UBLVersionID", "2.1"),
                        new XElement(cbc + "ProfileID", "reporting:1.0"),

                        new XElement(cbc + "ID",
                            h["InvoiceNumber"].ToString()),

                        new XElement(cbc + "IssueDate",
                            Convert.ToDateTime(h["InvoiceDate"]).ToString("yyyy-MM-dd")),

                        //---------------------------------------
                        // Seller
                        //---------------------------------------

                        new XElement(cac + "AccountingSupplierParty",

                            new XElement(cac + "Party",

                                new XElement(cac + "PartyLegalEntity",
                                    new XElement(cbc + "RegistrationName",
                                        sellerName)
                                ),

                                new XElement(cac + "PartyTaxScheme",

                                    new XElement(cbc + "CompanyID",
                                        sellerTaxNo),

                                    new XElement(cac + "TaxScheme",
                                        new XElement(cbc + "ID", "VAT")
                                    )
                                )
                            )
                        ),

                        //---------------------------------------
                        // Customer
                        //---------------------------------------

                        new XElement(cac + "AccountingCustomerParty",

                            new XElement(cac + "Party",

                                new XElement(cac + "PartyLegalEntity",
                                    new XElement(cbc + "RegistrationName",
                                        "عميل نقدي")
                                )
                            )
                        ),

                        //---------------------------------------
                        // Tax Total
                        //---------------------------------------

                        new XElement(cac + "TaxTotal",

                            new XElement(cbc + "TaxAmount",
                                new XAttribute("currencyID", "JOD"),
                                h["TotalTax"])
                        ),

                        //---------------------------------------
                        // Totals
                        //---------------------------------------

                        new XElement(cac + "LegalMonetaryTotal",

                            new XElement(cbc + "LineExtensionAmount",
                                new XAttribute("currencyID", "JOD"),
                                h["TotalBeforeTax"]),

                            new XElement(cbc + "TaxInclusiveAmount",
                                new XAttribute("currencyID", "JOD"),
                                h["TotalAfterTax"])
                        )
                    );

                //---------------------------------------
                // Invoice Lines
                //---------------------------------------

                int lineId = 1;

                foreach (DataRow r in dtLines.Rows)
                {
                    XElement line =

                        new XElement(cac + "InvoiceLine",

                            new XElement(cbc + "ID", lineId++),

                            new XElement(cbc + "InvoicedQuantity",
                                new XAttribute("unitCode", "PCE"),
                                r["Quantity"]),

                            new XElement(cbc + "LineExtensionAmount",
                                new XAttribute("currencyID", "JOD"),
                                r["TotalBeforeTax"]),

                            new XElement(cac + "Item",

                                new XElement(cbc + "Name",
                                    "Product " + r["ProductId"])
                            ),

                            new XElement(cac + "Price",

                                new XElement(cbc + "PriceAmount",
                                    new XAttribute("currencyID", "JOD"),
                                    r["UnitPrice"])
                            )
                        );

                    invoice.Add(line);
                }

                //---------------------------------------
                // XML
                //---------------------------------------

                XDocument xml = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    invoice
                );

                return xml.ToString();
            }
        }
    }
}