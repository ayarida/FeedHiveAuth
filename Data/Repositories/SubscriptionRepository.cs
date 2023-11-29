using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Telegram.Bot.Types;
using static FeedHiveAuth.Data.Database;

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
        public new List<Subscription> GlobalGetAll(string o = "")
        {
            var query =
                $"SELECT * " +
                $"FROM {Database.Tables.Subscription} ";

            using (connection)
            {
                var subscriptions = connection.Query<Subscription>(query).ToList();
                return subscriptions;
                
            }
        }

        public List<Channel> GetSubscriptionChannels(string id)
        {
            /*var query = $"SELECT * " +
                $"FROM {Database.Tables.Channel} WHERE SubscriptionId=";*/
            using (connection)
            {
                var subsChannels = connection.Query<Channel>("SELECT * FROM Channel WHERE SubscriptionId IN @ids", new { ids = new[] { id } }).ToList();
                return subsChannels;
            }
        }

        public static void SetUpConfigs()
        {
            
        }

    }
}
