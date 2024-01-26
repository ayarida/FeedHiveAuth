namespace FeedHiveAuth.Areas.Social.SocialDailymotion.Models
{
    public class UploadedResponse
    {
        public string id { get; set; }
        public string txt { get; set; }

        public override string ToString()
        {
            return "Video available now at:\n\nhttp://www.dailymotion.com/video/" + id;
        }
    }
}
