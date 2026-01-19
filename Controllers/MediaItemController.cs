using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.JSON;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeedHiveAuth.Controllers
{
    public class MediaItemController : BaseController<MediaItemController>
    {
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        protected PostRepository _postService = Instances.Repositories.PostRepository;
        private readonly ILogger<MediaItemController> _logger;
        
        public MediaItemController(UserManager<ApplicationUser> userManager, ILogger<MediaItemController> _logger) : base(userManager, _logger)
        {
        }
        public int InsertMedia(List<MediaItem> mediaItems)
        {
            try
            {
                _mediaItemService.CreateMedia(mediaItems);
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError("********************* Error in saving media, EXCEPTION \r\n " + ex + "\r\n*********************");
                return 0;
            }
        }
        [HttpPost]
        [PermissionFilter("MediaItem_UploadFile")]
        public string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return "File uploaded successfully!";
            }

            return "File failed to upload!";

        }


        [Authorize]
        [HttpPost]
        [PermissionFilter("MediaItem_SaveMedia")]

        public async Task<IActionResult> SaveMedia()
        {
            if (HttpContext.Request.Form.Files.Any())
            {
                //var currentUser = Instances.Repositories.UserRepository.GetByUsername(User.Identity.Name).Id;
                var currUser = await getApplicationUser();
                var oneFile = HttpContext.Request.Form.Files[0];
                UploadFile(oneFile);
                List<MediaItem> Medias = _mediaItemService.MediasList(HttpContext.Request.Form.Files, currUser);//mapFileToMediaItem
                InsertMedia(Medias.ToList());
            }
            return RedirectToAction("List", "MediaItem");
        }


        [HttpGet]
        [PermissionFilter("MediaItem_Create")]
        public IActionResult Create()
        {

            return View("~/Views/MediaItem/Create.cshtml");
        }
        public List<MediaItem> currUserMediaItems()
        {
            var currentUser = Instances.Repositories.UserRepository.GetByUsername(User.Identity.Name).Id;
            var mediaItemsList = _mediaItemService.GetMediasByUser(currentUser.ToString());
            return mediaItemsList;
        }
        public async Task<List<MediaItem>> getOrganizationMedia()
        {
            var currentUser = await getApplicationUser();
            var mediaItems = _mediaItemService.GetOrganizationMedia(currentUser.OrganizationId);
            //var mediaItemsList = _mediaItemService.GetMediasByUser(currentUser.ToString());
            return mediaItems;
        }
        [HttpGet]
        [PermissionFilter("MediaItem_List")]
        public IActionResult List()
        {
            var mediaItemsList = currUserMediaItems();
            return View(mediaItemsList);
        }
        public JsonResult GetFromArchive()
        {
            var mediaItemsList = currUserMediaItems();
            return new JsonResult(mediaItemsList);
        }
        [HttpDelete]
        [PermissionFilter("MediaItem_DeleteMediaItem")]
        public void DeleteMediaItem(string id)
        {
            var relatedPostMedias = _mediaItemService.GetPostMediaRelations(id);

            // If related, delete those entries first
            if (relatedPostMedias != null && relatedPostMedias.Any())
            {
                foreach (var postMedia in relatedPostMedias)
                {
                    _mediaItemService.DeletePostMedia(postMedia.MediaItemId, postMedia.PostId);
                }
            }
            _mediaItemService.Delete(id);
        }
        [HttpDelete]
        [PermissionFilter("MediaItem_DeletePostMedia")]
        public void DeletePostMedia(string media, string post)
        {
            _mediaItemService.DeletePostMedia(media, post);
        }

    }
}
