using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class PostsController : Controller
    {
        public ActionResult Create()
        {
            return View("~/Views/Posts/Create.cshtml");
        }
    }
}
