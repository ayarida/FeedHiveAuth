namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common
{
    public class UserMentionObject
    {
        public long Id { get; set; }
        public string IdStr { get; set; }
        public IEnumerable<int> Indices { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
    }
}
