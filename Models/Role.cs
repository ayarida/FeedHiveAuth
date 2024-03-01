namespace FeedHiveAuth.Models
{
    public class Role : BaseModel
    {
        public Role(string subscriptionId) : base(subscriptionId) { }
        public Role() : base(string.Empty) { }

        public string? Name { get; set; }
        public string? NormalizedName { get; set; }

        public List<RolePermission> Permissions { get; set; }
    }
}
