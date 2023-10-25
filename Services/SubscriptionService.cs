using FeedHiveAuth.Models;

namespace FeedHiveAuth.Services
{
    public class SubscriptionService : ISubscriptionContext
    {
        public Subscription GetCurrent()
        {
            return new Subscription();
        }
    }
}
