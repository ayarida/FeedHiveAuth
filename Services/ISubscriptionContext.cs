using FeedHiveAuth.Models;

namespace FeedHiveAuth.Services
{
    public partial interface ISubscriptionContext
    {
        Subscription GetCurrent();
    }
}
