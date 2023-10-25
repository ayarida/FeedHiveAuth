using Dapper;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace FeedHiveAuth.Data.Repositories
{
    public class BaseRepository<T> where T : BaseModel
    {
        protected string KeyColumn = "Id";
        protected string SqlSelect => "SELECT " + Columns.AddBraces() + " FROM " + TableName;
        protected string SqlInsert => Columns.GenerateInsertQuery(TableName);
        protected string SqlDelete => "DELETE FROM " + TableName + " WHERE Id=@Id;";

        protected string SqlUpdate => Columns.GenerateUpdateQuery(TableName, KeyColumn, "Id,CreationDate");

        protected string SqlCount => "SELECT COUNT(*) FROM " + TableName;
        protected string TableName;
        protected string Columns;
        public static string _connectionString = DatabaseConnection.GetConnectionStrings();

        public static CustomSqlConnection connection = DatabaseConnection.GetConnection(_connectionString);

        /*public static string GetConnectionStrings()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            var connString = configuration.GetConnectionString("DefaultConnection");
            return connString;
        }*/
        public T Get(string id)
        {
            if (string.IsNullOrEmpty(SqlSelect))
                return default;

            var sql = SqlSelect + $" WHERE Id=@Id;";
            using (connection)
            {
                var record = connection.Query<T>(SqlSelect + $" WHERE Id=@Id;", new { Id = new[] { id } }).FirstOrDefault();
                return record;
            }

        }
        public void Delete(string id)
        {
            using (connection)
            {
                var resultQuery = connection.Query<T>(SqlDelete, new { Id = new[] { id } });
            }
        }

        public void Update(T entity)
        {
            using (connection)
            {
                var resultQuery = connection.Query<T>(SqlUpdate);
            }
        }

        public void UpdateColumn(string column, object value, string id)
        {
            var x = "Ssksl";
            using (connection)
            {
                var result = connection.Query<T>("UPDATE " + TableName + " SET " + column + "=@value WHERE Id=@id", new { value, Id = id });

            }

            //_connection.globalSqlConnection.Query<T>("UPDATE " + TableName + " SET " + column + "=@value WHERE Id=@id", new { value, Id = id });               
            //Execute("UPDATE " + TableName + " SET " + column + "=@value WHERE Id=@id", new { value, id });
        }
        public int Execute(string sql, dynamic param = null)
        {
            try
            {
                var query = sql += "\nGO";
                var sqlBatch = "";
                var res = 0;
                using (connection)
                {
                    foreach (string line in query.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.ToUpperInvariant().Trim() == "GO")
                        {
                            if (sqlBatch.IsNotNullOrEmpty())
                                res = connection.Query<T>(sqlBatch, param);
                            sqlBatch = string.Empty;
                        }
                        else
                        {
                            sqlBatch += line + "\n";
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(sql + ":" + ex);
                throw;
            }
        }
        public void ExecuteQuery(string query)
        {
            var sqlConxn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, sqlConxn);
            if (sqlConxn.State != ConnectionState.Open)
            {
                sqlConxn.Open();
            }
            int i = cmd.ExecuteNonQuery();
        }

        public IEnumerable<T> Query<T>(string sql, dynamic param = null)
        {
            try
            {
                //var start = DateTime.Now.Ticks;
                using (connection)
                {
                    var result = connection.Query<T>(sql, param);
#if DEBUG
                    //WriteStaticJsonData(result, sql, param);
#endif
                    return result;
                }
                //var end = DateTime.Now.Ticks;
                //var diff = end - start;
                //var span = TimeSpan.FromTicks(diff).TotalSeconds;
                //Logger.Info(typeof(RequestHelper), sql + "\ntook " + span + " sec to execute");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception(ex.FullMessage());
            }
        }


    }
}
