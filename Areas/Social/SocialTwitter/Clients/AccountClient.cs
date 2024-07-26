using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Account;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Clients
{
    internal class AccountClient : TwitterClient
    {
        public AccountClient(TwitterConfigs configs, TwitterCredentials token) : base(configs, "/account", token: token)
        {

        }

        public IRestResponse<List<User>> GetUser()
        {
            return Get<List<User>>("/verify_credentials.json");
        }
    }
}
