using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : Controller
    {

        [Area("Social")]


        public ActionResult TechincalConfigs()
        {
            return View("~/Areas/Social/Views/Channels/Configs/TechincalConfigs.cshtml");
        }
    }
}
