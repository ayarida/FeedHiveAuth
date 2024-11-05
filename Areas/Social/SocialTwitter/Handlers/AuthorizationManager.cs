using FeedHiveAuth.Areas.Social.SocialFacebook.Clients;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Clients;
using AuthorizationClient = FeedHiveAuth.Areas.Social.SocialTwitter.Clients.AuthorizationClient;
using Tweetinvi.Exceptions;
using Tweetinvi;
using TwitterClient = Tweetinvi.TwitterClient;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Requests.Abstractions;
using FeedHiveAuth.Areas.Social.Models;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Handlers
{
    public static class AuthorizationManager
    {
        public static string StartOAuthFlow(string baseCallbackUrl, bool reauthorize,TwitterConfigs configs)
        {
            var requestToken = GenerateRequestToken(baseCallbackUrl, configs, reauthorize );

            if (string.IsNullOrWhiteSpace(requestToken?.Token))
            {
                return null;
            }

            return "https://api.twitter.com/oauth/authenticate".AddParameter("oauth_token", requestToken.Token);
        }
        
        public  static  TwitterCredentials GenerateRequestToken(string baseCallbackUrl, TwitterConfigs configs, bool reauthorize)
        {
            var client = new AuthorizationClient(configs, baseCallbackUrl);
            var result = client.GenerateRequestToken(reauthorize);

            if (!result.IsSuccessful)
            {
                var response = result.Content.FromJson<TwitterWebResponse>();

                return null;
            }

            var values = result.Content.Split('&');
            return new TwitterCredentials
            {
                Token = values[0].Split('=')[1],
                TokenSecret = values[1].Split('=')[1],
                Confirmed = Convert.ToBoolean(values[2].Split('=')[1])
            };
        }

        public static TwitterCredentials GenerateAccessToken(string token, string verifier)
        {
            var configs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;
            var client = new AuthorizationClient(configs);
            var result = client.GenerateAccessToken(token, verifier);

            if (!result.IsSuccessful)
            {
                var response = result.Content.FromJson<TwitterWebResponse>();
               // Logger.Error(typeof(AuthorizationManager), string.Join(",", response.Errors.Select(x => x.Message)));
                return null;
            }

            var values = result.Content.Split('&');
            return new TwitterCredentials
            {
                Token = values[0].Split('=')[1],
                TokenSecret = values[1].Split('=')[1],
                UserId = Convert.ToUInt64(values[2].Split('=')[1]),
                ScreenName = values[3].Split('=')[1],
            };

        }
    }
}

