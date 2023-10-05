using System.Data.SqlClient;

namespace FeedHiveAuth.Data
{
    public class DatabaseConnection
    {
        public DatabaseConnection() {
            ConnectionString = GetConnectionStrings();
            DbSqlConnection = GetConnection(ConnectionString);
            globalSqlConnection = new SqlConnection(ConnectionString);
        }
        public SqlConnection globalSqlConnection; 
        public string ConnectionString;
        public CustomSqlConnection DbSqlConnection;

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
