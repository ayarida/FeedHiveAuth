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
        public void saveRoleDescription(string roleId, string roleDesc)
        {
            using (connection)
            {
                var result = connection.Query<string>("Update AspNetRoles set Description=@Description WHERE Id=@Id", new { Id = new[] { roleId }, Description = new[] { roleDesc } });

            }
        }
        public string getRoleDescription(string roleId)
        {
            using (connection)
            {
                var result = connection.Query<string>("select Description from AspNetRoles where Id=@Id", new { Id = new[] { roleId }}).FirstOrDefault();
                return result;

            }
        }
        public string getRoleName(string roleId)
        {
            using (connection)
            {
                var result = connection.Query<string>("select Name from AspNetRoles where Id=@Id", new { Id = new[] { roleId } }).FirstOrDefault();
                return result;

            }
        }
        public Role userRole(string userId)
        {
            using (connection)
            {
                var userRoleId = GetUserRole(userId);
                var result = connection.Query<Role>("select * from AspNetRoles where Id=@Id", new { Id = new[] { userRoleId } }).FirstOrDefault();
                return result;
            }
        }

    }
}
