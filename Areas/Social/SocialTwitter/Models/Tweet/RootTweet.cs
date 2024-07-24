using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Account;
namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Tweet
{
    public class RootTweet : TwitterWebResponse
    {
        public string CreatedAt { get; set; }
        public long Id { get; set; }
        public string IdStr { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
        public Place Place { get; set; }
        public Entities Entities { get; set; }
        public ExtendedEntities ExtendedEntities { get; set; }
    }

    public class Entities
    {
        public IEnumerable<HashTagObject> HashTags { get; set; }
        public IEnumerable<MediaObject> Media { get; set; }
        public IEnumerable<UrlObject> Urls { get; set; }
        public IEnumerable<UserMentionObject> UserMentions { get; set; }
        public IEnumerable<SymbolObject> Symbols { get; set; }
        public IEnumerable<PollObject> Polls { get; set; }
    }

    public class ExtendedEntities
    {
        public IEnumerable<MediaObject> Media { get; set; }
    }
}
