using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Video
{
    public class VideoResponse
    {
        [JsonProperty("upload_session_id")]
        public string UploadSessionId { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("start_offset")]
        public string StartOffset { get; set; }

        [JsonProperty("end_offset")]
        public string EndOffset { get; set; }

        [JsonProperty("skip_upload")]
        public bool SkipUpload { get; set; }

        [JsonProperty("upload_domain")]
        public string UploadDomain { get; set; }

        [JsonProperty("region_hint")]
        public string RegionHint { get; set; }

        [JsonProperty("transcode_bit_rate_bps")]
        public string TranscodeBitRateBps { get; set; }

        [JsonProperty("transcode_dimension")]
        public string TranscodeDimension { get; set; }
    }
}
