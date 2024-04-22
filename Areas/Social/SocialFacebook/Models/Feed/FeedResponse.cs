using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Feed
{
    public class FeedResponse : FacebookWebResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
