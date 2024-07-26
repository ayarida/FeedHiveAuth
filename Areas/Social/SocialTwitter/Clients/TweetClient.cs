using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Tweet;
using FeedHiveAuth.Data.Extensions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    public class TweetClient : TwitterClient
    {
        public TweetClient(TwitterConfigs configs, TwitterCredentials token) : base(configs, "", token: token)
        {

        }

        public IRestResponse<Tweet> Tweet(string status, TwitterConfigs configs, TwitterCredentials token, string mediaIds = null)
        {
            OAuthRequest oAclient = OAuthRequest.ForProtectedResource("POST", configs.Application.ConsumerKey, configs.Application.ConsumerSecret, token.Token, token.TokenSecret);
            oAclient.RequestUrl = "https://api.twitter.com/tweets";
            string auth = oAclient.GetAuthorizationHeader();
            var client2 = new RestClient("https://api.twitter.com/tweets");
            client2.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", auth);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", "guest_id=v1%3A164647635225822612");
            dynamic body = new JObject();
            body.text = status;
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = client2.Execute<Tweet>(request);
            return response;
        }
        public IRestResponse<Tweet> TweetOld(string status, string mediaIds = null)
        {
            var body = new
            {
                status
            };
            var parms = new Dictionary<string, object>
            {

                { "text", body }
            };
            if (mediaIds.IsNotNullOrEmpty())
            {
                parms.Add("media_ids", mediaIds);
            }

            return Post<Tweet>("/tweets", parms);
        }

        public IRestResponse<Tweet> Destroy(string id)
        {
            return Post<Tweet>("/destroy/" + id + ".json");
        }

        public IRestResponse<List<Tweet>> GetUserTimeline(long? sinceId = null, long? maxId = null, int count = 200, bool trimUser = false, bool excludeReplies = true, bool includeRetweets = false)
        {
            var parms = new Dictionary<string, object>
            {
                { "count", count },
                { "trim_user", trimUser },
                { "exclude_replies", excludeReplies },
                { "include_rts", includeRetweets }
            };
            if (sinceId.HasValue)
            {
                parms.Add("since_id", sinceId);
            }
            if (maxId.HasValue)
            {
                parms.Add("max_id", maxId);
            }

            return Get<List<Tweet>>("/user_timeline.json", parms);
        }
    }
}
