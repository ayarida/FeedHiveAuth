using FeedHiveAuth.Areas.Social.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using RestSharp;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    public class AuthorizationClient : TwitterClient
    {
        private readonly string CallbackUrl;

        public AuthorizationClient(TwitterConfigs configs, string baseCallbackUrl = null) : base(configs, "/oauth", "https://api.twitter.com/2")
        {
            if (string.IsNullOrWhiteSpace(baseCallbackUrl))
            {
                return;
            }

            CallbackUrl = baseCallbackUrl + (baseCallbackUrl.EndsWith("/") ? "" : "/") + "Authorization/TwitterSignIn";
        }

        public IRestResponse GenerateRequestToken( bool reauthorize = false)
        {
          var url = "oauth / request_token";
          var parms = new Dictionary<string, object>
            {
                { "oauth_callback", $"{CallbackUrl}&reauthorize={reauthorize.ToString().ToLower()}" }
            };
            return Post("/request_token", parms);
        }

        public IRestResponse GenerateAccessToken(string token, string verifier)
        {
            var parms = new Dictionary<string, object>
            {
                { "oauth_verifier", verifier },
                { "oauth_token", token }
            };
            return Post("/access_token", parms);
        }
    }
}
