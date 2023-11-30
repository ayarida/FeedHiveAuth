using FeedHiveAuth.Areas.Social.Models;

namespace FeedHiveAuth.Data.Helpers
{
    public static class SocialServiceHelper
    {
        public static SocialConfigs GetConfigs(string subscriptionId)
        {
            var configs = SocialConfigs.Construct(subscriptionId);
            return configs;
        }
    }
}
