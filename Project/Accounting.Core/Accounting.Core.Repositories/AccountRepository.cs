using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using Accounting.Core.Models;
using System.Data;

namespace Accounting.Core.Repositories
{
    public class AccountRepository
    {
        private string _connectionString;

        public AccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Account> GetAllAccounts()
        {
            List<Account> list = new List<Account>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
SELECT AccountId, AccountCode, AccountName, AccountType, IsActive
FROM Accounts
WHERE IsActive = 1
ORDER BY AccountCode
";

                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Account
                    {
                        AccountId = (int)dr["AccountId"],
                        AccountCode = dr["AccountCode"].ToString(),
                        AccountName = dr["AccountName"].ToString(),
                        AccountType = dr["AccountType"].ToString(),
                        IsActive = (bool)dr["IsActive"]
                    });
                }
            }

            return list;
        }
        public DataTable GetTrialBalance(DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string sql = @"
        SELECT
            a.AccountId,
            a.AccountCode,
            a.AccountName,

            ISNULL(SUM(jl.Debit),0) AS Debit,
            ISNULL(SUM(jl.Credit),0) AS Credit,

            ISNULL(SUM(jl.Debit),0) - ISNULL(SUM(jl.Credit),0) AS Balance

        FROM Accounts a

        LEFT JOIN JournalLines jl
            ON a.AccountId = jl.AccountId

        LEFT JOIN JournalEntries je
            ON jl.JournalId = je.JournalId
            AND je.EntryDate >= @FromDate
            AND je.EntryDate < DATEADD(DAY,1,@ToDate)

        GROUP BY
            a.AccountId,
            a.AccountCode,
            a.AccountName

        ORDER BY a.AccountCode
        ";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);

                da.SelectCommand.Parameters.AddWithValue("@FromDate", fromDate);
                da.SelectCommand.Parameters.AddWithValue("@ToDate", toDate);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

    }
}


