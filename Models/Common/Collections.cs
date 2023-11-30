using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;

namespace FeedHiveAuth.Models.Common
{
    public static class Collections
    {
        private static readonly Dictionary<string, DateTime> Dates = new Dictionary<string, DateTime>();
        private static SubscriptionRepository _subscriptionService = Instances.Repositories.SubscriptionRepository;
        //private static List<Channel> _channels { get; set; }
        private static IEnumerable<Subscription> _subscriptions { get; set; }

        public static IEnumerable<Subscription> Subscriptions
        {
            get
            {
                if (_subscriptions == null) RefreshSubscriptions();
                return _subscriptions;
            }
        }
        public static IEnumerable<Subscription> RefreshSubscriptions()
        {
            _subscriptions = Instances.Repositories.SubscriptionRepository.GlobalGetAll();
            return _subscriptions;
        }
        public static List<Channel> ChannelsOf(string subscriptionId)
        {
            var _channels = _subscriptionService.GetSubscriptionChannels(subscriptionId);
/*            if (_channels == null) RefreshChannels();
*/
            return _channels;
        }
       /* public static void RefreshChannels()
        {
            _channels = (List<Channel>)Instances.Repositories.ChannelRepository.GlobalGetAll();
        }*/

    }
}
