using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Data.Extensions;
using RestSharp;
using System.Text;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    public abstract class TwitterClient : RestClientService
    {
        private readonly TwitterConfigs Configs;
        private readonly string OAuthToken;
        private readonly string OAuthTokenSecret;
        private readonly ulong UserID;

        public TwitterClient(TwitterConfigs configs, string node, string url = "https://api.twitter.com/2", TwitterCredentials token = null) : base(url + (string.IsNullOrWhiteSpace(node) || node.StartsWith("/") ? "" : "/") + node)
        {
            Configs = configs ?? new TwitterConfigs();
            if (token != null)
            {
                OAuthToken = token.Token;
                OAuthTokenSecret = token.TokenSecret;
                UserID = token.UserId;
            }
        }

        public IRestResponse Get(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, object> nonAuthParms = null)
        {
            parms = parms ?? new Dictionary<string, object>();
            nonAuthParms = nonAuthParms ?? new Dictionary<string, object>();
            var headers = new Dictionary<string, string> { { "Authorization", GetAuthorizationHeader(Method.GET.ToString(), endpoint, parms) } };
            parms.AddRange(nonAuthParms);
            return Get(endpoint, parms, headers);
        }

        public IRestResponse<T> Get<T>(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, object> nonAuthParms = null) where T : class, new()
        {
            parms = parms ?? new Dictionary<string, object>();
            nonAuthParms = nonAuthParms ?? new Dictionary<string, object>();
            var headers = new Dictionary<string, string> { { "Authorization", GetAuthorizationHeader(Method.GET.ToString(), endpoint, parms) } };
            parms.AddRange(nonAuthParms);
            return Get<T>(endpoint, parms, headers);
        }

        public IRestResponse Post(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, object> nonAuthParms = null, bool multipart = false)
        {
            parms = parms ?? new Dictionary<string, object>();
            nonAuthParms = nonAuthParms ?? new Dictionary<string, object>();
            var headers = new Dictionary<string, string> { { "Authorization", GetAuthorizationHeader(Method.POST.ToString(), endpoint, parms) } };
            parms.AddRange(nonAuthParms);
            return Post(endpoint, parms, headers, multipart);
        }

        public IRestResponse<T> Post<T>(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, object> nonAuthParms = null, bool multipart = false) where T : class, new()
        {
            parms = parms ?? new Dictionary<string, object>();
            nonAuthParms = nonAuthParms ?? new Dictionary<string, object>();
            var headers = new Dictionary<string, string> { { "Authorization", GetAuthorizationHeader(Method.POST.ToString(), endpoint, parms) } };
            parms.AddRange(nonAuthParms);
            parms.Add("Content-Type", "application/json");
            return Post<T>(endpoint, parms, headers, multipart);
        }

        public string GetAuthorizationHeader(string method, string endpoint, Dictionary<string, object> parms)
        {

            var oauthString = "OAuth ";
            parms.Add("oauth_consumer_key", Configs.Application.ConsumerKey);
            parms.Add("oauth_nonce", GenerateNonce());
            //parms.Add("oauth_nonce", "zVBULCKqyI2");
            parms.Add("oauth_signature_method", "HMAC-SHA1");
            parms.Add("oauth_version", "1.0");
            parms.Add("oauth_timestamp", GetTimestamp());

            if (OAuthToken.IsNotNullOrEmpty())
            {
                parms.Add("oauth_token", OAuthToken);
            }

            parms.Add("oauth_signature", GenerateSignature(method, endpoint, parms));

            var encoded = new List<string>();
            foreach (var parm in parms.Where(x => x.Key.ContainsIgnoreCase("oauth_")).OrderBy(x => x.Key))
            {
                encoded.Add(parm.Key.Encode() + "=\"" + parm.Value.ToString().Encode() + "\"");
            }
            return oauthString + string.Join(", ", encoded);
        }

        private string GenerateSignature(string method, string endpoint, Dictionary<string, object> parms)
        {
            var url = client.BaseUrl.OriginalString + endpoint;
            var signingKey = Configs.Application.ConsumerSecret.Encode() + "&" + (OAuthTokenSecret.IsNotNullOrEmpty() ? OAuthTokenSecret.Encode() : "");
            var encoded = new List<string>();
            foreach (var parm in parms.OrderBy(x => x.Key))
            {
                encoded.Add(parm.Key.Encode() + "=" + parm.Value.ToString().Encode());
            }
            var parmString = string.Join("&", encoded);
            var outputString = method.ToUpper() + "&" + url.Encode() + "&" + parmString.Encode();
            return Convert.ToBase64String(outputString.Hmacsh1(signingKey));
        }

        private string GenerateNonce()
        {
            var nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.Ticks.ToString()));
            return new string(nonce.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
        }

        private string GetTimestamp()
        {
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            return Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
        }
    }
}
