using Microsoft.AspNetCore.Identity;

namespace FeedHiveAuth.Models.ViewModels
{
    public class AddUserViewModel
    {
        public IdentityUser User { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public List<IdentityUser> Users { get; set; }

        public IList<IdentityUser> ParentUsers { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
