namespace FeedHiveAuth.Areas.Social.SocialDailymotion.Models
{
    public class OAuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
