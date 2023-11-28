using FeedHiveAuth.Areas.Social.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : Controller
    {

        [HttpGet]
        public ActionResult TechnicalConfigs()
        {

            var socialConfigs = SocialConfigs.Construct();
            return View("~/Areas/Social/Views/Channels/Configs/TechnicalConfigs.cshtml", socialConfigs);
        }
    }
}
