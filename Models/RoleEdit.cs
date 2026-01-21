using Microsoft.AspNetCore.Identity;

namespace FeedHiveAuth.Models
{
    public class RoleEdit
    {
        public Role Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
        public HashSet<string> PermissionSet { get; set; }
    }
}
