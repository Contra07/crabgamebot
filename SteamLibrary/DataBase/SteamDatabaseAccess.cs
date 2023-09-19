using SteamLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SteamLibrary.DataBase
{
    public class SteamDatabaseAccess
    {
        private DbProviderFactory _factory;
        public SteamDatabaseAccess(DbProviderFactory factory)
        {
            _factory = factory;
        }

        public List<AccountCredentialsModel> GetAccountsCredentials()
        {
            var users = new List<AccountCredentialsModel>();
            using (IDbConnection connection = _factory.CreateConnection())
            {
                connection.ConnectionString = LoadConnectionString();
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction()) {
                    try
                    {
                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM Credentials";
                            var result = command.ExecuteReader();
                            while (result.Read())
                            {
                                users.Add(new AccountCredentialsModel(result.GetString(0), result.GetString(1), result.GetString(2), result.GetString(3), result.GetString(4)));
                            }
                        }
                        transaction.Commit();
                    }
                    catch {
                        transaction.Rollback();
                    }
                }
                connection.Close();
            }
            return users;
        }

        public Task<List<AccountCredentialsModel>> GetAccountsCredentialsAsync()
        {
            var task = new Task<List<AccountCredentialsModel>>(GetAccountsCredentials);
            task.Start();
            return task;
        }

        public bool InsertUser(AccountCredentialsModel user) {
            bool result = false;
            using (IDbConnection connection = _factory.CreateConnection())
            {
                connection.ConnectionString = LoadConnectionString();
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "INSERT INTO " +
                                "Credentials (username, password, email, secret, number) " +
                                "VALUES (@username, @password, @email, @secret, @number)";
                            
                            var username = _factory.CreateParameter();
                            username.ParameterName = "@username";
                            username.DbType = DbType.String;
                            username.Value = user.Login;
                            command.Parameters.Add(username);
                            
                            var password = _factory.CreateParameter();
                            password.ParameterName = "@password";
                            password.DbType = DbType.String;
                            password.Value = user.Password;
                            command.Parameters.Add(password);
                            
                            var email = _factory.CreateParameter();
                            email.ParameterName = "@email";
                            email.DbType = DbType.String;
                            email.Value = user.Email;
                            command.Parameters.Add(email);
                            
                            var secret = _factory.CreateParameter();
                            secret.ParameterName = "@secret";
                            secret.DbType = DbType.String;
                            secret.Value = user.Secret;
                            command.Parameters.Add(secret);
                            
                            var number = _factory.CreateParameter();
                            number.ParameterName = "@number";
                            number.DbType = DbType.String;
                            number.Value = user.Number;
                            command.Parameters.Add(number);
                            result = !(command.ExecuteNonQuery() == 0);
                            if (!result) {
                                transaction.Rollback();
                            }
                        }
                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                    }
                }
                connection.Close();
            }
            return result;
        }

        public Task<bool> InsertUserAsync(AccountCredentialsModel user) {
            var task = new Task<bool>(() => InsertUser(user));
            task.Start();
            return task;
        }


        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
