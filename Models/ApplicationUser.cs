using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string ParentId { get; set; }
    public string RoleId { get; set; }
    public bool Status { get; set; }
}
