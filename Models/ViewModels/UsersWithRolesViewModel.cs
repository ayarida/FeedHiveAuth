using Microsoft.AspNetCore.Identity;
namespace FeedHiveAuth.Models.ViewModels
{
    public class UsersWithRolesViewModel
    {
        public IdentityUser user { get; set; }
        public string ParentId { get; set; }
        public string role { get; set; }

    }
}
