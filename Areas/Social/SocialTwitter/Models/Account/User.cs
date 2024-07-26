using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common;
namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Account
{
    public class User : TwitterWebResponse
    {
        public ulong Id { get; set; }
        public string IdStr { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string Location { get; set; }
        public Derived Derived { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool Protected { get; set; }
        public bool Verified { get; set; }
        public long FollowersCount { get; set; }
        public long FriendsCount { get; set; }
        public long ListedCount { get; set; }
        public long FavouritesCount { get; set; }
        public long StatusesCount { get; set; }
        public string CreatedAt { get; set; }
        public string ProfileBannerUrl { get; set; }
        public string ProfileImageUrlHttps { get; set; }
        public bool DefaultProfile { get; set; }
        public bool DefaultProfileImage { get; set; }
        public IEnumerable<string> WithheldInCountries { get; set; }
        public string WithheldScope { get; set; }
    }

    public class Derived
    {
        public IEnumerable<Location> Locations { get; set; }
    }
}

