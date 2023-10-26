using Dapper;
using FeedHiveAuth.Data.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace FeedHiveAuth.Data
{
    public class CustomSqlConnection : IDisposable
    {
        public readonly IDbConnection _connection;
        
        public CustomSqlConnection(string connectionString)
        {
            //_connection.ConnectionString = DatabaseConnection.GetConnectionStrings();
            //DatabaseConnection.GetConnection(_connectionString);
            _connection = new SqlConnection(connectionString);
        }

       
        public void OpenWithRetry()
        {
            ((SqlConnection)_connection).OpenWithRetry();
        }
        public SqlMapper.GridReader QueryMultiple(string query, object param = null)
        {
            try
            {
                var results = _connection.QueryMultiple(query, param);
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute query: " + query, ex);
                Close();
                throw;
            }
        }
        public IEnumerable<dynamic> Query(string query, object param = null, IDbTransaction transaction = null)
        {
            try
            {
                var results = _connection.Query(query, param, transaction);
                Close();
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute query: " + query, ex);
                Close();
                throw;
            }
        }
        public IEnumerable<T> Query<T>(string query, object param = null, IDbTransaction transaction = null)
        {
            try
            {
                var results = _connection.Query<T>(query, param, transaction);
                Close();
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute query: " + query + "\n" + JsonConvert.SerializeObject(param), ex);
                //Close();
                throw;
            }
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object param = null, IDbTransaction transaction = null)
        {
            try
            {

                var results = await _connection.QueryAsync<T>(query, param, transaction);
                Close();
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute query: " + query + "\n" + JsonConvert.SerializeObject(param), ex);
                Close();
                throw;
            }
        }
        public void Dispose()
        {
           // _connection.Dispose();
        }

        public void Close()
        {
            _connection.Close();
        }
        public ConnectionState State()
        {
            return _connection.State;
        }
    }
}
