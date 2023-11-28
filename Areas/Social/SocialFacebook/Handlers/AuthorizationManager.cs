using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Clients;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Handlers
{
    public static class AuthorizationManager
    {
        public static string StartOAuthFlow(string baseCallbackUrl, string type, bool reauthorize, Subscription subscription, SocialNetworkTypeEnum network = SocialNetworkTypeEnum.Facebook)
        {
            var client = GetAuthorizationClient(baseCallbackUrl, type, subscription.Id, network);
            var scope = GetScopes(type, network);
            return client.GetLoginUrl(type, reauthorize, subscription.Code, scope).Decode();
        }



        private static AuthorizationClient GetAuthorizationClient(string baseCallbackUrl, string type, string subscriptionId, SocialNetworkTypeEnum network, string nodeUrl = "/oauth")
        {
            var useFbLogin = network.Equals(SocialNetworkTypeEnum.Facebook) || type.EqualsIgnoreCase(SocialAccountTypeEnum.Business.Key());
            var configs = GetConfigs(network, subscriptionId, useFbLogin);
            return new AuthorizationClient(configs, baseCallbackUrl, network, useFbLogin, nodeUrl);
        }
        private static string GetScopes(string type, SocialNetworkTypeEnum network)
        {
            var accountType = EnumExtension.FromKey<SocialAccountTypeEnum>(type);
            switch (accountType)
            {
                case SocialAccountTypeEnum.Page:
                    return "pages_show_list,pages_read_engagement,pages_manage_posts";
                case SocialAccountTypeEnum.AdManager:
                    return "ads_management,ads_read,business_management,read_audience_network_insights,read_insights";
                case SocialAccountTypeEnum.Profile:
                default:
                    if (network.Equals(SocialNetworkTypeEnum.Instagram))
                        return "user_media,user_profile";
                    return "email,groups_access_member_info,publish_to_groups,user_age_range,user_birthday,user_events,user_gender,user_hometown,user_likes,user_link,user_location,user_photos,user_posts,user_tagged_places,user_videos";
            }
        }
        private static FacebookConfigs GetConfigs(SocialNetworkTypeEnum network, String subscriptionId)
        {
            var configs = SocialServiceHelper.GetConfigs(subscriptionId);
            return network.Equals(SocialNetworkTypeEnum.Facebook) ? configs.FacebookConfigs : null;
        }

        private static FacebookConfigs GetConfigs(SocialNetworkTypeEnum network, string subscriptionId, bool useFbLogin)
        {
            var configs = SocialServiceHelper.GetConfigs(subscriptionId);
            if (useFbLogin)
                return configs.FacebookConfigs;

            switch (network)
            {
                case SocialNetworkTypeEnum.Facebook:
                    return configs.FacebookConfigs;
                default:
                    throw new Exception($"Network Type {network.Key()} not supported");
            }
        }
    }
}
