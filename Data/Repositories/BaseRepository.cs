using Dapper;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace FeedHiveAuth.Data.Repositories
{
    public class BaseRepository<T> where T : BaseModel
    {
        protected string SqlSelect => "SELECT " + Columns.AddBraces() + " FROM " + TableName;
        protected string SqlInsert => Columns.GenerateInsertQuery(TableName);
        public string SqlDelete => "DELETE FROM " + TableName + " WHERE Id=@Id;";
        protected string SqlCount => "SELECT COUNT(*) FROM " + TableName;
        protected string KeyColumn = "Id";
        protected string TableName;
        protected string Columns;
        public DatabaseConnection _connection = new DatabaseConnection();
        /*public string ConnectionString = GetConnectionStrings();
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
        }*/

        public T Get(string id)
        {
            using (var connection = _connection.DbSqlConnection)
            {
                var returnedUser = connection.Query<T>("SELECT * FROM AspNetUsers WHERE id IN @ids", new { ids = new[] { id } }).FirstOrDefault();
                return returnedUser;
            }
        }
        public void Delete(string id)
        {
            using (var connection = _connection.DbSqlConnection)
            {
                var resultQuery = connection.Query<T>(SqlDelete, new { Id = new[] { id } });
            }
        }

        public void ExecuteQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query,_connection.globalSqlConnection);
            if (!_connection.globalSqlConnection.State.Equals(1))
            {
                _connection.globalSqlConnection.Open();
            }
            int i = cmd.ExecuteNonQuery();
        }



    }
}
