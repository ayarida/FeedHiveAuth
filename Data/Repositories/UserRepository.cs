using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository() {
            TableName = Database.Tables.Users;
            Columns = Database.Columns.Users;
        }
       
    }
}
