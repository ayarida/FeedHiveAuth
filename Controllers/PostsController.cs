using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace FeedHiveAuth.Controllers
{
    public class PostsController : Controller
    {
        [HttpGet]
        public ActionResult Create()
        {
            return View("~/Views/Posts/Create.cshtml");

        }
        [HttpPost]
        public ActionResult Create(HttpContext postForm)
        {
            Post post = new Post();

            var postMediaRequestForm = HttpContext.Request.Form.Files[0];
            post.Title = HttpContext.Request.Form["Title"];
            post.ShortTitle = HttpContext.Request.Form["ShortTitle"];
            post.Summary = HttpContext.Request.Form["Summary"];
            post.Content = HttpContext.Request.Form["Content"];
            var POSTDATE = HttpContext.Request.Form["PostDate"];

            post.PostMediaItem.Caption = post.Title;
            post.PostMediaItem.Path = postMediaRequestForm.FileName;


            /*if (model.File != null && model.File.Length > 0)
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
            }*/
            return View("~/Views/Posts/Create.cshtml");
        }
    }
}
