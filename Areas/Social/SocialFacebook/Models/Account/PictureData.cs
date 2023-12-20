using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account
{
    public class PictureData : FacebookWebResponse
    {
        [JsonProperty("data")]
        public Picture Data { get; set; }
    }

    public class Picture
    {
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }
    }
}
