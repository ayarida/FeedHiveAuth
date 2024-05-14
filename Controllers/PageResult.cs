using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class PageResult : Controller
    {
        public IActionResult Forbidden()
        {
            return View("~/Views/PageResult/forbidden.cshtml");
        }
    }
}
