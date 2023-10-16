using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using FeedHiveAuth.Data.Repositories;

namespace FeedHiveAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly IConfiguration configuration;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            BaseRepository<User> bs = new BaseRepository<User>();
            var ss = bs.Get("301dfa17-d9b0-411a-a742-daaa49c7e0ce");
            //User U = new User()
            //{
            //    FirstName = ss.Username,
            //    LastName = ss.LastName,
            //};

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