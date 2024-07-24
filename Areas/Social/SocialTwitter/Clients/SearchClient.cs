using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Search;
using FeedHiveAuth.Data.Extensions;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    class SearchClient : TwitterClient
    {
        public SearchClient(TwitterConfigs configs, TwitterCredentials token) : base(configs, "/search", token: token)
        {

        }

        public IRestResponse<SearchResponse> SearchTweets(string from = null, IEnumerable<string> hashTags = null, bool allHashTags = false, int count = 10, string resultType = "recent")
        {
            var parms = new Dictionary<string, object>
            {
                { "count", count },
                { "result_type", resultType }
            };

            var q = "";
            if (from.IsNotNullOrEmpty())
            {
                q = $"from:{from.TrimStart('@')} ";
            }

            if (hashTags.NotEmpty())
            {
                var separator = allHashTags ? " " : " OR ";
                var tags = hashTags.Where(hashTag => hashTag.IsNotNullOrEmpty()).ToList().Select(x => x.StartsWith("#") ? x : $"#{x}");
                q += string.Join(separator, tags);
            }

            parms.Add("q", q.Encode());

            return Get<SearchResponse>("/tweets.json", parms);
        }
    }
}
