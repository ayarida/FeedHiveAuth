namespace FeedHiveAuth.Models
{
    public class Channel : BaseModel
    {
        public Channel(string subscriptionId) : base(subscriptionId)
        {
        }
        public Channel() : this(string.Empty)
        {
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Network { get; set; }
        public string Account { get; set; }
        public string NetworkId { get; set; }
        public string Code { get; set; }
        public string NetworkUrl { get; set; }
        public string OriginalName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Credentials { get; set; }
        public string Settings { get; set; }
        public string ParentId { get; set; }
        public int Order { get; set; }
        public string Configs { get; set; }
        public string OrganizationId { get; set; }
    }
}

