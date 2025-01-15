using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers;
using FeedHiveAuth.Areas.Social.SocialTelegram.Handlers;
using FeedHiveAuth.Areas.Social.SocialTwitter.Clients;
using FeedHiveAuth.Areas.Social.SocialTwitter.Handlers;
using FeedHiveAuth.Controllers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Channels;
using Channel = FeedHiveAuth.Models.Channel;
using twtAuthorizationManager =FeedHiveAuth.Areas.Social.SocialTwitter.Handlers.AuthorizationManager;
using fbAuthorizationManager = FeedHiveAuth.Areas.Social.SocialFacebook.Handlers.AuthorizationManager;
namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class AuthorizationController : BaseController<AuthorizationController>
    {
        protected SubscriptionRepository _subscriptionService = Instances.Repositories.SubscriptionRepository;
        protected ChannelRepository _channelService = Instances.Repositories.ChannelRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(UserManager<ApplicationUser> userManager, ILogger<AuthorizationController> logger) : base(userManager,logger)
        {
            
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
                case SocialNetworkTypeEnum.Twitter:
                    return TwitterOAuthFlow(account, reauthorize);
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
        public IActionResult TwitterOAuthFlow(string type, bool reauthorize = false)
        {
            var success = false;
            var redirect = "";
            var message = "";
            var result = "";
            try
            {
                var baseUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs.appTechnicalConfigs.LocalUrl;
                var twitterConfigs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;
                result = twtAuthorizationManager.StartOAuthFlow(baseUrl,false, twitterConfigs);
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
            return Redirect(result);
           // return Json(new { success, redirect, message });
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

            baseUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs?.appTechnicalConfigs?.LocalUrl;
#else
            baseUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs?.appTechnicalConfigs?.PublicUrl;
#endif
                var result = fbAuthorizationManager.StartOAuthFlow(baseUrl, type, reauthorize).Decode();
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

                baseCallBackUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs?.appTechnicalConfigs?.LocalUrl;
#else
            baseCallBackUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs?.appTechnicalConfigs?.PublicUrl;
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
       // [PermissionFilter("Authorization_TwitterSignIn")]
        public async Task<IActionResult> TwitterSignIn(string oauth_token, string oauth_verifier)
        {
            if (string.IsNullOrEmpty(oauth_token) || string.IsNullOrEmpty(oauth_verifier))
            {
                return BadRequest("Missing OAuth token or verifier");
            }

            var channel = TwitterService.GetChannelInfo(oauth_token, oauth_verifier);
            if (channel.NetworkId == null)
            {
                Debug.WriteLine("Couldn't authorize the selected twitter account");
                return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Twitter.Key() });
            }
            var reauthorize = false;
            return SaveChannel(channel, reauthorize, SocialNetworkTypeEnum.Twitter);
        }
        [PermissionFilter("Authorization_DailymotionSignIn")]
        public IActionResult DailymotionSignIn(string network, string id)
        {
            var dailymotionConfigs = SocialServiceHelper.getSocialConfigs().DailymotionConfigs;
            var jsonDmConfigs = JsonConvert.SerializeObject(dailymotionConfigs);
            ChannelErrorEnum error;
            var channel = _channelService.GetById(id, out error);
            var masterId = "";
            switch (error)
            {
                case ChannelErrorEnum.NOT_FOUND:
                    if (isAdmin() || isMaster()) { masterId = currUserId(); }

                    channel = _channelService.Create(network, out error);
                    channel.Status = StatusEnum.Active.Value();
                    channel.Id = GuidExtension.GenerateGuid().ToString();
                    channel.Credentials = jsonDmConfigs;
                    channel.OriginalName = dailymotionConfigs.Application.ChannelName;
                    channel.NetworkId = "baaedba16b73e73577d7cd12e07158bd";
                    channel.ParentId = masterId;
                    _channelService.Insert(channel);
                    break;
                case ChannelErrorEnum.NO_ERROR:
                    return View(channel);
            }
            var allChannels = Collections.Channels();
            return RedirectToAction("Index", "Channels", new { area = "Social" });
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
        private IActionResult SaveChannel(Channel channel,bool  reauthorize, SocialNetworkTypeEnum networkTypeEnum)
        {
            if (reauthorize)
            {
                var networkType = networkTypeEnum.Key();
                var existingChannelsIds = Collections.Channels().Where(channel => channel.Network.EqualsIgnoreCase(networkType) && channel.Status.NotIn(new List<int> { StatusEnum.Deleted.Value() })).Select(channel => channel.NetworkId);
                
                channel = existingChannelsIds.ContainsIgnoreCase(channel.NetworkId) == true ? null : channel ;
                if (channel== null)
                {
                    _logger.LogError("********************* The reauthorized channel doesn't exist *********************");
                    return RedirectToAction("Create", "Channels", new { area = "social", type = networkType });
                }
            }

            /*TempData["channels"] = channels.ToList();*/
            if ( !reauthorize)
                _ = SaveNewChannel(channel);

            return RedirectToAction("Index", "Channels", new { area = "Social" });
        }
        private async Task<Channel> SaveNewChannel(Channel channel)
        {
            //var user = await _userManager.GetUserAsync(User);

            var masterId = "";
           
                var oldChannel = Collections.Channels().FirstOrDefault(c =>
                    c.NetworkId.EqualsIgnoreCase(channel.NetworkId) && c.Network.EqualsIgnoreCase(channel.Network));
                if (oldChannel == null)
                {
                    if (isAdmin() || isMaster())
                    {
                        masterId = currUserId();
                        channel.ParentId = masterId;
                    }

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
                    channel.Id = oldChannel.Id;
                
            }
            return channel;
        }
        private async Task<IEnumerable<Channel>> SaveNewChannels(IEnumerable<Channel> channels)
        {
            //var user = await _userManager.GetUserAsync(User);

            var masterId = "";
            foreach (var channel in channels)
            {
                var oldChannel = Collections.Channels().FirstOrDefault(c =>
                    c.NetworkId.EqualsIgnoreCase(channel.NetworkId) && c.Network.EqualsIgnoreCase(channel.Network));
                if (oldChannel == null)
                {
                    if (isAdmin() || isMaster())
                    {
                        masterId = currUserId();
                        channel.ParentId = masterId;
                    }
                   
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
            var masterId = "";
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
                        redirect = Url.Action("Create", "Channels", new { area = "social", type = networkType });
                    }
                    success = true;
                    channels.Add(channel);
                    foreach (var ch in channels)
                    {
                        ChannelErrorEnum error;
                        var oldChannel = _channelService.GetByNetwork(ch.Network, ch.NetworkId, out error);
                        switch (error)
                        {
                            case ChannelErrorEnum.NOT_FOUND:
                                if (isAdmin() || isMaster())
                                {
                                    masterId = currUserId();
                                    ch.ParentId = masterId;
                                }
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
            //redirect = Url.Action("Index", "Channel", new { area = "social"});
            return RedirectToAction("Index", "Channels", new { area = "Social" });
        }
/*
        public async Task<string> ApplicationUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
        public string currUserId()
        {
            return ApplicationUserId().GetAwaiter().GetResult();
        }
        public async Task<bool> isAdminAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            var isAdmin = currUser!=null && await _userManager.IsInRoleAsync(currUser, "Admin");
            return isAdmin;
        }
        public bool isAdmin()
        {
            return isAdminAsync().GetAwaiter().GetResult();
        }
        public async Task<bool> isMasterAsync()
        {
            var currUser = await _userManager.GetUserAsync(User);
            var isMaster = currUser != null && await _userManager.IsInRoleAsync(currUser, "Master");
            return isMaster;
        }
        public bool isMaster()
        {
            return isMasterAsync().GetAwaiter().GetResult();
        }*/
    }
}
