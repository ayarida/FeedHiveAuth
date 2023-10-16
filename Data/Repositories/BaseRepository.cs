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
        public DatabaseConnection _connection = new DatabaseConnection();
       

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

        public void Update(T entity)
        {
            using (var connection = _connection.DbSqlConnection)
            {
                var resultQuery = connection.Query<T>(SqlUpdate);
            }
        }

        public void ExecuteQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query,_connection.globalSqlConnection);
            if (_connection.globalSqlConnection.State != ConnectionState.Open)
            {
                _connection.globalSqlConnection.Open();
            }
            int i = cmd.ExecuteNonQuery();
        }

        public IEnumerable<T> Query<T>(string sql, dynamic param = null)
        {
            try
            {
                //var start = DateTime.Now.Ticks;
                using (var connection = _connection.DbSqlConnection)
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
