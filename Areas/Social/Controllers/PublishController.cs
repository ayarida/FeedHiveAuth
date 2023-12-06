using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.Models.Services;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    public class PublishController : Controller
    {
        PostRepository _postService = Instances.Repositories.PostRepository;

        public IEnumerable<Channel> GetChannels(string subscriptionId, out PublishErrorEnum error)
        {
            var channels = Collections.ChannelsOf(subscriptionId).Where(channel => channel.Status.In(new List<int> { StatusEnum.Active.Value(), StatusEnum.Expired.Value() }));

            if (!channels.Any())
            {
                error = PublishErrorEnum.NO_CHANNELS;
                return null;
            }
            //channels.ForEach(x => x.Name = x.Name.Nval(x.OriginalName));
            error = PublishErrorEnum.NO_ERROR;
            return channels;
        }
        [Area("Social")]
        [HttpGet(template: "/Publish/Share", Name = "SocialPublish")]
        public async Task<IActionResult> Share(string? postid = null, string? mediaid = null)
        {
            //get current subscription
            var subscriptionId = "1f59028d-15d0-4bf6-a61b-28f33b895310";
            //get the required post by Id 
            PublishErrorEnum error;
            var post = _postService.Get(postid);
            var media = _postService.GetPostMedia(postid, mediaid, out error);

            var subSocialConfigs = SocialServiceHelper.GetConfigs(subscriptionId);
            var channels = GetChannels(subscriptionId, out error);
            var activeNetworkTypes = GetActiveNetworkTypes(subSocialConfigs);
            var model = new ShareView
            {
                Post = post,
                Media = media,
                Channels = channels,
                ActiveSocialNetworks = activeNetworkTypes
            };
            return View(model);

        }

        public List<SocialNetworkTypeEnum> GetActiveNetworkTypes(SocialConfigs socialConfigs)
        {
            var activeNetworkTypes = new List<SocialNetworkTypeEnum>();
            //return an empty list
            if (socialConfigs == null) return new List<SocialNetworkTypeEnum> { };

            if (socialConfigs.FacebookConfigs?.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.Facebook);

            if (socialConfigs.TelegramConfigs?.Bot?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.Telegram);

            return activeNetworkTypes;
        }

        [HttpPost]
        public async void Send(ShareForm form)
        {
            try
            {
                var currUser = GlobalContext.UserConfigs?.UserData;
                if (form.Channels.Empty())
                {
                    Console.WriteLine("NoChannelsSelected");
                    //create an Error Page to redirect 
                    //return View();
                }
                if (form.Data.Empty())
                {
                    Console.WriteLine("NoData");
                    //create an Error Page to redirect 
                    //return View();
                }
                var channels = Instances.Repositories.ChannelRepository.GlobalGetAll();
                var creationDate = DomainTime.Now();
                foreach (var data in form.Data.Where(formData => formData.ChannelId.In(form.Channels)))
                {
                    var channel = channels.FirstOrDefault(x => x.Id.Equals(data.ChannelId) && x.Id.In(form.Channels));
                    if (channel == null) continue;
                    var operationId = SocialPublishService.Send(form.PostId, form.MediaId, data, channel, creationDate);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
