using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text;
namespace FeedHiveAuth.Controllers
{

    public class PostsController : Controller
    {
        protected PostRepository _postService = Instances.Repositories.PostRepository;
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        protected OperationRepository _operationService = Instances.Repositories.OperationRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<PostsController> _logger;

        private readonly UserManager<IdentityUser> userManager;

        public PostsController(UserManager<IdentityUser> userManager, ILogger<PostsController> logger)
        {
            this.userManager = userManager;

            _logger = logger;
        }


        [HttpGet]
        [PermissionFilter("Posts_Create")]
        public ActionResult Create()
        {
            return View();
        
        }

        [PermissionFilter("Posts_List")]
        [HttpGet]
        public async Task<ActionResult> List()
        {
           var user = await userManager.GetUserAsync(User);
            var isAdminTask = userManager.IsInRoleAsync(user, "Admin");
            var isAdmin = await isAdminTask;
            List<Post> posts;
            //Admin: return all posts who created them is member of admin team
            if (isAdmin)
            {
                var adminUsers = _userService.GetWhosParentId(user.Id).ToList();
                posts = _postService.GetAdminUsersPosts(adminUsers);
                //posts = _postService.GetPosts();
            }
            //Else: return curr user posts
            else
            {
                posts = _postService.GetCurrentUserPosts(user.Id);
            }
            foreach (var post in posts)
            {
                var postMedia = _mediaItemService.GetMediasByPostId(post.Id);
                var createdByName = _userService.GetNameById(post.CreatedBy);
                if (postMedia != null) post.PostMediaItems = postMedia;
                post.CreatedByName = createdByName;
            }
            PostListViewModel pvm = new PostListViewModel {
                allPosts = posts, 
                IsAdmin = isAdmin
            };
            return View("~/Views/Posts/List.cshtml", pvm);
        }

        public async Task<bool> isAdmin()
        {
            var currUser = await userManager.GetUserAsync(User);
            var isAdmin = await userManager.IsInRoleAsync(currUser, "Admin");
            return isAdmin;
        }

