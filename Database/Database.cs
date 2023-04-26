using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System;
using Tube_Traveller.Accounts;

namespace Tube_Traveller.Database
{
    public class Database
    {
        private static SqliteConnectionStringBuilder connectionStringBuilder = new();

        public Database()
        {
            connectionStringBuilder.DataSource = "./Database/AccountsDB.db"; //Where the database is
        }

        protected static string[]? GetUserInfoByUsername(string username)
        {
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString)) //Connects to database
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();

                selectCmd.CommandText = "SELECT * FROM User WHERE Username = @username";
                selectCmd.Parameters.AddWithValue("@username", username); //Parameters are used to help prevent SQL injection attacks

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] accountColumns = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++) //Getting all columns
                        {
                            accountColumns[i] += reader.GetString(i);
                        }
                        return accountColumns;
                    }
                }
            }

            return null;
        }

        protected static string[]? GetUserInfoByEmail(string email)
        {
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();

                selectCmd.CommandText = $"SELECT * FROM User WHERE Email = @email";
                selectCmd.Parameters.AddWithValue("@email", email);

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] accountColumns = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            accountColumns[i] += reader.GetString(i);
                        }
                        return accountColumns;
                    }
                }
            }

            return null;
        }

        protected static string? GetUserId(string username)
        {
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();

                selectCmd.CommandText = $"SELECT Id FROM User WHERE Username = @username";
                selectCmd.Parameters.AddWithValue("@username", username);
                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }

            return null;
        }

        protected static string[]? GetExtraInfo(string id)
        {
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();

                selectCmd.CommandText = $"SELECT * FROM Extra WHERE UserId = @Id";
                selectCmd.Parameters.AddWithValue("@Id", id);
                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] accountColumns = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            accountColumns[i] += reader.GetString(i);
                        }
                        return accountColumns;
                    }
                }
            }

            return null;
        }


        public void AddAccount(Account userAccount)
        {
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) //Ensures that all data would be inserted at the same time
                {
                    try
                    {
                        AddUserInfo(connection ,userAccount.GetUsername(), userAccount.GetFirstName(), userAccount.GetLastName(), userAccount.GetPassword(), userAccount.GetEmail());
                        AddExtraInfo(connection ,userAccount.GetRouteMethod(), userAccount.GetHomeStation());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void AddUserInfo(SqliteConnection connection, string username, string firstName, string? lastName, string password, string? email)
        {
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO User(Username, Firstname, Lastname, Password, Email) VALUES(@username, @firstName, @lastName, @password, @email)";
            insertCmd.Parameters.AddWithValue("@username", username); 
            insertCmd.Parameters.AddWithValue("@firstName", firstName);
            insertCmd.Parameters.AddWithValue("@lastName", lastName);
            insertCmd.Parameters.AddWithValue("@password", password);
            insertCmd.Parameters.AddWithValue("@email", email);

            insertCmd.ExecuteNonQuery();
            //Does command and doesn't return any results
        }

        private void AddExtraInfo(SqliteConnection connection, string routeMethod, string? homeStation)
        {
            var insertCmd = connection.CreateCommand();

            insertCmd.CommandText = $"INSERT INTO Extra(RouteMethod, HomeStation) VALUES(@routeMethod, @homeStation)";
            insertCmd.Parameters.AddWithValue("@routeMethod", routeMethod);
            insertCmd.Parameters.AddWithValue("@homeStation", homeStation);

            insertCmd.ExecuteNonQuery();
        }
    }
}
