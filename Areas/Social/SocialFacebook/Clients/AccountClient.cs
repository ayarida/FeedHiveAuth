using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account;
using FeedHiveAuth.Data.Extensions;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Clients
{
    public class AccountClient : FacebookClient
    {
        public AccountClient(FacebookConfigs configs, string accessToken, string nodeUrl = "/me") : base(configs, accessToken, nodeUrl: nodeUrl)
        {

        }
        public IRestResponse<PictureData> GetProfilePic()
        {
            var parms = new Dictionary<string, object>
            {
                { "redirect", "0" },
                { "type", "large" }
            };

            return Get<PictureData>("/picture", parms);
        }

        public IRestResponse<User> GetUser(string fields = null)
        {
            var parms = new Dictionary<string, object>();
            if (fields.IsNotNullOrEmpty())
            {
                parms.Add("fields", fields);
            }
            return Get<User>(parms: parms);
        }

        public IRestResponse<PagesData> GetPages(string fields = null, string after = null)
        {
            var parms = new Dictionary<string, object>();
            if (after.IsNotNullOrEmpty())
            {
                parms.Add("after", after);
            }

            if (fields.IsNotNullOrEmpty())
            {
                parms.Add("fields", fields);
            }
            return Get<PagesData>("/accounts", parms);
        }

        public IRestResponse<GroupsData> GetGroups(string fields = null, string after = null)
        {
            var parms = new Dictionary<string, object>();
            if (after.IsNotNullOrEmpty())
            {
                parms.Add("after", after);
            }

            if (fields.IsNotNullOrEmpty())
            {
                parms.Add("fields", fields);
            }
            return Get<GroupsData>("/groups", parms);
        }

        public IRestResponse<AdAccountsData> GetAdManagers()
        {
            return Get<AdAccountsData>("/adaccounts");
        }

        public IRestResponse<InstagramUser> GetIGUser(string fields = null)
        {
            var parms = new Dictionary<string, object>();
            if (fields.IsNotNullOrEmpty())
            {
                parms.Add("fields", fields);
            }
            return Get<InstagramUser>(parms: parms);
        }
    }
}
