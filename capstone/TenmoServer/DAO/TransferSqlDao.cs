using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.Xml;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private static IAccountDao accountDao;
        private readonly string connectionString;
        public TransferSqlDao(string dbConnectionString, IAccountDao _accountDao)
        {
            connectionString = dbConnectionString;
            accountDao = _accountDao;
        }

        public Transfer SendTransfer(int account_from, int account_to, decimal amount)
        {
            int newTransferId = 0;
            if (accountDao.GetBalanceByAccount(account_from) > amount && account_from != account_to && amount > 0)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION; UPDATE account SET balance -= @amount WHERE account_id = @fromAccount; UPDATE account SET balance += @amount WHERE account_id = @toAccount;" +
                            "INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.transfer_id VALUES (2, 2, @fromAccount, @toAccount, @amount); COMMIT;", conn);

                        cmd.Parameters.AddWithValue("@fromAccount", account_from);
                        cmd.Parameters.AddWithValue("@toAccount", account_to);
                        cmd.Parameters.AddWithValue("@amount", amount);

                        newTransferId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return TransferDetails(newTransferId);
            }
            else
            {
                Console.WriteLine($"Unable to complete transfer, please try again");
                return new Transfer(); //might be a lil weird, just don't want to break the program when we can't transfer.
            } //were going to throw exception, but would probably break program.
        }
        public IList<Transfer> GetAllTransfers(int accountId)
        {
            IList<Transfer> transfers = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfer WHERE account_from = @account_id OR account_to = @account_id;", conn);
                    cmd.Parameters.AddWithValue("@account_id", accountId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Transfer t = GetTransferFromReader(reader);
                        transfers.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return transfers;
        }
        public Transfer TransferDetails(int transferId)
        {
            Transfer transfer = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfer " +
                        "WHERE transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        transfer = GetTransferFromReader(reader);                         
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return transfer;
        }
        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"]),
            };

            return t;
        }
    }
}
