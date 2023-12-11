using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class MediaItemController : Controller
    {
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;

        public MediaItemController() {   
            
        }

        public ActionResult Upload()
        {

            return  View("~/Views/MediaItem/Upload.cshtml");
        }

        [HttpPost]
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

        //MediaUpload based on coming media type 
        public ActionResult MediaUpload(string type)
        {
            return View("Upload/_mediaUploadWizard", type);
        }

        [HttpGet]
        public IActionResult List() {
            var mediaItemsList = _mediaItemService.GetMediaList();
            return View("~/Views/MediaItem/List.cshtml", mediaItemsList);
        }
    }
}
