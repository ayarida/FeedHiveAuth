using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Shared;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account
{
    public class GroupsData : FacebookWebResponse
    {
        [JsonProperty("data")]
        public IEnumerable<Group> Data { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("summary")]
        public Summary Summary { get; set; }
    }

    public class Group
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string About { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
