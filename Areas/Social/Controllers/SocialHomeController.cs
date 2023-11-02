using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class SocialHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
