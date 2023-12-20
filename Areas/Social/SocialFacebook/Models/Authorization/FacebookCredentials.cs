using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Authorization
{
    public class FacebookCredentials : FacebookWebResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
