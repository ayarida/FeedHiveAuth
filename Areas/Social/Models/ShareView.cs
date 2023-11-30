using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class ShareView
    {
        public Post Post { get; set; }
        public MediaItem Media { get; set; }
        public IEnumerable<MediaItem> Medias { get; set; }
        public string Shortlink { get; set; }
        public string Caption { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<Channel> Channels { get; set; }
        public List<SocialNetworkTypeEnum> ActiveSocialNetworks { get; set; }
    }
}
