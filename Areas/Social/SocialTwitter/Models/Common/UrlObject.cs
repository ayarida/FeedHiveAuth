namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common
{
    public class UrlObject
    {
        public string DisplayUrl { get; set; }
        public string ExpandedUrl { get; set; }
        public IEnumerable<int> Indices { get; set; }
        public string Url { get; set; }
        public UnwoundUrl Unwound { get; set; }
    }

    public class UnwoundUrl
    {
        public string Url { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
