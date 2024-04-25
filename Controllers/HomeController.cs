using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FeedHiveAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SignInManager<IdentityUser> SignInManager;
        public UserManager<IdentityUser> UserManager;

        protected PostRepository _postRepository = Instances.Repositories.PostRepository;
        protected UserRepository _userRepository = Instances.Repositories.UserRepository;
        protected MediaItemRepository _mediaRepository = Instances.Repositories.MediaItemRepository;
        //private readonly IConfiguration configuration;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            var posts = _postRepository.GetPosts();
            foreach (var post in posts)
            {
                post.PostMediaItems = _mediaRepository.GetMediasByPostId(post.Id);
            }
            var userPostsCount = _postRepository.GetCurrentUserPosts(GetCurrentUser().Id).Count();
            UserDataViewModel uvm = new UserDataViewModel
            {
                allPosts = posts,
                userPostsCount = userPostsCount
            };
            return View(uvm);
        }
        public User GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userRepository.Get(userId);
            return currentUser;
        }
        public IActionResult Welcome()
        {
            return View();
        }
    }
}