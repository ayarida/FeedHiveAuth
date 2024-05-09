using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Feed;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Shared;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class FeedClient : FacebookClient
    {
        public FeedClient(FacebookConfigs configs, string accessToken) : base(configs, accessToken)
        {


        }
        public IRestResponse<FeedResponse> Publish(string message, string link, bool published = true, string scheduledPublishTime = null, string? pageId=null)
        {
            var parms = new Dictionary<string, object> {
                { "message", message },
                { "link", link },
                { "published", published }
            };

            if (!published && scheduledPublishTime!=null)
            {
                parms.Add("scheduled_publish_time", scheduledPublishTime);
                string endpoint = $"/{pageId}/feed";
                return Post<FeedResponse>(endpoint, parms);
            }

            return Post<FeedResponse>("/me/feed", parms);
        }

        public IRestResponse<BasicResult> Update(string id, string message)
        {
            var parms = new Dictionary<string, object> {
                { "message", message }
            };

            return Post<BasicResult>(id, parms);
        }



    }
}
