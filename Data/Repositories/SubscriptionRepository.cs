using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class SubscriptionRepository
    {
        public static string _connectionString = DatabaseConnection.GetConnectionStrings();
        public static CustomSqlConnection connection = DatabaseConnection.GetConnection(_connectionString);
        
        public SubscriptionRepository() 
        {
            
        }
        public new Subscription Get(string id)
        {
            using (connection)
            {
                var returnedUser = connection.Query<Subscription>("SELECT * FROM Subscription WHERE id IN @ids", new { ids = new[] { id } }).FirstOrDefault();
                return returnedUser;
            }
        }

    }
}
