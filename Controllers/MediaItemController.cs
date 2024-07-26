using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.JSON;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class MediaItemController : Controller
    {
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        private readonly ILogger<PostsController> _logger;
        public MediaItemController()
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
 
        public IActionResult SaveMedia()
        {
            if (HttpContext.Request.Form.Files.Any())
            {
                var oneFile = HttpContext.Request.Form.Files[0];
                UploadFile(oneFile);
                List<MediaItem> Medias = _mediaItemService.MediasList(HttpContext.Request.Form.Files);//mapFileToMediaItem
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
        public void DeleteMediaItem(MediaItem mediaItem)
        {
            _mediaItemService.Delete(mediaItem.Id);
        }
        [HttpDelete]
        [PermissionFilter("MediaItem_DeletePostMedia")]
        public void DeletePostMedia(string media,string post)
        {
            _mediaItemService.DeletePostMedia(media, post);
        }

    }
}
