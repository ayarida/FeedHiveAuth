namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common
{
    public class MediaObject
    {
        public string DisplayUrl { get; set; }
        public string ExpandedUrl { get; set; }
        public long Id { get; set; }
        public string IdStr { get; set; }
        public IEnumerable<int> Indices { get; set; }
        public string MediaUrl { get; set; }
        public string MediaUrlHttps { get; set; }
        public MediaSizes Sizes { get; set; }
        public long SourceStatusId { get; set; }
        public string SourceStatusIdStr { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }

    public class VideoInfo
    {
        public IEnumerable<int> AspectRatio { get; set; }
        public long DurationMillis { get; set; }
        public IEnumerable<Variant> Variants { get; set; }
    }

    public class Variant
    {
        public long? Bitrate { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }
    }

    public class MediaInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Embeddable { get; set; }
        public bool Monetizable { get; set; }
    }
}
