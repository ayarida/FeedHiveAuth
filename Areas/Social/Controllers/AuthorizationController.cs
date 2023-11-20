using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers; 

namespace FeedHiveAuth.Areas.Social.Controllers
{
    public class AuthorizationController : Controller
    {
        
        protected SubscriptionRepository _subscriptionService = Instances.Repositories.SubscriptionRepository;
        [HttpPost]
        public async Task<IActionResult> OAuthFlow(string network, string account, bool reauthorize = false, string credentials = null, string id = null)
        {
            //var subscription = await _adminWorkContext.GetCurrentSubscriptionAsync();
            //var currentUser = await _adminWorkContext.GetCurrentUserAsync();
            Subscription subscription = _subscriptionService.Get("1f59028d-15d0-4bf6-a61b-28f33b895310");
            var type = EnumExtension.FromKey<SocialNetworkTypeEnum>("Facebook");
            switch (type)
            {
                case SocialNetworkTypeEnum.Facebook:
                    return FacebookOAuthFlow(account, subscription, reauthorize);
                    //        /*case SocialNetworkTypeEnum.Instagram:
                    //            return InstagramOAuthFlow(account, subscription, reauthorize);
                    //        case SocialNetworkTypeEnum.Twitter:
                    //            return TwitterOAuthFlow(account, subscription, reauthorize);
                    //        case SocialNetworkTypeEnum.Youtube:
                    //            return GoogleOAuthFlow(network, subscription, reauthorize);
                    //        case SocialNetworkTypeEnum.Firebase:
                    //            return FirebaseSignIn(credentials, subscription, reauthorize);
                    //        case SocialNetworkTypeEnum.Soundcloud:
                    //            return SoundcloudOAuthFlow(subscription, reauthorize);
                    //        case SocialNetworkTypeEnum.Odnoklassniki:
                    //            return OdnoklassnikiOAuthFlow(subscription, reauthorize);
                    //        case SocialNetworkTypeEnum.Telegram:
                    //            return TelegramSignIn(id, subscription, currentUser, reauthorize);
                    //        case SocialNetworkTypeEnum.Mangomolo:
                    //            var selectedChannel = Collections.ChannelsOf(subscription.Id).FirstOrDefault(channel => channel.NetworkId.EqualsIgnoreCase(id));
                    //            var url = reauthorize ? Url.Action("Edit", "Channel", new { Area = "Social", selectedChannel.Id }) : Url.Action("Edit", "Channel", new { Area = "Social", network = type.Key(), account = SocialAccountTypeEnum.Profile.Key() });
                    //            ViewBag.Reauthorize = reauthorize;
                    //            return Json(new { success = true, redirect = url });*/
                    //        //case SocialNetworkTypeEnum.Shutterstock:
                    //        //    return ShutterstockOAuthFlow(reauthorize);
            }
            return NotFound();
        }
        #region facebook
        public IActionResult FacebookOAuthFlow(string type, Subscription subscription, bool reauthorize = false)
        {

            var success = false;
            var redirect = "";
            var message = "";
            //var fbAuthorizationManager = AuthorizationManager; 
            try
            {

                var result = AuthorizationManager.StartOAuthFlow("https://localhost:7157", type, reauthorize, subscription).Decode();
                //var result = "";
                if (result.IsNotNullOrEmpty())
                {
                    success = true;
                    redirect = result;
                }
                else
                {
                    message = "Couldn't start facebook authentication process";
                }
            }
            catch (Exception ex)
            {
                message = "Couldn't start facebook authentication process: " + ex.FullMessage();
            }
            return Json(new { success, redirect, message });
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }


        public ActionResult FacebookSignIn(string state)
        {
            return GetAdminUrl("");
        }

        private ActionResult GetAdminUrl(string subscriptionCode)
        {
            var redirectUrl = "https://localhost:7157/Home/RedirectSocial";
            return Redirect(redirectUrl);
        }
    }
}
