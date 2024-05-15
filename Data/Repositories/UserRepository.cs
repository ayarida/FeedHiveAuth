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

    }
}
