using static System.Net.Mime.MediaTypeNames;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Search
{
    public class SearchResponse : TwitterWebResponse
    {
        public IEnumerable<Tweet.Tweet> Statuses { get; set; }
        public SearchMetadata SearchMetadata { get; set; }
    }

    public class SearchMetadata
    {

    }
}
