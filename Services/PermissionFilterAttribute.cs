using FeedHiveAuth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PermissionFilterAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _permissionName;
    private readonly UserManager<IdentityUser> _userManager;
    public PermissionFilterAttribute(string permissionName)
    {
        _permissionName = permissionName;

    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = new IdentityUser();
        user.UserName = context.HttpContext.User.Identity.Name;
        var currentuser = Instances.Repositories.UserRepository.GetByUsername(user.UserName);
        var userrole = Instances.Repositories.RoleRepository.GetUserRole(currentuser.Id);
        var roleclaims = Instances.Repositories.RoleRepository.GetUserClaims(userrole);
        // Check if the user has the required permission
        if (!CheckUserPermission(context.HttpContext.User, roleclaims.ToList()))
        {
            // If the user does not have permission, return 403 Forbidden
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "PageResult", action = "Forbidden" }));
        }
    }

    private bool CheckUserPermission(System.Security.Claims.ClaimsPrincipal user, List<string> claims)
    {
        // Implement your logic to check user's permissions here
        // For example, you can check against a database, roles, etc.
        // In this example, I'm just checking against a hardcoded permission name
        foreach (var claim in claims)
        {
            if (claim.ToString() == _permissionName)
            {
                return user.HasClaim("Permission", _permissionName);
            }
        }
        return false;
    }
}
