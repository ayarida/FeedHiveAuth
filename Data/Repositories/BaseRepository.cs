using Dapper;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace FeedHiveAuth.Data.Repositories
{
    public class BaseRepository<T> where T : BaseModel
    {
        protected string KeyColumn = "Id";
        protected string SqlSelect => "SELECT " + Columns.AddBraces() + " FROM " + TableName;

        protected string SqlSelectWhole => "SELECT * FROM " + TableName;
        protected string SqlInsert => Columns.GenerateInsertQuery(TableName, ExcludedColumns);
        protected string SqlDelete => "DELETE FROM " + TableName + " WHERE Id=@Id;";

        protected string SqlUpdate => Columns.GenerateUpdateQuery(TableName, KeyColumn, "Id,CreationDate");

        protected string SqlCount => "SELECT COUNT(*) FROM " + TableName;
        protected string TableName;
        protected string Columns;
        protected string ExcludedColumns = "PublicId";

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

        public IEnumerable<T> Get(IEnumerable<string> ids)
        {
            if (ids == null || string.IsNullOrEmpty(SqlSelect))
                return default(IEnumerable<T>);
            return Query<T>(SqlSelect + " WHERE Id IN @Ids", new { Ids = ids });
        }
        public IEnumerable<T>? Delete(string id)
        {
            using (connection)
            {
                var resultQuery = connection.Query<T>(SqlDelete, new { Id = new[] { id } });
                return resultQuery;
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
            using (connection)
            {
                var result = connection.Query<T>("UPDATE " + TableName + " SET " + column + "=@value WHERE Id=@id", new { value, Id = id });

            }

            //_connection.globalSqlConnection.Query<T>("UPDATE " + TableName + " SET " + column + "=@value WHERE Id=@id", new { value, Id = id });               
            //Execute("UPDATE " + TableName + " SET " + column + "=@value WHERE Id=@id", new { value, id });
        }

       /* public Post UpdatePostInfo(Post post)
        {
            using (connection)
            {
                //var result = (Post) connection.Query<T>("UPDATE " + TableName + " SET " + "Title = " + post.Title + ", ShortTitle = " + post.ShortTitle + ", Summary = " + post.Summary + ", Content = " + post.Content + "=@value WHERE Id=" + post.Id);
                var result = (Post)connection.Query<T>("UPDATE " + TableName + " SET " + "Title = " + post.Title + "=@value WHERE Id=" + new { Id = new[] { post.Id } });
                return result;
            }
        }*/

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
                                res = connection.Execute(sqlBatch, param);
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
                using (connection)
                {
                    var result = connection.Query<T>(sql, param);
#if DEBUG
#endif
                    return result;
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception(ex.FullMessage());
            }
        }
            
            
        public void Insert(T model, DateTime? creationDate = null)
        {
            model.LastModified = DomainTime.Now();
            model.CreationDate = creationDate.HasValue ? creationDate.Value : DomainTime.Now();
           var sql = SqlInsert;
            Execute(sql, model);
        }
        public void Insert(IEnumerable<T> models, DateTime? creationDate = null)
        {
            foreach (var model in models)
            {
                model.CreationDate = creationDate.HasValue ? creationDate.Value : DomainTime.Now();
                model.LastModified = DomainTime.Now();
            }
            if (SqlInsert.IsNotNullOrEmpty())
                Execute(SqlInsert, models);
        }

    }
}
