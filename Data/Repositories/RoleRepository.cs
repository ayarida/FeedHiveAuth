using System;
using System.Data.SqlClient;
using System.Xml.Linq;
using Dapper;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FeedHiveAuth.Data.Repositories
{

    public class RoleRepository 
    {
        public static string _connectionString = DatabaseConnection.GetConnectionStrings();
        public static CustomSqlConnection connection = DatabaseConnection.GetConnection(_connectionString);
        public RoleRepository()
        {
            
        }
        public string GetUserRole(string userid)
        {
            using (connection)
            {
                return connection.Query<string>("select RoleId from AspNetUserRoles" + " WHERE UserId=@userid", new { UserId = userid }).FirstOrDefault();

            }
        }
        public IEnumerable<string> GetUserClaims(string roleid)
        {
            using (connection)
            {
                return connection.Query<string>("select ClaimValue from AspNetRoleClaims" + " WHERE RoleId=@roleid", new { RoleId = roleid });
            }
        }
    }
}
