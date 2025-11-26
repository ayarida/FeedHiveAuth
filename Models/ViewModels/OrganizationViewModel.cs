namespace FeedHiveAuth.Models.ViewModels
{
    public class OrganizationViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Status { get; set; }
        public int OrgAdminsCount { get; set; }
        public int EditorsCount { get; set; }
    }

}
