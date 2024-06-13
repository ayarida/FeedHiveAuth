using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Authorization;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class AuthorizationClient : FacebookClient
    {
        private readonly bool UseFbLogin;
        private readonly string CallbackUrl;
        private const string FacebookGraphApiUrl = "https://graph.facebook.com/v18.0";
        private const string InstagramGraphApiUrl = "https://graph.instagram.com";
        private const string FacebookLoginUrl = "https://www.facebook.com/v18.0/dialog/oauth";
        private const string InstagramLoginUrl = "https://api.instagram.com/oauth/authorize";

        public AuthorizationClient(FacebookConfigs configs, string baseCallbackUrl = null, SocialNetworkTypeEnum network = SocialNetworkTypeEnum.Facebook, bool useFbLogin = true, string nodeUrl = "/oauth") : base(configs, nodeUrl: nodeUrl, url: useFbLogin ? FacebookGraphApiUrl : InstagramGraphApiUrl)
        {
            if (string.IsNullOrWhiteSpace(baseCallbackUrl))
            {
                return;
            }

            UseFbLogin = useFbLogin;
            CallbackUrl = baseCallbackUrl + (baseCallbackUrl.EndsWith("/") ? "" : "/") + $"Social/Authorization/{network}SignIn/";
        }

        public string GetLoginUrl(string type, bool reauthorize, string scope)
        {
            var apiUrl = UseFbLogin ? FacebookLoginUrl : InstagramLoginUrl;
            var state = "{type:'" + type + "',reauthorize:" + reauthorize.ToString().ToLower() + "}";
            var redirectUrl = apiUrl.
                AddParameter("client_id", GetAppId()).
                AddParameter("state",state).
                AddParameter("scope", scope).
                AddParameter("redirect_uri", CallbackUrl).Decode();
            return redirectUrl.AddParameter("response_type", "code").Decode();
        }

        public IRestResponse<FacebookCredentials> ExchangeCode(string code)
        {
            var URL = CallbackUrl;
            var parms = new Dictionary<string, object>
            {
                { "client_id", GetAppId() },
                { "redirect_uri",  CallbackUrl},
                { "client_secret", GetAppSecret() },
                { "code", code }
            };

            if (UseFbLogin)
                parms.Add("grant_type", "authorization_code");

            var getresult = Post<FacebookCredentials>("/access_token", parms);
            return getresult;
        }
        public IRestResponse<FacebookCredentials> ExchangeInstaCode(string code)
        {
            var parms = new Dictionary<string, object>
            {
                { "client_id", GetAppId() },
                { "redirect_uri", CallbackUrl },
                { "client_secret", GetAppSecret() },
                { "code", code },
                {"grant_type", "authorization_code" }
            };

            return Post<FacebookCredentials>("/access_token", parms);
        }
    }
}
