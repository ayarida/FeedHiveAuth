namespace FeedHiveAuth.Models.ViewModels
{
    public class OrgDetailsViewModel
    {
        public string OrganizationName { get; set; }
        public List<UserViewModel> OrgAdmins { get; set; }
        public List<UserViewModel> Editors { get; set; }
        public string OrganizationId { get; set; }
        public List<RoleGroupViewModel> Roles { get; set; } = new();

    }

}
