namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Tweet
{
    public class Tweet : RootTweet
    {
        public IEnumerable<int> DisplayTextRange { get; set; }
        public bool Truncated { get; set; }
        public ExtendedTweet ExtendedTweet { get; set; }

    }
}
