using FeedHiveAuth.Areas.Social.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    public class AuthorizationClient : TwitterClient
    {
        private readonly string CallbackUrl;

        public AuthorizationClient(TwitterConfigs configs, string baseCallbackUrl = null) : base(configs, "/oauth", "https://api.x.com")
        {
            if (string.IsNullOrWhiteSpace(baseCallbackUrl))
            {
                return;
            }

            CallbackUrl = baseCallbackUrl + (baseCallbackUrl.EndsWith("/") ? "" : "/") + "Authorization/TwitterSignIn";
        }

        public IRestResponse GenerateRequestToken(string Username, bool reauthorize = false)
        {
            var parms = new Dictionary<string, object>
            {
                { "oauth_callback", $"{CallbackUrl}?subscriptionCode={Username}&reauthorize={reauthorize.ToString().ToLower()}" }
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
