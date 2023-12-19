using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Shared;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account
{
    public class PagesData : FacebookWebResponse
    {
        [JsonProperty("data")]
        public IEnumerable<Page> Data { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("summary")]
        public Summary Summary { get; set; }
    }

    public class Page
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("perms")]
        public IEnumerable<string> Perms { get; set; }
    }

    public class Summary
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }
}
