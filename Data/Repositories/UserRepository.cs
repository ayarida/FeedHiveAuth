using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository() {
            TableName = Database.Tables.Users;
            Columns = Database.Columns.Users;
        }

        public User GetByUsername(string username)
        {
            var query =
                $"SELECT {Columns.AddBraces()}, " +
                $"FROM {TableName} u " +
                $"WHERE u.Username = {username.EscapeForSql()}";

            return Query<User>(SqlSelect + " WHERE UserName=@username", new { UserName = username }).FirstOrDefault();
        }

        


    }
}
