using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class MediaItemController : Controller
    {
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;

        public MediaItemController()
        {

        }

        [HttpPost]
        [PermissionFilter("MediaItem_UploadFile")]
        public IActionResult UploadFile(MediaItem model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.File.CopyTo(stream);
                }

                // You can save the file path or other information in your database if needed.

                return RedirectToAction("Index"); // Redirect to a relevant page after successful upload.
            }

            return View();
        }
        [HttpGet]
        [PermissionFilter("MediaItem_List")]
        public IActionResult List()
        {
            var mediaItemsList = _mediaItemService.GetMediaList();
            return View("~/Views/MediaItem/List.cshtml", mediaItemsList);
        }
        [PermissionFilter("MediaItem_DeleteMediaItem")]
        [HttpDelete]
        public void DeleteMediaItem(MediaItem mediaItem)
        {
            _mediaItemService.Delete(mediaItem.Id);
        }

    }
}
