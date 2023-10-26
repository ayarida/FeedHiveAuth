using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class SubscriptionRepository
    {
        public SubscriptionRepository()
        {
        }
       /* public new Subscription Get(string id)
        {
            using (connection)
            {
                var returnedUser = connection.Query<Subscription>("SELECT * FROM AspNetUsers WHERE id IN @ids", new { ids = new[] { id } }).FirstOrDefault();
                return returnedUser;
            }
        }*/

    }
}
