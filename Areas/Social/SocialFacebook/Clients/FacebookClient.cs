using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public abstract class FacebookClient : RestClientService
    {
        private readonly FacebookConfigs Configs;
        private readonly string AccessToken;

        public FacebookClient(FacebookConfigs configs, string accessToken = null, string nodeUrl = "", string url = "https://graph.facebook.com/v18.0") : base(url + (string.IsNullOrWhiteSpace(nodeUrl) || nodeUrl.StartsWith("/") ? "" : "/") + nodeUrl)
        {
            Configs = configs ?? new FacebookConfigs();
            AccessToken = accessToken;
        }
        public string GetAppId()
        {
            return Configs?.Application?.AppId;
        }

        public string GetAppSecret()
        {
            return Configs?.Application?.AppSecret;
        }

        public IRestResponse<T> Get<T>(string endpoint = null, Dictionary<string, object> parms = null) where T : class, new()
        {
            parms = parms ?? new Dictionary<string, object>();
            if (AccessToken.IsNotNullOrEmpty())
            {
                parms.Add("access_token", AccessToken);
            }
            var request = base.Get<T>(endpoint, parms);
            return request;
        }

        public IRestResponse<T> Post<T>(string endpoint = null, Dictionary<string, object> parms = null, bool multipart = false) where T : class, new()
        {
            parms = parms ?? new Dictionary<string, object>();
            if (AccessToken.IsNotNullOrEmpty())
            {
                parms.Add("access_token", AccessToken);
            }
            return base.Post<T>(endpoint, parms, multipart: multipart);
        }

        public IRestResponse<T> Delete<T>(string endpoint, Dictionary<string, object> parms = null) where T : class, new()
        {
            parms = parms ?? new Dictionary<string, object>();
            if (AccessToken.IsNotNullOrEmpty())
            {
                parms.Add("access_token", AccessToken);
            }
            return base.Delete<T>(endpoint, parms);
        }
    }
}
