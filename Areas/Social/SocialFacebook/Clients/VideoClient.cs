using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Feed;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Shared;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Video;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class VideoClient : FacebookClient
    {
        public VideoClient(FacebookConfigs configs, string accessToken, string url = "https://graph.facebook.com/v4.0") : base(configs, accessToken, url: url)
        {

        }

        public IRestResponse<VideoResponse> StartUpload(long fileSize)
        {
            var parms = new Dictionary<string, object>
            {
                { "upload_phase", "start" },
                { "file_size", fileSize }
            };
            return Post<VideoResponse>("/me/videos", parms, true);
        }

        public IRestResponse<VideoResponse> TransferUpload(string sessionId, int start, byte[] chunk)
        {
            var parms = new Dictionary<string, object>
            {
                { "upload_phase", "transfer" },
                { "start_offset", start },
                { "upload_session_id", sessionId },
                { "video_file_chunk", Convert.ToBase64String(chunk) }
            };
            return Post<VideoResponse>("/me/videos", parms, true);
        }

        public IRestResponse<BasicResult> FinishUpload(string sessionId, string title, string description, bool published = true, long? scheduledPublishTime = null)
        {
            var parms = new Dictionary<string, object>
            {
                { "upload_phase", "finish" },
                { "upload_session_id", sessionId },
                { "title", title ?? "" },
                { "description", description },
                { "published", published }
            };

            if (!published && scheduledPublishTime.HasValue)
            {
                parms.Add("scheduled_publish_time", scheduledPublishTime);
            }

            return Post<BasicResult>("/me/videos", parms, true);
        }

        public IRestResponse<FeedResponse> Publish(string title, string description, string url, bool published = true, long? scheduledPublishTime = null)
        {
            var parms = new Dictionary<string, object> {
                { "title", title ?? "" },
                { "description", description },
                { "file_url", url },
                { "published", published }
            };

            if (!published && scheduledPublishTime.HasValue)
            {
                parms.Add("scheduled_publish_time", scheduledPublishTime);
            }

            return Post<FeedResponse>("/me/videos", parms);
        }

        public IRestResponse<BasicResult> Update(string id, string title, string description)
        {
            var parms = new Dictionary<string, object> {
                { "name", title ?? "" },
                { "description", description }
            };

            return Post<BasicResult>(id, parms);
        }
    }
}
