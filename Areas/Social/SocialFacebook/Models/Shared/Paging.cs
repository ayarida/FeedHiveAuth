using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Shared
{
    public class Paging
    {
        [JsonProperty("cursors")]
        public Cursors Cursors { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }

    public class Cursors
    {
        [JsonProperty("after")]
        public string After { get; set; }

        [JsonProperty("before")]
        public string Before { get; set; }
    }
}
