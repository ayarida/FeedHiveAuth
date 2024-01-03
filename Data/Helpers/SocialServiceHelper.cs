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
        public static SocialConfigs GetConfigs()
        {
            var configs = SocialConfigs.Construct();
            return configs;
        }
    }
}
