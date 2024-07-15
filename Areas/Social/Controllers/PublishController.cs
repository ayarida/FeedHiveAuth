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
using RestSharp.Extensions;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class PublishController : Controller
    {
        PostRepository _postService = Instances.Repositories.PostRepository;
        MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        OperationRepository _operationService = Instances.Repositories.OperationRepository;
        ILogger<PostsController> _logger;

        public PublishController(ILogger<PostsController> logger)
        {
            _logger = logger;
        }
        [PermissionFilter("Publish_GetChannels")]
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
        [PermissionFilter("Publish_Share")]
        public async Task<IActionResult> Share(string? postid = null)
        {

            var svm = FillShareViewForm(postid);
            return View(svm);
        }
        [PermissionFilter("Publish_GetActiveNetworkTypes")]
        public List<SocialNetworkTypeEnum> GetActiveNetworkTypes(SocialConfigs socialConfigs)
        {
            var activeNetworkTypes = new List<SocialNetworkTypeEnum>();
            //return an empty list
            if (socialConfigs == null) return new List<SocialNetworkTypeEnum> { };

            if (socialConfigs.FacebookConfigs?.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.Facebook);

            if (socialConfigs.TelegramConfigs?.Bot?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.Telegram);

            if (socialConfigs.DailymotionConfigs?.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.DailyMotion);

            if (socialConfigs.WhatsappConfigs?.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.WhatsApp);

            if (socialConfigs.TwitterConfigs?.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.Twitter);

            /*if (socialConfigs.InstagramConfigs?.Application?.Enabled ?? false)
                activeNetworkTypes.Add(SocialNetworkTypeEnum.Instagram);*/

            return activeNetworkTypes;
        }
        [PermissionFilter("Publish_Send")]
        [HttpPost]
        public IActionResult Send(ShareForm form)
        {
            var svm = FillShareViewForm(form.PostId);
            try
            {            
                var currUser = GlobalContext.UserConfigs;
                if (form.Channels.Empty())
                {
                    form.Channels = Instances.Repositories.ChannelRepository.GlobalGetAll().Select(ch => ch.Id).ToList();
                    _logger.LogError("********************* No Channels Selected! ********************");
                    TempData["ShareToSocialMsg"] = "No channels selected!";
                    return RedirectToAction("Preview", "Posts", new { id = form.PostId });
                }
                if (form.Data.Empty() || form.Data.First().Text.Empty())
                {
                    _logger.LogError("********************* No Data Passed! ********************");
                    return RedirectToAction("Preview", "Posts", new { id = form.PostId });
                }
                form.Channels = svm.Channels.Select(X => X.Id).ToList();
                var creationDate = DomainTime.Now();
                foreach (var data in form.Data.Where(formData => formData.ChannelId.In(form.Channels)))
                {
                    var channel = svm.Channels.FirstOrDefault(x => x.Id.Equals(data.ChannelId) && x.Id.In(form.Channels));
                    if (channel == null) continue;
                    var operationId = SocialPublishService.Send(form.PostId, form.MediaId, data, channel, creationDate);
                    var op = _operationService.Get(operationId);
                    if (operationId == null )
                    {
                        TempData["ShareToSocialMsg"] = "Something wrong happened, check post operations.";
                        return RedirectToAction("Preview", "Posts", new { id = form.PostId });
                    }
                }
                //TempData["ShareToSocialMsg"] = "Published Successfully!";
                return RedirectToAction("Preview", "Posts", new { id = form.PostId });
            }
            catch (Exception e)
            {
                _logger.LogError("********************* " + e.Message + "*********************");
                TempData["Error"] = e.Message;
                return RedirectToAction("Preview", "Posts", new { id = form.PostId });
            }
        }

        [HttpGet]
        public ActionResult GetPreview(string previewName, string postId)
        {
            var post = _postService.Get(postId);
            var media = _mediaItemService.GetMediaByPostId(postId);
            var medias = _mediaItemService.GetMediasByPostId(postId);
            if (medias != null)
                post.PostMediaItems = medias;

            return PartialView("~/Areas/Social/Views/Shared/_" + previewName + "Preview.cshtml", post);
        }

        public ShareView FillShareViewForm(string? postid=null)
        {
            var subSocialConfigs = SocialServiceHelper.getSocialConfigs();
            PublishErrorEnum error;
            var channels = GetChannels(out error);
            var activeNetworkTypes = GetActiveNetworkTypes(subSocialConfigs);
            var post = _postService.Get(postid);
            var media = _mediaItemService.GetMediaByPostId(postid);
            var medias = _mediaItemService.GetMediasByPostId(postid);
            if (medias != null)
                post.PostMediaItems = medias;
            var svm = new ShareView {
                ActiveSocialNetworks = activeNetworkTypes,
                Channels = channels,
                Post = post,
                Media = media,
                Medias = medias
            
            };
            return svm;
        }
    }

}
