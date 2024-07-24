using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Trends;
using FeedHiveAuth.Data.Extensions;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    public class TrendsClient : TwitterClient
    {
        public TrendsClient(TwitterConfigs configs, TwitterCredentials token) : base(configs, "/trends", token: token)
        {

        }

        public IRestResponse<List<TrendsList>> GetByPlace(ulong placeId, string excludedTags = null)
        {
            var parms = new Dictionary<string, object>
            {
                { "id", placeId }
            };

            if (excludedTags.IsNotNullOrEmpty())
            {
                parms.Add("exclude", excludedTags);
            }

            return Get<List<TrendsList>>("/place.json", parms);
        }
    }
}
