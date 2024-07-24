namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Trends
{
    public class TrendsList : TwitterWebResponse
    {
        public DateTime? QueryDate { get; set; }
        public IEnumerable<Trend> Trends { get; set; }
    }

    public class Trend
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string PromotedContent { get; set; }
        public string Query { get; set; }
        public ulong? TweetVolume { get; set; }
    }
}