        [Authorize]
        [PermissionFilter("Posts_CreatePost")]
        [HttpPost]
        public Task<ActionResult> CreatePost()
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
            string userId = GetCurrentUserId().GetAwaiter().GetResult();
            if (userId != null)
            {
                post.ModifiedBy = post.CreatedBy = userId;
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
            return List();
        }
        public ActionResult CreateQuickPost()
        {

            Post post = new Post
            {
                Title = HttpContext.Request.Form["Title"],
                ShortTitle = HttpContext.Request.Form["ShortTitle"],
                Summary = HttpContext.Request.Form["Summary"],
                Content = HttpContext.Request.Form["Content"],
                PublicLink = "/Posts/" + GeneratePostLink(HttpContext.Request.Form["Title"]),
                PostDate = DateTime.Now
            };
            string userId = GetCurrentUserId().GetAwaiter().GetResult();
            if (userId != null)
            {
                post.ModifiedBy = post.CreatedBy = userId;
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
            post.PostDate = DateTime.Now;
            return RedirectToAction("Edit", "Posts", post.Id, "");
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
                _logger.LogError("********************* Error in saving media, EXCEPTION \r\n " + ex + "\r\n*********************");
                return 0;
            }
        }
        [PermissionFilter("Posts_Update")]
        [HttpPost]
        public ActionResult Update(Post updatedPost)
        {
            string userId = GetCurrentUserId().GetAwaiter().GetResult();
            var oldPost = _postService.GetPostById(updatedPost.Id);
            oldPost.Title = updatedPost.Title;
            oldPost.ShortTitle = updatedPost.ShortTitle;
            oldPost.Content = updatedPost.Content;
            oldPost.Summary = updatedPost.Summary;
            oldPost.PublicLink = GeneratePostLink(updatedPost.Title);
            oldPost.ModifiedBy = userId;
            try
            {
                var result = _postService.Update(oldPost);
            }
            catch (Exception ex)
            {
                _logger.LogError("********************* Can't Update Post, EXCEPTION: \r\n" + ex + "\r\n*********************");
            }

            return RedirectToAction("List", "Posts");
        }
        [PermissionFilter("Posts_UploadMedia")]
        public string UploadMedia(IFormFile file)
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
        [PermissionFilter("Posts_MultiUpload")]
        public void MultiUpload(IFormFileCollection Files)
        {
            foreach (var file in Files)
            {

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                string fileNameWithPath = Path.Combine(path, file.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
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
        [PermissionFilter("Posts_SavePostMedia")]
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
                        _logger.LogError("********************* Error in saving media, EXCEPTION \r\n " + ex + "\r\n*********************");
                    }
                }
                _mediaItemService.InsertPostMedia(post.PostMediaItems, post.Id);
            }
        }
        [PermissionFilter("Posts_Delete")]
        public IActionResult Delete(Post post)
        {
            var postMedia = _mediaItemService.GetMediaByPostId(post.Id);
            var postOps = _postService.GetPostOperations(post.Id);
            if (postMedia != null)
            {
                _mediaItemService.Delete(postMedia.Id);
            }
            if (postOps != null)
            {
                foreach (var op in postOps)
                {
                    _operationService.Delete(op.Id);
                }
            }
            try
            {
                _postService.Delete(post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("********************* Error in deleting this post, EXCEPTION \r\n" + ex + "\r\n*********************");
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        public void Publish(string Id)
        {
            string userId = GetCurrentUserId().GetAwaiter().GetResult();

            _postService.Publish(Id, userId);

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
                    _logger.LogError("********************* Error in publishing this post, EXCEPTION \r\n" + ex + "\r\n*********************");
                }
            }
        }

        [PermissionFilter("Posts_MultipleDelete")]
        public IActionResult MultipleDelete([FromQuery] string idsStr)
        {


            Guid[] idsArray = idsStr?.Split(',').Select(Guid.Parse).ToArray() ?? Array.Empty<Guid>();
            List<string> stringList = new List<string>();
            var errors = new List<string>();
            foreach (Guid guid in idsArray)
            {
                stringList.Add(guid.ToString());
            }
            var posts = _postService.GetPostsByIds(stringList);

            foreach (var post in posts)
            {
                try
                {
                    Delete(post);
                }
                catch (Exception ex)
                {
                    errors.Add($"Error deleting post with ID {post.Id}: {ex.Message}");
                    _logger.LogError($"********************* Error in deleting post with ID {post.Id}, EXCEPTION \r\n{ex}\r\n*********************");
                }
            }
            if (errors.Any())
            {
                return BadRequest(errors);
            }
            return Ok();
        }

        [HttpGet]
        public List<Post> PublishedPosts()
        {
            List<Post> publishedPosts = _postService.GetPublishedPosts();
            return publishedPosts;
        }
/*        [PermissionFilter("Posts_GetCurrentUser")]
        public User GetCurrentUser()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userService.Get(userId);
            return currentUser;
        }*/
        public async Task<string> GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await userManager.GetUserAsync(User);

            return userId;
        }

        [PermissionFilter("Posts_Edit")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var post = _postService.GetPostById(id);
            var postMedia = _mediaItemService.GetMediasByPostId(id);
            if (postMedia != null) { post.PostMediaItems = postMedia; };
            return View(post);
        }

        [PermissionFilter("Posts_Calender")]
        [HttpGet]
        public ActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public List<Post> GetCurrUserPosts()
        {
            string userId = GetCurrentUserId().GetAwaiter().GetResult();

            try
            {
                var userPosts = _postService.GetCurrentUserPosts(userId);
                return userPosts;
            }
            catch (Exception ex)
            {
                _logger.LogError("********************* NULL USER EXCEPTION: *********************", ex);
            }
            return new List<Post>();
        }
        [PermissionFilter("Posts_GetPostOperations")]
        [HttpGet]
        public List<Operation> GetPostOperations(string postId)
        {
            var ops = Instances.Repositories.OperationRepository.getPostOperationsById(postId);
            return ops;
        }
        [PermissionFilter("Posts_Search")]
        [HttpPost]
        public JsonResult Search([FromQuery] string valinput) 
        {
            var postsresult = Instances.Repositories.PostRepository.GetPostByTitle(valinput);
            var result = new JsonResult(postsresult);
            return result;
        }
    }
}
