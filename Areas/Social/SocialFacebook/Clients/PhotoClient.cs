using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Photo;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Shared;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class PhotoClient : FacebookClient
    {
        public PhotoClient(FacebookConfigs configs, string accessToken) : base(configs, accessToken)
        {

        }

        public IRestResponse<PhotoResponse> Publish(string caption, string url, bool published = true, long? scheduledPublishTime = null)
        {
            var parms = new Dictionary<string, object> {
                { "caption", caption },
                { "url", url },
                { "published", published }
            };

            if (!published && scheduledPublishTime.HasValue)
            {
                parms.Add("scheduled_publish_time", scheduledPublishTime);
            }

            return Post<PhotoResponse>("/me/photos", parms);
        }

        public IRestResponse<BasicResult> Update(string id, string caption)
        {
            var parms = new Dictionary<string, object> {
                { "caption", caption }
            };

            return Post<BasicResult>(id, parms);
        }
    }
}
