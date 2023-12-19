using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account
{
    public class AdAccountsData : FacebookWebResponse
    {
        [JsonProperty("data")]
        public IEnumerable<AdAccount> Data { get; set; }
    }

    public class AdAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("account_status")]
        public long AccountStatus { get; set; }

        [JsonProperty("business_name")]
        public string BusinessName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }    
}
