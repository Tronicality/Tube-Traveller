using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace Tube_Traveller
{
    internal class Database
    {
        private SqliteConnectionStringBuilder connectionStringBuilder = new();

        public Database(string DataSource)
        {
            connectionStringBuilder.DataSource = DataSource;
        }

        public void ExecuteCommand()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            //connectionStringBuilder.DataSource = "./accountsDB.db";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "CREATE TABLE accounts(INTEGER ID PRIMARY KEY NOT NULL UNIQUE, TEXT username, TEXT password UNIQUE, TEXT homeStation);";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
