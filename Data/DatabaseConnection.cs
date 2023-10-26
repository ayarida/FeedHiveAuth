using FeedHiveAuth.Data.Extensions;
using System.Data.SqlClient;
using System.Data;

namespace FeedHiveAuth.Data
{
    public static class DatabaseConnection
    {
        /*public DatabaseConnection() {
            ConnectionString = GetConnectionStrings();
            DbSqlConnection = GetConnection(ConnectionString);
            globalSqlConnection = new SqlConnection(ConnectionString);
            globalSqlConnection.ConnectionString = ConnectionString;
            if (globalSqlConnection.State.Value()!= ConnectionState.Open.Value()) { globalSqlConnection.Open(); }
        }*/
        public static SqlConnection globalSqlConnection; 
        public static string ConnectionString = GetConnectionStrings();
        public static CustomSqlConnection DbSqlConnection;

        public static string GetConnectionStrings()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            var connString = configuration.GetConnectionString("DefaultConnection");
            return connString;
        }
        public static CustomSqlConnection GetConnection(string connectionString)
        {

            return OpenConnection(connectionString);
        }

        private static CustomSqlConnection OpenConnection(string connectionString)
        {

            var connection = new CustomSqlConnection(connectionString);
            connection.OpenWithRetry();
            return connection;
        }
    }
}
