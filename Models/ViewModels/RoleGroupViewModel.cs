namespace FeedHiveAuth.Models.ViewModels
{
    public class RoleGroupViewModel
    {
        public string RoleName { get; set; }
        public List<UserViewModel> Users { get; set; } = new();
    }
}
