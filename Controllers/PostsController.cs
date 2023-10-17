using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;
using NuGet.Protocol;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
namespace FeedHiveAuth.Controllers
{
    public class PostsController : Controller
    {
        protected PostRepository _postService = Instances.Repositories.PostRepository;
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View("~/Views/Posts/Create.cshtml");

        }

        [HttpPost]
        public ActionResult CreatePost()
        {
            GetCurrentUser();

            Post post = new Post();
            MediaItem postmedia = new MediaItem();
            //Single Media Upload
            /*if(HttpContext.Request.Form.Files!=null && HttpContext.Request.Form.Files.Count() == 1)
            {
                postmedia.File = HttpContext.Request.Form.Files[0];

            }
            else
            {
                //Multi Media Upload
                MultiUpload(HttpContext.Request.Form.Files);
            }*/
            post.Title = HttpContext.Request.Form["Title"];
            post.ShortTitle = HttpContext.Request.Form["ShortTitle"];
            post.Summary = HttpContext.Request.Form["Summary"];
            post.Content = HttpContext.Request.Form["Content"];
            post.PublicLink = "/Posts/" + GeneratePostLink(post.Title);

            if (HttpContext.Request.Form.Files.Any())
            {
                List<MediaItem> postMedias = _mediaItemService.MediasList(HttpContext.Request.Form.Files);
                post.PostMediaItems = postMedias;
            }

            _postService.Save(post);
            SavePostMedias(post);

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
                _mediaItemService.InsertPostMedia(post.PostMediaItems, post.Id);
            }
        }

        [HttpGet]
        public void DeletePost(Post post)
        {
            _postService.Delete(post.Id);
        }

        [HttpGet]
        public void Publish()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            string x = "Ssss";

        }


        public void GetCurrentUser()
        {
/*            string email = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;*/
            
            var nmid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var namm = User.FindFirstValue(ClaimTypes.Name);
            //Console.WriteLine("Role: " + User.FindFirstValue(ClaimTypes.Role));
            //Console.WriteLine("First name: " + User.FindFirstValue("firstname"));
            //Console.WriteLine("Last name: " + User.FindFirstValue("lastname"));
            var x = "S";
        }

    }
}
