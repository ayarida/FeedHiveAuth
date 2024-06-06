using Microsoft.AspNetCore.Identity;
namespace FeedHiveAuth.Models.ViewModels
{
    public class UsersWithRolesViewModel
    {
        public ApplicationUser user { get; set; }
        public string ParentId { get; set; }
        public string role { get; set; }
        public string Status { get; set; }  

    }
}
