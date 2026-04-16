using System.Data.SqlClient;

namespace Accounting.Core.EInvoice
{
    public class ICVGenerator
    {
        private string _cs;

        public ICVGenerator(string connectionString)
        {
            _cs = connectionString;
        }

        public int GetNextICV()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                "SELECT ISNULL(MAX(ICV),0)+1 FROM Invoices", con);

                return (int)cmd.ExecuteScalar();
            }
        }
    }
}