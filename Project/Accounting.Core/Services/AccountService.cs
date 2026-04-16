using Accounting.Core.Models;
using Accounting.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Services
{
    public class AccountService
    {
        private string _connectionString;
        private AccountRepository _repo;

        public AccountService(string connectionString)
        {
            _connectionString = connectionString;
            _repo = new AccountRepository(connectionString);
        }
        public List<Account> GetAllAccounts()
        {
            return _repo.GetAllAccounts();
        }
        public DataTable GetCashAndBankAccounts()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"


SELECT AccountId, AccountCode, AccountName
FROM Accounts
WHERE AccountId IN (1,10)   -- الصندوق والبنك
AND IsActive = 1
";


                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        public DataTable GetTrialBalance(DateTime from, DateTime to)
        {
            return _repo.GetTrialBalance(from, to);
        }


    }


}


