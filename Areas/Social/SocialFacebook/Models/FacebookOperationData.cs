using FeedHiveAuth.Areas.Social.Models;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models
{
    public class FacebookOperationData : SendOperationData
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string HTMLSource { get; set; }
    }
}
