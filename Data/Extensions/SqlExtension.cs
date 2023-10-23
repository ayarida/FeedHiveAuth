using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FeedHiveAuth.Data.Extensions;
using Mangox.Data.Extensions;

namespace FeedHiveAuth.Data.Extensions
{
    public static class SqlExtensions
    {
        private static readonly int[] SqlErrorCodesToRetry =
        {
            //-2 Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding.
            //, 20  The instance of SQL Server you attempted to connect to does not support encryption. (PMcE: amazingly, this is transient)
            //, 64 A connection was successfully established with the server, but then an error occurred during the login process.
            //, 233 The client was unable to establish a connection because of an error during connection initialization process before login
            //, 10053 A transport-level error has occurred when receiving results from the server.
            //, 10054 A transport-level error has occurred when sending the request to the server.
            //, 10060 A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible.
            //, 40143 The service has encountered an error processing your request. Please try again.
            //, 40197 The service has encountered an error processing your request. Please try again.
            //, 40501 The service is currently busy. Retry the request after 10 seconds.
            //, 40613 Database '%.*ls' on server '%.*ls' is not currently available. Please retry the connection later.
        };
        public static bool IsRetryable(this SqlException that)
        {
            return that.Errors.Cast<SqlError>().Any(sqlError => SqlErrorCodesToRetry.Contains(sqlError.Number));
        }
        private const int MaxRetries = 3;
        public static void OpenWithRetry(this SqlConnection connection)
        {
            var retries = 0;
            Exception failedToRetryException = null;
            while (connection.State != ConnectionState.Open && retries < MaxRetries)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (SqlException exc)
                {
                    var retryable = exc.IsRetryable();
                    Console.WriteLine("Exception while opening a connection. Reties: " + retries + ". Retryable: " + retryable, exc);
                    if (!retryable)
                        //throw;

                        failedToRetryException = exc;
                    retries += 1;
                    Thread.Sleep(1000); // 1 second
                }
            }
            if (connection.State != ConnectionState.Open)
            {
                Console.WriteLine("Failed to open sql connection after " + MaxRetries + " retries", failedToRetryException);
                throw new Exception("Failed to open sql connection after " + MaxRetries + " retries", failedToRetryException);
            }
        }
        public static async Task<bool> OpenWithRetryAsync(this SqlConnection connection)
        {
            var retries = 0;
            Exception failedToRetryException = null;
            while (connection.State != ConnectionState.Open && retries < MaxRetries)
            {
                try
                {
                    await connection.OpenAsync();

                    //Logger.Info("Connection opened. Reties: " + retries, null, Logger.LoggerType.DbMonitor);
                    return true;
                }
                catch (SqlException exc)
                {
                    var retryable = exc.IsRetryable();
                    if (!retryable)
                        throw;

                    failedToRetryException = exc;
                    retries += 1;
                    Thread.Sleep(1000); // 1 second
                }
            }
            if (connection.State != ConnectionState.Open)
            {
                Console.WriteLine("Failed to open sql connection after " + MaxRetries + " retries", failedToRetryException);
                throw new Exception("Failed to open sql connection after " + MaxRetries + " retries", failedToRetryException);
            }
            return false;
        }
        public static int ExecuteWithRetry(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var retries = 0;

            Exception failedToRetryException = null;
            while (retries < MaxRetries)
            {
                try
                {
                    var result = cnn.Execute(sql, (object)param, transaction, commandTimeout, commandType);
                    return result;
                }
                catch (SqlException exc)
                {
                    var retryable = exc.IsRetryable();
                    Console.WriteLine("Failed to execute sql connection after " + retries + " retries. Retryable: " + retryable + ". SQL:\n" + sql, exc);
                    if (!retryable)
                        throw;

                    failedToRetryException = exc;
                    retries += 1;
                    Thread.Sleep(1000); // 1 second
                }
            }
            Console.WriteLine("Failed to execute sql connection after " + MaxRetries + " retries. SQL:\n" + sql, failedToRetryException);
            return 0;
        }

        public static string EscapeForSql(this Guid? guid)
        {
            return guid.HasValue ? guid.Value.EscapeForSql() : "NULL";
        }
        public static string EscapeForSql(this Guid guid)
        {
            return guid != Guid.Empty ? string.Format("'{0}'", guid) : "NULL";
        }

        public static string EscapeForSql(this DateTime value, bool withTime = false, bool nowIfNull = false)
        {
            if (value == DateTime.MinValue && nowIfNull)
                value = DomainTime.Now();
            if (value == DateTime.MinValue)
                return "NULL";
            return string.Format("'{0}'", value.ToStandardFormat(withTime).CleanSql());
        }
        public static string EscapeForSql(this DateTime? date, bool withTime = false, bool nowIfNull = false)
        {
            if (date.HasValue)
                return date.Value.EscapeForSql(withTime, nowIfNull);
            else
            {
                if (nowIfNull)
                    return DomainTime.Now().EscapeForSql(withTime, true);
                else return "NULL";
            }
        }

