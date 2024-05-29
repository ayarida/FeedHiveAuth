using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FeedHiveAuth.Data.Repositories
{
    public class UserRepository
    {
        public static string _connectionString = DatabaseConnection.GetConnectionStrings();
        public static CustomSqlConnection connection = DatabaseConnection.GetConnection(_connectionString);
        public ApplicationUser GetByUsername(string username)
        {
            using (connection)
            {
                var returnedUser = connection.Query<ApplicationUser>("SELECT * FROM AspNetUsers WHERE Username IN @userName", new { userName = new[] { username } }).FirstOrDefault();
                return returnedUser;
            }
        }
        public ApplicationUser GetById(string id)
        {
            using (connection)
            {
                var returnedUser = connection.Query<ApplicationUser>("SELECT * FROM AspNetUsers WHERE Id IN @id", new { Id = new[] { id } }).FirstOrDefault();
                return returnedUser;
            }
        }
        public IEnumerable<ApplicationUser>? Delete(string id)
        {
            using (connection)
            {
                var resultQuery = connection.Query<ApplicationUser>("DELETE FROM AspNetUsers WHERE Id=@Id;", new { Id = new[] { id } });
                return resultQuery;
            }
        }

        public IEnumerable<ApplicationUser>? AssignParentToUser(string parentId, string id)
        {
            using (connection)
            {
                var resultQuery = connection.Query<ApplicationUser>("UPDATE AspNetUsers SET ParentId=@ParentId WHERE Id=@Id;", new { Id = new[] { id }, ParentId = new[] { parentId } });
                return resultQuery;
            }
        }
        public IEnumerable<ApplicationUser>? GetAll()
        {
            using (connection)
            {
                var resultQuery = connection.Query<ApplicationUser>("SELECT * FROM AspNetUsers").ToList();
                return resultQuery;
            }
        }

        public IEnumerable<string> GetWhosParentId(string adminId)
        {
            using (connection)
            {
                var resultQuery = connection.Query<string>("SELECT Id FROM AspNetUsers WHERE ParentId=@ParentId", new { ParentId = new[] { adminId } }).ToList();
                return resultQuery;
            }
        }

        public string GetNameById(string id)
        {
            using (connection)
            {
                var userName = connection.Query<string>("SELECT UserName FROM AspNetUsers WHERE Id=@Id;", new { Id = new[] { id } }).FirstOrDefault();
                return userName;
            }
        }

    }
}
