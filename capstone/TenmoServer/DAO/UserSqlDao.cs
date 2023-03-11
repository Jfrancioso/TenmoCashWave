using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class UserSqlDao : IUserDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM tenmo_user WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }

        public string GetUsernameByTransfer(Transfer transfer, int accountId)
        {
            string username = "";
            if (transfer != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        if (transfer.AccountFrom == accountId)
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("SELECT username FROM tenmo_user " +
                                "FULL JOIN account ON account.user_id = tenmo_user.user_id " +
                                "FULL JOIN transfer ON account.account_id = transfer.account_to " +
                                "WHERE account_id != @accountId AND transfer_id = @transferId;", conn);
                            cmd.Parameters.AddWithValue("@accountId", accountId);
                            cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                username = Convert.ToString(reader["username"]);
                            }
                        }
                        else
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("SELECT username FROM tenmo_user " +
                                "FULL JOIN account ON account.user_id = tenmo_user.user_id " +
                                "FULL JOIN transfer ON account.account_id = transfer.account_from " +
                                "WHERE account_id != @accountId AND transfer_id = @transferId;", conn);
                            cmd.Parameters.AddWithValue("@accountId", accountId);
                            cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                username = Convert.ToString(reader["username"]);
                            }
                        }
                    }

                }
                catch (SqlException)
                {
                    throw;
                }
                return username;
            }
            else
            {
                return "";

            }
        }

        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM tenmo_user", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = GetUserFromReader(reader);
                        returnUsers.Add(u);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO tenmo_user (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO account (user_id, balance) VALUES (@userid, @startBalance)", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetUser(username);
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };

            return u;
        }
    }
}
