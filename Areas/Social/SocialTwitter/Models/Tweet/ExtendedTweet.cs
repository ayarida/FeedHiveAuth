namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Tweet
{
    public class ExtendedTweet
    {
        public string FullText { get; set; }
        public IEnumerable<int> DisplayTextRange { get; set; }
    }
}
