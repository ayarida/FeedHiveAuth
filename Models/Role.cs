using Microsoft.AspNetCore.Identity;

namespace FeedHiveAuth.Models
{
    public class Role : IdentityRole
    {
        public Role(string subscriptionId) : base(subscriptionId) { }
        public Role() : base(string.Empty) { }
        public string Description { get; set; }
        public List<RolePermission> Permissions { get; set; }
    }
}
