using Microsoft.AspNetCore.Identity;

namespace FeedHiveAuth.Models
{
    public class UsersViewModel
    {
        public IdentityUser user { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
