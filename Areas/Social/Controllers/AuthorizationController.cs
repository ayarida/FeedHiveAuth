using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    public class AuthorizationController : Controller
    {
        public async Task<IActionResult> OAuthFlow(string network, string account, bool reauthorize = false, string credentials = null, string id = null)
        {
            //var subscription = await _adminWorkContext.GetCurrentSubscriptionAsync();
            //var currentUser = await _adminWorkContext.GetCurrentUserAsync();
            Subscription subscription = new Subscription();
            var type = EnumExtension.FromKey<SocialNetworkTypeEnum>(network);
            //switch (type)
            //{
            //    case SocialNetworkTypeEnum.Facebook:
            //        return FacebookOAuthFlow(account, subscription, reauthorize);
            //    /*case SocialNetworkTypeEnum.Instagram:
            //        return InstagramOAuthFlow(account, subscription, reauthorize);
            //    case SocialNetworkTypeEnum.Twitter:
            //        return TwitterOAuthFlow(account, subscription, reauthorize);
            //    case SocialNetworkTypeEnum.Youtube:
            //        return GoogleOAuthFlow(network, subscription, reauthorize);
            //    case SocialNetworkTypeEnum.Firebase:
            //        return FirebaseSignIn(credentials, subscription, reauthorize);
            //    case SocialNetworkTypeEnum.Soundcloud:
            //        return SoundcloudOAuthFlow(subscription, reauthorize);
            //    case SocialNetworkTypeEnum.Odnoklassniki:
            //        return OdnoklassnikiOAuthFlow(subscription, reauthorize);
            //    case SocialNetworkTypeEnum.Telegram:
            //        return TelegramSignIn(id, subscription, currentUser, reauthorize);
            //    case SocialNetworkTypeEnum.Mangomolo:
            //        var selectedChannel = Collections.ChannelsOf(subscription.Id).FirstOrDefault(channel => channel.NetworkId.EqualsIgnoreCase(id));
            //        var url = reauthorize ? Url.Action("Edit", "Channel", new { Area = "Social", selectedChannel.Id }) : Url.Action("Edit", "Channel", new { Area = "Social", network = type.Key(), account = SocialAccountTypeEnum.Profile.Key() });
            //        ViewBag.Reauthorize = reauthorize;
            //        return Json(new { success = true, redirect = url });*/
            //        //case SocialNetworkTypeEnum.Shutterstock:
            //        //    return ShutterstockOAuthFlow(reauthorize);
            //}
            return NotFound();
        }
        #region facebook
        //public IActionResult FacebookOAuthFlow(string type, Subscription subscription, bool reauthorize = false)
        //{

        //    var success = false;
        //    var redirect = "";
        //    var message = "";
        //    try
        //    {
        //        var result = fbAuthorizationManager.StartOAuthFlow(ApplicationEnum.Social.ServiceUrl(subscription.Id), type, reauthorize, subscription).Decode();
        //        if (result.IsNotNullOrEmpty())
        //        {
        //            success = true;
        //            redirect = result;
        //        }
        //        else
        //        {
        //            message = "Couldn't start facebook authentication process";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Couldn't start facebook authentication process: " + ex.FullMessage();
        //    }
        //    return Json(new { success, redirect, message });
        //}
        #endregion
        public IActionResult Index()
        {
            return View();
        }
    }
}
