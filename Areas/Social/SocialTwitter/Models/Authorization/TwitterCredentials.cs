using Newtonsoft.Json;
namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization
{
    public class TwitterCredentials : TwitterWebResponse
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public ulong UserId { get; set; }
        public string ScreenName { get; set; }

        [JsonIgnore]
        public bool Confirmed { get; set; }
    }
}
