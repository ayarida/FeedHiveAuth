namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Media
{
    public class MediaUploadResponse : TwitterWebResponse
    {
        public ulong MediaId { get; set; }
        public string MediaIdString { get; set; }
        public long Size { get; set; }
    }
}
