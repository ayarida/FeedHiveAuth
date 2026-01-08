namespace FeedHiveAuth.Models.ViewModels
{
    public class OrganizationViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Status { get; set; }
        public int OrgAdminsCount { get; set; }
        public int EditorsCount { get; set; }
        public string CreatedOnFormatted =>
        CreationDate.HasValue && CreationDate != DateTime.MinValue
            ? CreationDate.Value.ToString("dd MMM yyyy")
            : "-";
    }

}
