using FeedHiveAuth.Areas.Social.Controllers;
using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FeedHiveAuth.Controllers
{
    public class OAuthorizationController : Controller
    {
        private readonly ILogger<OAuthorizationController> _logger;

        public OAuthorizationController(ILogger<OAuthorizationController> logger)
        {
            _logger = logger;
        }

        [PermissionFilter("OAuthorization_FacebookSignIn")]
        //[EnableCors]
        public IActionResult FacebookSignIn(string state, string code)
        {
            try
            {
                var subscription = "1f59028d-15d0-4bf6-a61b-28f33b895310";
                var baseCallBackUrl = "";
#if DEBUG

                baseCallBackUrl = SocialConfigs.Construct().TechnicalConfigs.LocalUrl;
#else
            baseCallBackUrl = SocialConfigs.Construct().TechnicalConfigs.PublicUrl;
#endif
                var channels = FacebookService.GetChannelsInfo(baseCallBackUrl, code,
                                state,out string msg);

                if (channels.Empty())
                {
                    Debug.WriteLine(msg ?? "Couldn't authorize the selected Facebook account");
                    return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Facebook.Key() });
                }

                var reauthorize = false;
                return SaveChannels(channels, subscription, reauthorize, SocialNetworkTypeEnum.Facebook);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Couldn't authorize the selected Facebook account: " + ex.FullMessage());
                //return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Facebook.Key() });
                return null; 
            }
        }
        private IActionResult SaveChannels(IEnumerable<Channel> channels, string subscriptionId, bool reauthorize, SocialNetworkTypeEnum networkTypeEnum)
        {
            if (reauthorize)
            {
                var networkType = networkTypeEnum.Key();
                var existingChannelsIds = Collections.ChannelsOf(subscriptionId).Where(channel => channel.Network.EqualsIgnoreCase(networkType) && channel.Status.NotIn(new List<int> { StatusEnum.Deleted.Value() })).Select(channel => channel.NetworkId);
                channels = channels.Where(channel => existingChannelsIds.ContainsIgnoreCase(channel.NetworkId));
                if (channels.Empty())
                {
                    _logger.LogError("********************* The reauthorized channel doesn't exist *********************");
                    return RedirectToAction("Create", "Channels", new { area = "social", type = networkType });
                }
            }

            /*TempData["channels"] = channels.ToList();*/
            if (channels.Count() >= 1 && !reauthorize)
                _ = SaveNewChannels(channels, subscriptionId);
            return RedirectToAction("Index", "Channels", new { area = "Social" });
        }

        private async Task<IEnumerable<Channel>> SaveNewChannels(IEnumerable<Channel> channels,
            string? subscriptionId = null)
        {
            foreach (var channel in channels)
            {
                var oldChannel = Collections.Channels().FirstOrDefault(c =>
                    c.NetworkId.EqualsIgnoreCase(channel.NetworkId) && c.Network.EqualsIgnoreCase(channel.Network));
                //var result;
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

                //Collections.RefreshChannels();
                /*foreach (Exception resultException in result.Exceptions)
                {
                    //Logger.Warn(typeof(ChannelHelper), resultException.FullMessage());
                }*/
            }

            return channels;
        }

    }
}
