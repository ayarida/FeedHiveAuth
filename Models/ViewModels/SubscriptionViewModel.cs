namespace FeedHiveAuth.Models.ViewModels
{
    public class SubscriptionViewModel
    {
        public ApplicationUser Master { get; set; }

        public List<ApplicationUser> Admins { get; set; }

        public Dictionary<ApplicationUser, List<ApplicationUser>> Editors { get; set; }
    }
}
