using Newtonsoft.Json;
namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models
{
    public class TwitterWebResponse
    {
        [JsonIgnore]
        public IEnumerable<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
