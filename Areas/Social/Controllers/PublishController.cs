using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.Models.Services;
using FeedHiveAuth.Controllers;
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
    [Area("Social")]
    public class PublishController : Controller
    {
        PostRepository _postService = Instances.Repositories.PostRepository;
        MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        ILogger<PostsController> _logger; 

        public PublishController(ILogger<PostsController> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Channel> GetChannels(out PublishErrorEnum error)
        {
            var channels = Collections.Channels().Where(channel => channel.Status.In(new List<int> { StatusEnum.Active.Value(), StatusEnum.Expired.Value() }));

            if (!channels.Any())
            {
                error = PublishErrorEnum.NO_CHANNELS;
                return null;
            }
            //channels.ForEach(x => x.Name = x.Name.Nval(x.OriginalName));
            error = PublishErrorEnum.NO_ERROR;
            return channels;
        }
        public async Task<IActionResult> Share(string? postid = null)
        {
            //get current subscription
            //var subscriptionId = "1f59028d-15d0-4bf6-a61b-28f33b895310";
            var media = _mediaItemService.GetMediaByPostId(postid);
            PublishErrorEnum error;
            //Aya's local DB ids for API testing reasons

            var post = _postService.Get(postid);
            var subSocialConfigs = SocialServiceHelper.GetConfigs();
            var channels = GetChannels(out error);
            var activeNetworkTypes = GetActiveNetworkTypes(subSocialConfigs);
            var model = new ShareView
            {
                Post = post,
                Media = media != null ? _postService.GetPostMedia(postid, media.Id, out error) : null,
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

            if (socialConfigs.DailymotionConfigs.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.DailyMotion);

            return activeNetworkTypes;
        }

        [HttpPost]
        public IActionResult Send(ShareForm form)
        {
            try
            {

                var currUser = GlobalContext.UserConfigs;
                if (form.Channels.Empty())
                {
                    form.Channels =  Instances.Repositories.ChannelRepository.GlobalGetAll().Select(ch => ch.Id).ToList();
                    _logger.LogError("********************* No Channels Selected! ********************");
                    //create an Error Page to redirect 
                    //return View();
                }
                if (form.Data.Empty())
                {
                    _logger.LogError("********************* No Data Passed! ********************");
                    //create an Error Page to redirect 
                    //return View();
                }
                var channels = Instances.Repositories.ChannelRepository.GlobalGetAll();
                form.Channels = channels.Select(X => X.Id).ToList();
                var creationDate = DomainTime.Now();
                foreach (var data in form.Data.Where(formData => formData.ChannelId.In(form.Channels)))
                {
                    var channel = channels.FirstOrDefault(x => x.Id.Equals(data.ChannelId) && x.Id.In(form.Channels));
                    if (channel == null) continue;
                    var operationId = SocialPublishService.Send(form.PostId, form.MediaId, data, channel, creationDate);

                }
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return BadRequest(e.Message);
            }
        }
    }
}
