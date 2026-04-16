using Accounting.Core.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Accounting.Core.Repositories
{
    public class JournalRepository
    {
        private readonly string _connectionString;

        public JournalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddJournalEntry(
       SqlConnection con,
       SqlTransaction trans,
       JournalEntry entry)
        {
            string insertEntry = @"
INSERT INTO JournalEntries
(EntryDate,ReferenceType,ReferenceId,Description)
VALUES
(@Date,@RefType,@RefId,@Desc);
SELECT SCOPE_IDENTITY();
";

            int journalId;

            using (SqlCommand cmd = new SqlCommand(insertEntry, con, trans))
            {
                cmd.Parameters.AddWithValue("@Date", entry.EntryDate);
                cmd.Parameters.AddWithValue("@RefType", entry.ReferenceType);
                cmd.Parameters.AddWithValue("@RefId", entry.ReferenceId);
                cmd.Parameters.AddWithValue("@Desc", entry.Description ?? "");

                journalId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            foreach (var line in entry.Lines)
            {
                string insertLine = @"
INSERT INTO JournalLines
(JournalId,AccountId,Debit,Credit)
VALUES
(@Journal,@Account,@Debit,@Credit)";

                using (SqlCommand cmd = new SqlCommand(insertLine, con, trans))
                {
                    cmd.Parameters.AddWithValue("@Journal", journalId);
                    cmd.Parameters.AddWithValue("@Account", line.AccountId);
                    cmd.Parameters.AddWithValue("@Debit", line.Debit);
                    cmd.Parameters.AddWithValue("@Credit", line.Credit);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddJournalEntry(JournalEntry entry)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    AddJournalEntry(con, trans, entry);
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

    }
}

