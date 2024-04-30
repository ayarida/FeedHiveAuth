using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Feed;
using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Photo
{
    public class PhotoResponse : FeedResponse
    {
        [JsonProperty("post_id")]
        public string PostId { get; set; }
    }
}
