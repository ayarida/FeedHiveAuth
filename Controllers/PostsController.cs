using FeedHiveAuth.Data;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;
using NuGet.Protocol;
using System.Text;
using System.Text.Json;

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
        public ActionResult CreatePost()
        {
            Post post = new Post();
            MediaItem postmedia = new MediaItem();
            //Single Media Upload
            if(HttpContext.Request.Form.Files!=null && HttpContext.Request.Form.Files.Count() == 1)
            {
                postmedia.File = HttpContext.Request.Form.Files[0];

            }
            else
            {
                //Multi Media Upload
                MultiUpload(HttpContext.Request.Form.Files);
            }
            post.Id = Guid.NewGuid().ToString();
            post.Title = HttpContext.Request.Form["Title"];
            post.ShortTitle = HttpContext.Request.Form["ShortTitle"];
            post.Summary = HttpContext.Request.Form["Summary"];
            post.Content = HttpContext.Request.Form["Content"];
            //post.PostDate = DateTime.Parse(HttpContext.Request.Form["PostDate"].ToString());
            post.PublicLink = "/Posts/" + GeneratePostLink(post.Title);
            

            List<MediaItem> postMedias = Instances.Repositories.MediaItemRepository.MediasList(HttpContext.Request.Form.Files);
            //JsonResult PostMediaItems = Json(files);
            //Save all post media in DB

            post.PostMediaItems = postMedias;
            SavePostMedias(post);

            //var jsonObject = JsonSerializer.Serialize(postmedia);



            //Instances.Repositories.PostRepository.SavePost(post);

            return View("~/Views/Posts/Create.cshtml");
        }

        public MediaItem UploadMedia(MediaItem model)
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

                return model; // Redirect to a relevant page after successful upload.
            }

            return model;
        }

        public void MultiUpload(IFormFileCollection Files)
        {
           /* if (ModelState.IsValid)
            {
                if (model.Files.Count > 0)
                {*/
                    foreach (var file in Files)
                    {

                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");

                        //create folder if not exist
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);


                        string fileNameWithPath = Path.Combine(path, file.FileName);

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                   
                /*}
                
            }*/
        }
        public static string GeneratePostLink(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            var stringBuilder = new StringBuilder();
            foreach (char c in input.ToArray())
            {
                if (Char.IsLetterOrDigit(c))
                {
                    stringBuilder.Append(c);
                }
                else if (c == ' ')
                {
                    stringBuilder.Append("-");
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        public void SavePostMedias(Post post)
        {
            if (post.PostMediaItems.Any())
            {
                Instances.Repositories.MediaItemRepository.InsertPostMedia(post.PostMediaItems, post.Id);
            }
        }

    }
}
