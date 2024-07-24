using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Media;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models;
using FeedHiveAuth.Data.Extensions;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    public class MediaClient : TwitterClient
    {
        public MediaClient(TwitterConfigs configs, TwitterCredentials token) : base(configs, "/media", "https://upload.twitter.com/1.1", token: token)
        {

        }

        public IRestResponse<MediaUploadResponse> InitUpload(long totalBytes, string mediaType, string mediaCategory)
        {
            var parms = new Dictionary<string, object>
            {
                { "command", "INIT" },
                { "total_bytes", totalBytes },
                { "media_type", mediaType }
            };

            if (mediaCategory.IsNotNullOrEmpty())
            {
                parms.Add("media_category", mediaCategory);
            }
            return Post<MediaUploadResponse>("/upload.json", nonAuthParms: parms, multipart: true);
        }

        public IRestResponse<TwitterWebResponse> AppendUpload(ulong id, byte[] chunk, int index)
        {
            var parms = new Dictionary<string, object>
            {
                { "command", "APPEND" },
                { "media_id", id },
                { "media_data", Convert.ToBase64String(chunk) },
                { "segment_index", index }
            };
            return Post<TwitterWebResponse>("/upload.json", nonAuthParms: parms, multipart: true);
        }

        public IRestResponse<UploadStatus> FinalizeUpload(ulong id)
        {
            var parms = new Dictionary<string, object>
            {
                { "command", "FINALIZE" },
                { "media_id", id }
            };
            return Post<UploadStatus>("/upload.json", nonAuthParms: parms, multipart: true);
        }

        public IRestResponse<UploadStatus> UploadStatus(ulong id)
        {
            var parms = new Dictionary<string, object>
            {
                { "command", "STATUS" },
                { "media_id", id }
            };
            return Get<UploadStatus>("/upload.json", parms);
        }
    }
}
