using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Authorization;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class AuthorizationClient : FacebookClient
    {
        private readonly bool UseFbLogin;
        private readonly string CallbackUrl;

        private const string FacebookGraphApiUrl = "https://graph.facebook.com/v12.0";
        private const string InstagramGraphApiUrl = "https://graph.instagram.com";
        private const string FacebookLoginUrl = "https://www.facebook.com/v12.0/dialog/oauth";
        private const string InstagramLoginUrl = "https://api.instagram.com/oauth/authorize";

        public AuthorizationClient(FacebookConfigs configs, string baseCallbackUrl = null, SocialNetworkTypeEnum network = SocialNetworkTypeEnum.Facebook, bool useFbLogin = true, string nodeUrl = "/oauth") : base(configs, nodeUrl: nodeUrl, url: useFbLogin ? FacebookGraphApiUrl : InstagramGraphApiUrl)
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
            var state = "{type:'" + type + "',reauthorize:" + reauthorize.ToString().ToLower() + "}";
            var redirect_uri = CallbackUrl.AddParameter("state", state).AddParameter("scope", "pages_manage_metadata,pages_read_engagement,pages_read_user_content,pages_manage_posts,pages_manage_engagement,pages_show_list").Decode();
            var redirectUrl = apiUrl.
                AddParameter("client_id", GetAppId()).
                AddParameter("redirect_uri", redirect_uri).Decode();
            return redirectUrl.AddParameter("response_type", "code").Decode();
        }

        public IRestResponse<FacebookCredentials> ExchangeCode(string code)
        {
            var URL = CallbackUrl;
            URL += "?state={type:'page',reauthorize:false}";
            //URL = "https%3A%2F%2Flocalhost%3A7055%2FAuthorization%2FFacebookSignIn%3Fstate%3D%257Btype%253A%2527page%2527%252CsubscriptionCode%253A%2527mangopulse%2527%252Creauthorize%253Afalse%257D";
            var parms = new Dictionary<string, object>
            {
                { "client_id", GetAppId() },
                { "redirect_uri",  URL},
                { "client_secret", GetAppSecret() },
                { "code", code }
            };

            if (UseFbLogin)
                parms.Add("grant_type", "authorization_code");

            var getresult = Get<FacebookCredentials>("/access_token", parms);
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
