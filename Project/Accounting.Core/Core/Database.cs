using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using Accounting.Core.EInvoice;

namespace Accounting.Core
{
    public class Database
    {
        private string connectionString =
@"Data Source=.\SQLEXPRESS;Initial Catalog=AccountingCoreDB;Integrated Security=True";
        public string ConnectionString { get; } =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=AccountingCoreDB;Integrated Security=True";

            // =========================
            // Execute (INSERT / UPDATE / DELETE)
            // =========================
            public int Execute(
                string query,
                List<SqlParameter> parameters,
                SqlConnection con,
                SqlTransaction trans
            )
            {
                using (SqlCommand cmd = new SqlCommand(query, con, trans))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }

            // =========================
            // Read (SELECT) with Transaction
            // =========================
            public DataTable Read(
                string query,
                List<SqlParameter> parameters,
                SqlConnection con,
                SqlTransaction trans
            )
            {
                DataTable table = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, con, trans))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    table.Load(cmd.ExecuteReader());
                }
                return table;
            }

            // =========================
            // ExecuteScalar
            // =========================
            public object Scalar(
                string query,
                List<SqlParameter> parameters,
                SqlConnection con,
                SqlTransaction trans
            )
            {
                using (SqlCommand cmd = new SqlCommand(query, con, trans))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteScalar();
                }
            }
   
        private string GetQRPathFromDatabase(int invoiceId)
        {
            string qrPath = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT QRCodePath FROM Invoices WHERE InvoiceId = @Id", con);
                cmd.Parameters.AddWithValue("@Id", invoiceId);
                qrPath = cmd.ExecuteScalar()?.ToString();
            }
            return qrPath;
        }

        internal DataTable Read(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
    }

