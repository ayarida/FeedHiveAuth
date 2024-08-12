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
        public BaseController(UserManager<IdentityUser> userManager, ILogger<T> logger)
        {
            _logger = logger;
            _userManager = userManager;
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
