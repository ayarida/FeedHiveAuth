using FeedHiveAuth.Areas.Social.Controllers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeedHiveAuth.Controllers
{
    public class BaseController<T> : Controller where T : Controller
    {
     
        protected ChannelRepository _channelService = Instances.Repositories.ChannelRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<T> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BaseController(UserManager<ApplicationUser> userManager, ILogger<T> logger, RoleManager<IdentityRole> roleManager = null)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<ApplicationUser> allUsers()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            return users;
        }
        public List<IdentityRole> allRoles()
        {
            var availableRoles = _roleManager.Roles.ToList();
            return availableRoles;
        }

        public async Task<ApplicationUser> getApplicationUser()
        {
            return await _userManager.GetUserAsync(User);
        }
        public async Task<ApplicationUser> getUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<IdentityResult> userCreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user,password);
        }
        public async Task<IdentityResult> userUpdateAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityRole> getRoleById(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }
        public async Task<IdentityRole> getRoleByName(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }
        public async Task<IdentityResult> removeUserFromRoleAsync(ApplicationUser user, string oldRole)
        {
            return await _userManager.RemoveFromRoleAsync(user, oldRole);
        }
        public async Task<IdentityResult> addUserToRole(ApplicationUser user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }
        public async Task<List<ApplicationUser>> getAdminUsers()
        {
            return (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Admin");

        }
        public async Task<string> ApplicationUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
        public string currUserId()
        {
            return ApplicationUserId().GetAwaiter().GetResult();
        }
        //public async Task<bool> IsInRoleIgnoreCaseAsync(string role)
        //{
        //    var roles = await _userManager.GetRolesAsync(CurrentUser);
        //    return roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
        //}
        private async Task<bool> IsInRoleAsync(string roleName)
        {
            var user = await _userManager.GetUserAsync(User);
            return user != null && await _userManager.IsInRoleAsync(user, roleName);
        }

        public Task<bool> isAdminAsync() => IsInRoleAsync("Admin");
        //public Task<bool> isSuperAdmin() =>  IsInRoleAsync()
        public Task<bool> isMasterAsync() => IsInRoleAsync("Master");
        public Task<bool> isEditorAsync() => IsInRoleAsync("Editor");
        public Task<bool> isSuperAdminAsync() => IsInRoleAsync("SuperAdmin");
        public bool isAdmin()
        {
            return isAdminAsync().GetAwaiter().GetResult();
        }

        public bool isMaster()
        {
            return isMasterAsync().GetAwaiter().GetResult();
        }
      
        public bool isEditor()
        {
            return isEditorAsync().GetAwaiter().GetResult();
        }

        public bool isAdminOrMaster()
        {
            return (isMaster() || isAdmin());
        }
        
        public bool isSuperAdmin()
        {
            return isSuperAdminAsync().GetAwaiter().GetResult();
        }
        
    }
}
