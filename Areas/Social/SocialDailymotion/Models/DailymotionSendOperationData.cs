using FeedHiveAuth.Areas.Social.Models;

namespace FeedHiveAuth.Areas.Social.SocialDailymotion.Models
{
    public class DailymotionSendOperationData : SendOperationData
    {
        public string Channel { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }



    }
}
