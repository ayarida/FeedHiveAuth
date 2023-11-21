using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class AuthorizationClient : FacebookClient
    {
        private readonly bool UseFbLogin;
        private readonly string CallbackUrl;

        private const string FacebookApiUrl = "https://graph.facebook.com/v18.0";
        private const string InstagramApiUrl = "https://graph.instagram.com";
        private const string FacebookLoginUrl = "https://www.facebook.com/v18.0/dialog/oauth";
        private const string InstagramLoginUrl = "https://api.instagram.com/oauth/authorize";

        public AuthorizationClient(FacebookConfigs configs, string baseCallbackUrl = null, SocialNetworkTypeEnum network = SocialNetworkTypeEnum.Facebook, bool useFbLogin = true, string nodeUrl = "/oauth") : base(configs, nodeUrl: nodeUrl, url: useFbLogin ? FacebookApiUrl : InstagramApiUrl)
        {
            if (string.IsNullOrWhiteSpace(baseCallbackUrl))
            {
                return;
            }

            UseFbLogin = useFbLogin;
            CallbackUrl = baseCallbackUrl + (baseCallbackUrl.EndsWith("/") ? "" : "/") + $"Authorization/{network}SignIn";
        }

        public string GetLoginUrl(string type, bool reauthorize, string subscriptionCode, string scope)
        {
            var apiUrl = UseFbLogin ? FacebookLoginUrl : InstagramLoginUrl;
            var state = "{type:'" + type + "',subscriptionCode:'" + subscriptionCode + "',reauthorize:" + reauthorize.ToString().ToLower() + "}";
            var redirect_uri = CallbackUrl.AddParameter("state", state).AddParameter("scope", "public_profile,email,pages_list").Decode();
            var redirectUrl = apiUrl.
                AddParameter("client_id", GetAppId()).
                AddParameter("redirect_uri", redirect_uri).Decode();
            return UseFbLogin ?
                redirectUrl.AddParameter("display", "popup").AddParameter("auth_type", reauthorize ? "reauthorize" : "rerequest").Decode()
                : redirectUrl.AddParameter("response_type", "code").Decode();
        }
    }
}
