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

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Handlers
{
    public static class AuthorizationManager
    {
        public static string StartOAuthFlow(string baseCallbackUrl, string username, bool reauthorize)
        {
            var requestToken = GenerateRequestToken(baseCallbackUrl, username, reauthorize);

            if (string.IsNullOrWhiteSpace(requestToken?.Token))
            {
                return null;
            }

            return "https://api.x.com/oauth/authenticate".AddParameter("oauth_token", requestToken.Token);
        }

        public static  TwitterCredentials GenerateRequestToken(string baseCallbackUrl, string username, bool reauthorize)
        {
            //var clientt = new Tweetinvi.TwitterClient("HUZMedlx3tgx675KghAOMuAuY", "syXMXHpCLhT543JdxtPBYZQ3oAdtD9L16UgQ2PKj1sFDNlL3KW", "1801170753942896641-KHVD6DPz3ihnJfpCkIRhxhx9FXMXX6", "K1YBen2dDOC4M2r47tB9DZmjv9tF89LONqvQUaakPPXT9");
           
            var configs = SocialServiceHelper.GetConfigs().TwitterConfigs;
            var client = new AuthorizationClient(configs, baseCallbackUrl);
            var result = client.GenerateRequestToken(username, reauthorize);

            if (!result.IsSuccessful)
            {
                var response = result.Content.FromJson<TwitterWebResponse>();
                //Logger.Error(typeof(AuthorizationManager), string.Join(",", response.Errors.Select(x => x.Message)));
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

        public static TwitterCredentials GenerateAccessToken(string token, string verifier, Guid subscriptionId)
        {
            var configs = SocialServiceHelper.GetConfigs().TwitterConfigs;
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

