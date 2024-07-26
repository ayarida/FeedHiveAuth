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

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Handlers
{
    public static class AuthorizationManager
    {
        public static string StartOAuthFlow(string baseCallbackUrl, bool reauthorize)
        {
            var requestToken = GenerateRequestToken(baseCallbackUrl, reauthorize);

            if (string.IsNullOrWhiteSpace(requestToken?.Token))
            {
                return null;
            }

            return "https://api.twitter.com/oauth/authenticate".AddParameter("oauth_token", requestToken.Token);
        }
        
        public  static  TwitterCredentials GenerateRequestToken(string baseCallbackUrl, bool reauthorize)
        {
            string consumerKey = "xlqMf0BnlgrJ99GESlxgJGMeg";
            string consumerSecret = "OUfKiyqJmJlh6EDIS6rDkzUV524qfy20QJErzNi7nyATBuO7Jv";
            string accessToken = "1801170753942896641-5AQZtfUTrDX7IRpXciHwowL0k02EUe";
            string accessTokenSecret = "EF2rJuHcWam6aMjgZVH1f67KytxO41FWIbqQwn7Sj9BC5";

            // Authenticate with Twitter
            var userClient = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
           
            // Fetch user information
            var authenticatedUser = userClient.Users.GetAuthenticatedUserAsync().Result;

            // Get the screen name
            string screenName = authenticatedUser.ScreenName;
            Console.WriteLine($"Authenticated user's screen name: {screenName}");
            //var clientt = new Tweetinvi.TwitterClient("HUZMedlx3tgx675KghAOMuAuY", "syXMXHpCLhT543JdxtPBYZQ3oAdtD9L16UgQ2PKj1sFDNlL3KW", "1801170753942896641-KHVD6DPz3ihnJfpCkIRhxhx9FXMXX6", "K1YBen2dDOC4M2r47tB9DZmjv9tF89LONqvQUaakPPXT9");
            
            var configs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;
            var client = new AuthorizationClient(configs, baseCallbackUrl);
            var result = client.GenerateRequestToken(reauthorize);

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

