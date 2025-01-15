using Microsoft.AspNetCore.Identity;

namespace FeedHiveAuth.Models.ViewModels
{
    public class AddUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public List<ApplicationUser> Users { get; set; }

        public IList<ApplicationUser> ParentUsers { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
