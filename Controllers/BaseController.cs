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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BaseController(UserManager<IdentityUser> userManager, ILogger<T> logger, RoleManager<IdentityRole> roleManager = null)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<IdentityUser> allUsers()
        {
            List<IdentityUser> users = _userManager.Users.ToList();
            return users;
        }
        public List<IdentityRole> allRoles()
        {
            var availableRoles = _roleManager.Roles.ToList();
            return availableRoles;
        }

        public async Task<IdentityUser> getIdentityUser()
        {
            return await _userManager.GetUserAsync(User);
        }
        public async Task<IdentityUser> getUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<IdentityResult> userCreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user,password);
        }
        public async Task<IdentityResult> userUpdateAsync(IdentityUser user)
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
        public async Task<IdentityResult> removeUserFromRoleAsync(IdentityUser user, string oldRole)
        {
            return await _userManager.RemoveFromRoleAsync(user, oldRole);
        }
        public async Task<IdentityResult> addUserToRole(IdentityUser user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }
        public async Task<List<IdentityUser>> getAdminUsers()
        {
            return (List<IdentityUser>)await _userManager.GetUsersInRoleAsync("Admin");

        }
        public async Task<string> identityUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
        public string currUserId()
        {
            return identityUserId().GetAwaiter().GetResult();
        }
        public async Task<bool> isAdminAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            var isAdmin = currUser != null && await _userManager.IsInRoleAsync(currUser, "Admin");
            return isAdmin;
        }
        public bool isAdmin()
        {
            return isAdminAsync().GetAwaiter().GetResult();
        }
        public async Task<bool> isMasterAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            var isMaster = currUser != null && await _userManager.IsInRoleAsync(currUser, "Master");
            return isMaster;
        }
        public bool isMaster()
        {
            return isMasterAsync().GetAwaiter().GetResult();
        }
        public async Task<bool> isEditorAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            var isAdmin = currUser != null && await _userManager.IsInRoleAsync(currUser, "Editor");
            return isAdmin;
        }
        public bool isEditor()
        {
            return isEditorAsync().GetAwaiter().GetResult();
        }
    }
}
