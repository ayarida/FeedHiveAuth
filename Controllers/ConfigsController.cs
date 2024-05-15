using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : Controller
    {
        [PermissionFilter("Configs_TechnicalConfigs")]
        [HttpGet]
        public ActionResult TechnicalConfigs()
        {
            var socialConfigs = SocialServiceHelper.GetConfigs();
            return View(socialConfigs);
        }
    }
}
