using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
namespace FeedHiveAuth.Controllers
{

    public class PostsController : Controller
    {
        protected PostRepository _postService = Instances.Repositories.PostRepository;
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<PostsController> _logger;

        private readonly IUserService _userServiceContext;
        public PostsController(IUserService _userService, ILogger<PostsController> logger)
        {
            _userServiceContext = _userService;
            _logger = logger;
        }

        [Authorize(Policy = "Admin", Roles = "Admin")]
        [HttpGet]
        public string RegisterUserstoSubscription()
        {
            return "You are admin and have access";
        }

        /*[Authorize(Policy = "Admin",Roles = "Admin")]*/
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpGet]
        public ActionResult List()
        {
            var posts = _postService.GetPosts();
            foreach(var post in posts)
            {
                var postMedia = _mediaItemService.GetMediasByPostId(post.Id);
                if(postMedia!=null) post.PostMediaItems = postMedia;
            }
            return View(posts);
        }


        [HttpPost]
        public ActionResult CreatePost()
        {

            Post post = new Post
            {
                Title = HttpContext.Request.Form["Title"],
                ShortTitle = HttpContext.Request.Form["ShortTitle"],
                Summary = HttpContext.Request.Form["Summary"],
                Content = HttpContext.Request.Form["Content"],
                PublicLink = "/Posts/" + GeneratePostLink(HttpContext.Request.Form["Title"]),
                PostDate = DateTime.Parse(HttpContext.Request.Form["PostDate"])
            };
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                post.ModifiedBy = post.CreatedBy = currentUser.Id;
            }
            _postService.Save(post);
            if (HttpContext.Request.Form.Files.Any())
            {
                var oneFile = HttpContext.Request.Form.Files[0];
                var message = UploadMedia(oneFile);
                List<MediaItem> postMedias = _mediaItemService.MediasList(HttpContext.Request.Form.Files);

                //SavePostMedias(post);
                var result = SaveMedia(postMedias, post.Id);
                //UploadMedia(post.PostMediaItems.FirstOrDefault());
            }
            post.PostMediaItems = _mediaItemService.GetMediasByPostId(post.Id);
            return View("~/Views/Posts/Create.cshtml");
        }

        public int SaveMedia(List<MediaItem> mediaItems, string postId)
        {
            try
            {
                _mediaItemService.InsertPostMedia(mediaItems, postId);
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError("------------ Exception in saving media ", ex);
                return 0;
            }
        }
        [HttpPost]
        public ActionResult UpdatePost(string Id)
        {
            Post post = _postService.GetPostById(Id);
            _postService.UpdatePostData(post);
            return View("~/Views/Home/Index.cshtml");
        }

        public string UploadMedia(IFormFile file)
        {
            if (file != null)
            {
                //var wwwPath = this.Environment.WebRootPath;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                //Uploaded Successfully
                return "File Uploaded Successfully";
            }

            return "File Failed to upload!";
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
                foreach (var postMediaItem in post.PostMediaItems)
                {
                    try
                    {
                        //UploadMedia();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("------------ Exception in saving media ", ex);
                    }
                }
                _mediaItemService.InsertPostMedia(post.PostMediaItems, post.Id);
            }
        }


        [HttpGet]
        public void DeletePost(Post post)
        {
            var postMedia = _mediaItemService.GetMediaByPostId(post.Id);
            if (postMedia != null) _mediaItemService.Delete(postMedia.Id);
            _postService.Delete(post.Id);
        }

        [HttpGet]
        public void Publish(string Id)
        {
            //System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var currUser = GetCurrentUser();
            _postService.Publish(Id, currUser.Id);

        }

        public void MultiplePublish([FromQuery] string idsStr)
        {
            Guid[] idsArray = idsStr.Split(',').Select(Guid.Parse).ToArray() ?? Array.Empty<Guid>();
            List<string> stringList = new List<string>();
            foreach (Guid guid in idsArray)
            {
                stringList.Add(guid.ToString());
            }

            var posts = _postService.GetPostsByIds(stringList);

            foreach (var post in posts)
            {
                try
                {
                    Publish(post.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while publishing this post", post);
                }
            }
        }


        public void MultipleDelete([FromQuery] string idsStr)
        {


            Guid[] idsArray = idsStr?.Split(',').Select(Guid.Parse).ToArray() ?? Array.Empty<Guid>();
            List<string> stringList = new List<string>();
            foreach (Guid guid in idsArray)
            {
                stringList.Add(guid.ToString());
            }
            var posts = _postService.GetPostsByIds(stringList);

            foreach (var post in posts)
            {
                try
                {
                    DeletePost(post);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while deleting this post", post);
                }
            }
        }

        [HttpGet]
        public List<Post> PublishedPosts()
        {
            List<Post> publishedPosts = _postService.GetPublishedPosts();
            return publishedPosts;
        }

        public User GetCurrentUser()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userService.Get(userId);
            return currentUser;
        }


        [HttpGet]
        public ActionResult Edit(string Id)
        {
            var post = _postService.GetPostById(Id);
            return View(post);
        }


        [HttpGet]
        public ActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public List<Post> GetCurrUserPosts()
        {
            try
            {
                var userPosts = _postService.GetCurrentUserPosts(GetCurrentUser()?.Id);
                return userPosts;
            }
            catch (Exception ex)
            {
                _logger.LogError("********************* NULL USER EXCEPTION: *********************",ex);
            }
            return new List<Post>();
        }
    }
}
