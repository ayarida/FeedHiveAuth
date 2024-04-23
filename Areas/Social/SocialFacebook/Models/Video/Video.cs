using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Feed;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Video
{
    public class Video : FeedResponse
    {
        public VideoStatusObj Status { get; set; }

    }
    public class VideoStatusObj
    {
        public uint ProcessingProgress { get; set; }
        public string VideoStatus { get; set; }
    }
}
