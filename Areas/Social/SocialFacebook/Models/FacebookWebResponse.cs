using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models
{
    public class FacebookWebResponse
    {
        [JsonIgnore]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("error_subcode")]
        public int ErrorSubcode { get; set; }

        [JsonProperty("is_transient")]
        public bool IsTransient { get; set; }

        [JsonProperty("error_user_title")]
        public string ErrorUserTitle { get; set; }

        [JsonProperty("error_user_msg")]
        public string ErrorUserMsg { get; set; }

        [JsonProperty("fbtrace_id")]
        public string FbtraceId { get; set; }
    }
}
