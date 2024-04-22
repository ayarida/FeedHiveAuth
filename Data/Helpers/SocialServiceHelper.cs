using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;

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

        public static void SetChannelExpired(this Channel channel)
        {
            channel.Status = StatusEnum.Expired.Value();
            Instances.Repositories.ChannelRepository.UpdateStatus(channel);
        }

    }
}
