namespace FeedHiveAuth.Models
{
    public class RolePermission
    {
        public string Controller { get; set; }
        public string Action { get; set; }
    }

    public class RolePermissionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RolePermission> Permissions { get; set; }
    }
}
