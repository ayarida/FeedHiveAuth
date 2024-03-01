namespace FeedHiveAuth.Models
{
    public class RolePermission
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Parameters { get; set; }
        public bool Allow { get; set; }
    }
}
