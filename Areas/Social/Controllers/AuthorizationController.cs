using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers;
using FeedHiveAuth.Areas.Social.SocialTelegram.Handlers;
using FeedHiveAuth.Models.Common;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    public class AuthorizationController : Controller
    {
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        protected SubscriptionRepository _subscriptionService = Instances.Repositories.SubscriptionRepository;
        protected ChannelRepository _channelService = Instances.Repositories.ChannelRepository;
        [HttpPost]
        public async Task<IActionResult> OAuthFlow(string network, string account, bool reauthorize = false, string credentials = null, string id = null)
        {
            //var subscription = await _adminWorkContext.GetCurrentSubscriptionAsync();
            var currentUser = _userService.GetByUsername("testnew");
            Subscription subscription = _subscriptionService.Get("1f59028d-15d0-4bf6-a61b-28f33b895310");
            var type = EnumExtension.FromKey<SocialNetworkTypeEnum>(network);
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
                case SocialNetworkTypeEnum.Telegram:
                    return TelegramSignIn(id, subscription, currentUser, reauthorize);
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
        public IActionResult TelegramSignIn(string username, Subscription subscription, User currentUser, bool reauthorize = false)
        {
            var success = false;
            var redirect = "";
            var message = "";
            var channels = new List<Channel> { };
            try
            {
                var channel = TelegramService.GetChannelInfo(username, Guid.Parse(subscription.Id));
                if (channel == null)
                {
                    message = "Couldn't authorize the selected Telegram account";
                }
                else
                {
                    var networkType = SocialNetworkTypeEnum.Telegram.Key();
                    var existingChannel = Collections.ChannelsOf(subscription.Id).FirstOrDefault(oldChannel => oldChannel.Network.EqualsIgnoreCase(networkType) && channel.Status.NotIn(new List<int> { StatusEnum.Deleted.Value() }) && oldChannel.NetworkId.EqualsIgnoreCase(channel.NetworkId));
                    if (reauthorize && existingChannel == null)
                    {
                        Console.WriteLine($"The reauthorized Telegram channel doesn't exist");
                        redirect = Url.Action("Create", "Channel", new { area = "social", type = networkType });
                    }
                    success = true;
                    channels.Add(channel);
                    foreach(var ch in channels)
                    {
                        ch.SubscriptionId = subscription.Id;
                        ChannelErrorEnum error;
                        var oldChannel = _channelService.GetByNetwork(ch.SubscriptionId, ch.Network, ch.NetworkId, out error);
                        switch (error)
                        {
                            case ChannelErrorEnum.NOT_FOUND:
                                var newChannel = _channelService.AddChannel(ch);
                                break;
                            case ChannelErrorEnum.NO_ERROR:
                                oldChannel.OriginalName = ch.OriginalName; 
                                oldChannel.NetworkUrl = ch.NetworkUrl;
                                oldChannel.Status = StatusEnum.Active.Value();
                                oldChannel.Credentials = ch.Credentials;
                                _channelService.Update(oldChannel);
                                break;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                message = "Couldn't authorize the selected Telegram account: " + ex.FullMessage();
            }
            return Json(new { success, redirect, message });
        }

        private ActionResult GetAdminUrl(string subscriptionCode)
        {
            var redirectUrl = "https://localhost:7157/Home/RedirectSocial";
            return Redirect(redirectUrl);
        }
    }
}
