using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers;
using FeedHiveAuth.Areas.Social.SocialTelegram.Handlers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using twtAuthorizationManager =FeedHiveAuth.Areas.Social.SocialTwitter.Handlers.AuthorizationManager;
namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class AuthorizationController : Controller
    {
        protected SubscriptionRepository _subscriptionService = Instances.Repositories.SubscriptionRepository;
        protected ChannelRepository _channelService = Instances.Repositories.ChannelRepository;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }
        
        [PermissionFilter("Authorization_OAuthFlow")]

        public async Task<IActionResult> OAuthFlow(string network, string account, bool reauthorize = false, string credentials = null, string id = null)

        {
            //var subscription = await _adminWorkContext.GetCurrentSubscriptionAsync();
            //var currentUser = _userService.GetByUsername("testnew");
            var type = EnumExtension.FromKey<SocialNetworkTypeEnum>(network);
            switch (type)
            {
                case SocialNetworkTypeEnum.Facebook:
                    var jsonRes = FacebookOAuthFlow(account, reauthorize);
                    var obj = jsonRes.TryCast<JObject>();
                    var objResult = obj.ValueFromJson("Value", new JObject());
                    var redirect = objResult.ValueFromJson<string>("redirect", null);
                    //return Redirect(redirect);*/
                    //var redirectUri = ConstructFacebookOAuthUrl(account, subscription, reauthorize);
                    return Ok(new { redirect });
                case SocialNetworkTypeEnum.DailyMotion:
                    return DailymotionSignIn(network, id);
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
                    return TelegramSignIn(id, reauthorize);
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
        public IActionResult TwitterOAuthFlow(string type, string username , bool reauthorize = false)
        {
            var success = false;
            var redirect = "";
            var message = "";
            try
            {
                var baseUrl = SocialConfigs.Construct().TechnicalConfigs.LocalUrl;
                var result = twtAuthorizationManager.StartOAuthFlow(baseUrl, username,false);
                if (result.IsNotNullOrEmpty())
                {
                    success = true;
                    redirect = result;
                }
                else
                {
                    message = "Couldn't start twitter authentication process";
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(this, "Couldn't start twitter authentication process", ex);
                message = "Couldn't start twitter authentication process: " + ex.FullMessage();
            }
            return Json(new { success, redirect, message });
        }
        #region facebook
        [PermissionFilter("Authorization_FacebookOAuthFlow")]
        public IActionResult FacebookOAuthFlow(string type, bool reauthorize = false)
        {
            var success = false;
            var redirect = "";
            var message = "";
            var baseUrl = "";
            try
            {
#if DEBUG 

            baseUrl = SocialConfigs.Construct().TechnicalConfigs.LocalUrl;
#else
            baseUrl = SocialConfigs.Construct().TechnicalConfigs.PublicUrl;
#endif
                var result = AuthorizationManager.StartOAuthFlow(baseUrl, type, reauthorize).Decode();
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
        [PermissionFilter("Authorization_Index")]
        public IActionResult Index()
        {
            return View("/Areas/Social/Views/Shared/Index.cshtml /Views/Shared/Index.cshtml");
        }

        [PermissionFilter("Authorization_FacebookSignIn")]
        //[EnableCors]
        public IActionResult FacebookSignIn(string state, string code)
        {
            try
            {
                //var subscription = "1f59028d-15d0-4bf6-a61b-28f33b895310";
                var baseCallBackUrl = "";
#if DEBUG

                baseCallBackUrl = SocialConfigs.Construct().TechnicalConfigs.LocalUrl;
#else
            baseCallBackUrl = SocialConfigs.Construct().TechnicalConfigs.PublicUrl;
#endif
                var channels = FacebookService.GetChannelsInfo(baseCallBackUrl, code,
                                state, out string msg);

                if (channels.Empty())
                {
                    Debug.WriteLine(msg ?? "Couldn't authorize the selected Facebook account");
                    return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Facebook.Key() });
                }

                var reauthorize = false;
                return SaveChannels(channels, reauthorize, SocialNetworkTypeEnum.Facebook);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't authorize the selected Facebook account: " + ex.FullMessage());
                //return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Facebook.Key() });
                return null;
            }
        }


        [PermissionFilter("Authorization_DailymotionSignIn")]
        public IActionResult DailymotionSignIn(string network, string id)
        {
            var dailymotionConfigs = SocialConfigs.Construct().DailymotionConfigs;
            var jsonDmConfigs = JsonConvert.SerializeObject(dailymotionConfigs);
            ChannelErrorEnum error;
            var channel = _channelService.GetById(id, out error);
            switch (error)
            {
                case ChannelErrorEnum.NOT_FOUND:
                    channel = _channelService.Create(network, out error);
                    channel.Status = StatusEnum.Active.Value();
                    channel.SubscriptionId = "1f59028d-15d0-4bf6-a61b-28f33b895310";
                    channel.Id = GuidExtension.GenerateGuid().ToString();
                    channel.Credentials = jsonDmConfigs;
                    channel.OriginalName = dailymotionConfigs.Application.ChannelName;
                    channel.NetworkId = "baaedba16b73e73577d7cd12e07158bd";
                    _channelService.Insert(channel);
                    break;
                case ChannelErrorEnum.NO_ERROR:
                    return View(channel);
            }
            var allChannels = Collections.Channels();
            return View("~/Areas/Social/Views/Channels/Index.cshtml", allChannels);
        }

        private IActionResult SaveChannels(IEnumerable<Channel> channels, bool reauthorize, SocialNetworkTypeEnum networkTypeEnum)
        {
            if (reauthorize)
            {
                var networkType = networkTypeEnum.Key();
                var existingChannelsIds = Collections.Channels().Where(channel => channel.Network.EqualsIgnoreCase(networkType) && channel.Status.NotIn(new List<int> { StatusEnum.Deleted.Value() })).Select(channel => channel.NetworkId);
                channels = channels.Where(channel => existingChannelsIds.ContainsIgnoreCase(channel.NetworkId));
                if (channels.Empty())
                {
                    _logger.LogError("********************* The reauthorized channel doesn't exist *********************");
                    return RedirectToAction("Create", "Channels", new { area = "social", type = networkType });
                }
            }

            /*TempData["channels"] = channels.ToList();*/
            if (channels.Count() >= 1 && !reauthorize)
                _ = SaveNewChannels(channels);
            return RedirectToAction("Index", "Channels", new { area = "Social" });
        }

        private async Task<IEnumerable<Channel>> SaveNewChannels(IEnumerable<Channel> channels)
        {
            foreach (var channel in channels)
            {
                var oldChannel = Collections.Channels().FirstOrDefault(c =>
                    c.NetworkId.EqualsIgnoreCase(channel.NetworkId) && c.Network.EqualsIgnoreCase(channel.Network));
                if (oldChannel == null)
                {
                    var newChannel = Instances.Repositories.ChannelRepository.AddChannel(channel);
                    channel.Id = newChannel.Id;
                }
                else
                {
                    oldChannel.OriginalName = channel.OriginalName;
                    oldChannel.NetworkUrl = channel.NetworkUrl;
                    oldChannel.Credentials = channel.Credentials;
                    oldChannel.Status = StatusEnum.Active.Value();
                    if (oldChannel.Status.Equals(StatusEnum.Deleted.Value()))
                    {
                        oldChannel.Name = channel.Name;
                        oldChannel.Description = channel.Description;
                        oldChannel.ProfileImageUrl = channel.ProfileImageUrl;
                        oldChannel.CreationDate = channel.CreationDate;
                        oldChannel.Settings = channel.Settings;
                    }

                    Instances.Repositories.ChannelRepository.Update(oldChannel);
                    /* result = oldChannel.Update(oldChannel.OriginalName,
                         _adminWorkContext.getRequestData(Url.Action("Preview", "Channel",
                             new { area = "Social", id = oldChannel.Id })));*/

                    channel.Id = oldChannel.Id;
                }
            }

            return channels;
        }


        [PermissionFilter("Authorization_TelegramSignIn")]
        public IActionResult TelegramSignIn(string username, bool reauthorize = false)
        {
            var success = false;
            var redirect = "";
            var message = "";
            var channels = new List<Channel> { };
            try
            {
                var channel = TelegramService.GetChannelInfo(username);
                if (channel == null)
                {
                    message = "Couldn't authorize the selected Telegram account";
                }
                else
                {
                    var networkType = SocialNetworkTypeEnum.Telegram.Key();
                    var existingChannel = Collections.Channels().FirstOrDefault(oldChannel => oldChannel.Network.EqualsIgnoreCase(networkType) && channel.Status.NotIn(new List<int> { StatusEnum.Deleted.Value() }) && oldChannel.NetworkId.EqualsIgnoreCase(channel.NetworkId));
                    if (reauthorize && existingChannel == null)
                    {
                        Console.WriteLine($"The reauthorized Telegram channel doesn't exist");
                        redirect = Url.Action("Create", "Channel", new { area = "social", type = networkType });
                    }
                    success = true;
                    channels.Add(channel);
                    foreach (var ch in channels)
                    {
                        //ch.SubscriptionId = subscription.Id;
                        ChannelErrorEnum error;
                        var oldChannel = _channelService.GetByNetwork(ch.Network, ch.NetworkId, out error);
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
                _logger.LogError(message);
            }
            return Json(new { success, redirect, message });
        }
    }
}
