using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using FeedHiveAuth.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FeedHiveAuth.Data;

namespace FeedHiveAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SignInManager<IdentityUser> SignInManager;
        public UserManager<IdentityUser> UserManager;
        protected PostRepository _postRepository = Instances.Repositories.PostRepository;

        //private readonly IConfiguration configuration;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /*     public IActionResult Index()
             {

                 List<Post> posts = _postRepository.GetPosts();
                 BaseRepository<User> bs = new BaseRepository<User>();
                 //var x = SignInManager.IsSignedIn(User);
                 //var ss = bs.Get("301dfa17-d9b0-411a-a742-daaa49c7e0ce");
                 //User U = new User()
                 //{
                 //    FirstName = ss.Username,
                 //    LastName = ss.LastName,
                 //};

                 return View(posts);
             }*/

        [Authorize]
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}