        public static string EscapeForSql(this IEnumerable<Guid> guids)
        {
            if (guids.None() || guids.None(x => x.HasValue()))
                return "(" + Guid.Empty.EscapeForSql() + ")";
            return " (" + string.Join(",", guids.Where(x => x.HasValue()).Select(g => g.EscapeForSql())) + ") ";
        }
        public static string EscapeForSql(this IEnumerable<int> array)
        {
            return " (" + string.Join(",", array) + ") ";
        }
        public static string EscapeForSql(this IEnumerable<string> array)
        {
            return " (" + string.Join(",", array.Where(x => x.IsNotNullOrEmpty()).Select(g => g.EscapeForSql())) + ") ";
        }
        public static string EscapeForSql(this string value, bool emptyIsNull = false, bool like = false)
        {
            if (value == null)
                return "NULL";
            if (emptyIsNull && string.IsNullOrEmpty(value))
                return "NULL";
            return string.Format("'{0}'", like ? "%" + value.CleanSql() + "%" : value.CleanSql());
        }
       /* public static string EscapeForSql(this DateTime value, bool withTime = false, bool nowIfNull = false)
        {
            if (value == DateTime.MinValue && nowIfNull)
                value = DomainTime.Now();
            if (value == DateTime.MinValue)
                return "NULL";
            return string.Format("'{0}'", value.ToStandardFormat(withTime).CleanSql());
        }*/
       /* public static string EscapeForSql(this DateTime? date, bool withTime = false, bool nowIfNull = false)
        {
            if (date.HasValue)
                return date.Value.EscapeForSql(withTime, nowIfNull);
            else
            {
                if (nowIfNull)
                    return DomainTime.Now().EscapeForSql(withTime, true);
                else return "NULL";
            }
        }*/
        public static string BooleanToBit(this bool value)
        {
            return value ? "1" : "0";
        }

        public static StringBuilder And(this StringBuilder builder, string with)
        {
            builder.Append(" and ").Append(with);
            return builder;
        }

        public static StringBuilder Or(this StringBuilder builder, string with)
        {
            builder.Append(" or ").Append(with);
            return builder;

        }
        public static string And(this string builder, string with)
        {
            return builder + " and " + with;
        }
        //used as href example #id
        public static string HrefId(this string str)
        {
            return str?.Replace(" ", "_");
        }
        public static string Or(this string builder, string with)
        {
            return builder + " or " + with;
        }
        //public static string CleanSql(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return string.Empty;
        //    return input.Trim().Replace("'", "''");
        //}

        public static string AddPrefix(this string columns, string prefix, bool removeIdentity = false, string excludedColumns = null)
        {
            var columnsArr = columns.Replace(" ", "").Split(',');
            if (removeIdentity)
                columnsArr = columnsArr.Where(x => excludedColumns == null || !excludedColumns.Split(',').Contains(x)).ToArray();
            return string.Join(", ", columnsArr.Select(col => prefix + col));
        }
        public static string AddBraces(this string columns, string excludedColumns = null)
        {
            var columnsArr = columns.Replace(" ", "").Split(',').Where(x => excludedColumns == null || !excludedColumns.Split(',').Contains(x));
            return string.Join(", ", columnsArr.Select(col => string.Format("[{0}]", col)));
        }
        public static string GenerateUpdateQuery(this string columns, string table, string key, string excludedColumns = "Id,CreationDate")
        {
            var columnsArr = columns.Replace(" ", "").Split(',').Where(x => !excludedColumns.Split(',').Contains(x));
            bool notSubscription = false;// removed by zahraa. columns.Contains("SubscriptionId");
            return string.Format("UPDATE {0} SET {1} WHERE {2}=@{2}" +
                 (notSubscription ? " AND SubscriptionId=@SubscriptionId;" : ";"), table, string.Join(", ", columnsArr.Select(col => string.Format("[{0}]=@{0}", col))), key);
        }
        public static string GenerateInsertQuery(this string columns, string table, string excludedColumns = "PublicId")
        {
            return string.Format("INSERT INTO {0} ({1}) VALUES({2});", table, columns.AddBraces(excludedColumns), columns.AddPrefix("@", true, excludedColumns));
        }

        public static string GetIdColumn(this string id)
        {
            Guid guid;
            int publicid;
            if (Guid.TryParse(id, out guid))
            {
                return "Id";
            }
            if (int.TryParse(id, out publicid))
            {
                return "PublicId";
            }
            return "";
        }
        public static string GetIdColumn(this IEnumerable<string> ids)
        {
            var id = ids.FirstOrDefault();
            if (id == null) return "";

            Guid guid;
            int publicid;

            if (Guid.TryParse(id, out guid))
            {
                return "Id";
            }
            if (int.TryParse(id, out publicid))
            {
                return "PublicId";
            }
            return "";
        }

        


    }
}
