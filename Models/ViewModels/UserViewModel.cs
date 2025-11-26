namespace FeedHiveAuth.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ParentId { get; set; }  // Needed to group editors under orgadmin
    }

}
