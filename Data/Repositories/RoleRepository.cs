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
        public RoleRepository()
        {
           /* TableName = Database.Tables.Roles;
            Columns = Database.Columns.Roles;      */     
        }
        /*public new void CreateRole(Role role)
        {
            connection.OpenWithRetry();
            
            string query = "INSERT INTO AspNetRoles(Id,Name,NormalizedName) values ('1234','" + role.Name + "','" + role.NormalizedName + "')";
            ExecuteQuery(query);
        } */
    }
}
