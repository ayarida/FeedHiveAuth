using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class SocialHomeController : Controller
    {
        [PermissionFilter("SocialHome_Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
