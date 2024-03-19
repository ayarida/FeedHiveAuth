using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FeedHiveAuth.Controllers
{
    public class OAuthorizationController : Controller
    {

        /*        public void FacebookSignIn(string code)
                {
                    var credentials = AuthorizationManager.ExchangeCode(baseCallbackUrl, code, subscriptionId, out msg);
                }*/
        [PermissionFilter("OAuthorization_FacebookSignIn")]
        public IEnumerable<Channel> FacebookSignIn(string state, string code)
        {
            try
            {
                var subscription = "1f59028d-15d0-4bf6-a61b-28f33b895310";
                var channels = FacebookService.GetChannelsInfo("https://localhost:7157/",code,
                                state, subscription, out string msg);
                if (channels.Empty())
                {
                    Debug.WriteLine(msg ?? "Couldn't authorize the selected Facebook account");
                    //return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Facebook.Key() });
                }
                var reauthorize = false;
                return channels;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Couldn't authorize the selected Facebook account: " + ex.FullMessage());
                //return RedirectToAction("Create", "Channels", new { area = "social", type = SocialNetworkTypeEnum.Facebook.Key() });
                return null;
            }
        }

    }
}
