using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account
{
    public class InstagramUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ig_id")]
        public string IgId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("profile_picture_url")]
        public string ProfilePictureUrl { get; set; }
    }
}
