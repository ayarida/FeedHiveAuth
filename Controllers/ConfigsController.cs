using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : Controller
    {
        public SocialConfigs GetSubscriptionSocialConfigs(string subscriptionId)
        {
            return SocialServiceHelper.GetConfigs(subscriptionId);
        }

        //[Area("Social")]
        [HttpGet]
        public ActionResult TechnicalConfigs()
        {
            return View();
        }
    }
}
