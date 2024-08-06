using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models.JSON;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Hosting;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
namespace FeedHiveAuth.Controllers
{

    public class PostsController : Controller
    {
        protected PostRepository _postService = Instances.Repositories.PostRepository;
        protected MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;
        protected OperationRepository _operationService = Instances.Repositories.OperationRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<PostsController> _logger;

        private readonly UserManager<IdentityUser> UserManager;

        public PostsController(UserManager<IdentityUser> userManager, ILogger<PostsController> logger)
        {
            this.UserManager = userManager;

            _logger = logger;
        }


        [HttpGet]
        [PermissionFilter("Posts_Create")]
        public ActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [PermissionFilter("Posts_Search")]
        public JsonResult Search([FromQuery] string valinput)
        {
            List<Post> posts = new List<Post>();
            List<Post> userPosts;
            var isAdmin = User.IsInRole("Admin");
            var isMaster = User.IsInRole("Master");
            var user = Instances.Repositories.UserRepository.GetByUsername(User.Identity.Name);
            if (isMaster)
            {
                var masterAdmins = _userService.GetWhosParentId(user.Id).ToList();//get admins
                foreach (var admin in masterAdmins)
                {
                    var adminUsers = _userService.GetWhosParentId(admin.ToString()).ToList();//get Users
                    if (adminUsers.Count > 0)
                    {
                        userPosts = _postService.GetAdminUsersPostsSearch(adminUsers, valinput);
                        posts.AddRange(userPosts);
                    }

                }

            }
            else if (isAdmin)
            {
                var adminUsers = _userService.GetWhosParentId(user.Id).ToList();
                if (adminUsers.Count > 0)
                {
                    posts = _postService.GetAdminUsersPostsSearch(adminUsers, valinput);
                }

                //posts = _postService.GetPosts();
            }
            else
            {
                posts = _postService.GetCurrentUserPostsSearch(user.Id, valinput);
            }
            //var postsresult = Instances.Repositories.PostRepository.GetPostByTitle(valinput);
            var result = new JsonResult(posts);
            return result;
        }
        [HttpGet]
        [PermissionFilter("Posts_Admins")]
        public ActionResult Admins()
        {
            var user = Instances.Repositories.UserRepository.GetByUsername(User.Identity.Name);
            var adminUsers = _userService.GetAll().Where(x => x.ParentId == user.Id);
            return View(adminUsers);

        }
        [HttpGet]
        [PermissionFilter("Posts_AllPosts")]
        public ActionResult AllPosts(string id)
        {
            var posts = new List<Post>();
            var user = Instances.Repositories.UserRepository.GetById(id);
            var Editors = _userService.GetAll().Where(x => x.ParentId == user.Id);
            var adminUsers = _userService.GetWhosParentId(user.Id).ToList();
            if (adminUsers.Count > 0)
            {
                posts = _postService.GetAdminUsersPostsSearch(adminUsers, "");
            }
            foreach (var post in posts)
            {
                var postMedia = _mediaItemService.GetMediasByPostId(post.Id);
                var createdByName = _userService.GetNameById(post.CreatedBy);
                var postOps = GetPostOperations(post.Id);

                if (postMedia != null) post.PostMediaItems = postMedia;
                post.CreatedByName = createdByName;
                if (postOps != null) post.Operations = postOps.Where(op => op.Status == StatusEnum.Success.Value() || op.Status == StatusEnum.Processing.Value()).ToList();
            }

            PostListViewModel pvm = new PostListViewModel
            {
                allPosts = posts,
                IsAdmin = true
            };
            return View("~/Views/Posts/List.cshtml", pvm);
        }
        [HttpGet]
        [PermissionFilter("Posts_List")]
        
        public async Task<ActionResult> List()
        {
            var user = await UserManager.GetUserAsync(User);
            var isAdminTask = UserManager.IsInRoleAsync(user, "Admin");
            var isAdmin = await isAdminTask;
            List<Post> posts;
            //Admin: return all posts who created them is member of admin team
            if (isAdmin)
            {
                var adminUsers = _userService.GetWhosParentId(user.Id).ToList();
                posts = _postService.GetAdminUsersPostsSearch(adminUsers, "");
                //posts = _postService.GetPosts();
            }
            //Else: return curr user posts
            else
            {
                posts = _postService.GetCurrentUserPostsSearch(user.Id, "");
            }
            foreach (var post in posts)
            {
                var postMedia = _mediaItemService.GetMediasByPostId(post.Id);
                var createdByName = _userService.GetNameById(post.CreatedBy);
                var postOps = GetPostOperations(post.Id);
                if (postMedia != null) post.PostMediaItems = postMedia;
                if (postOps != null) post.Operations = postOps.Where(op => op.Status == StatusEnum.Success.Value() || op.Status == StatusEnum.Processing.Value()).ToList();
                post.CreatedByName = createdByName;
            }
            PostListViewModel pvm = new PostListViewModel
            {
                allPosts = posts,
                IsAdmin = isAdmin
            };
            return View(pvm);
        }


        [HttpGet]
        public IActionResult Preview(string id)
        {
            var post = _postService.GetPostById(id);
            var ops = Instances.Repositories.OperationRepository.getPostOperationsById(id);
            post.Operations = (ops?.Count > 0) ? ops : new List<Operation>();

            var postMedia = _mediaItemService.GetPostMedias(id);
            var mediaList=new List<MediaItem>();
            foreach(var postmed in postMedia)
            {
               var media= _mediaItemService.GetMediaById(postmed.MediaItemId);
                mediaList.Add(media);
            }
            if (postMedia != null) { post.PostMediaItems = mediaList; };

            return View(post);
        }

        public async Task<bool> isAdmin()
        {
            var currUser = await UserManager.GetUserAsync(User);
            var isAdmin = await UserManager.IsInRoleAsync(currUser, "Admin");
            return isAdmin;
        }        
        [Authorize]
        [PermissionFilter("Posts_CreatePost")]
        [HttpPost]
        public ActionResult CreatePost(Post postForm)
        {
            if (postForm.Title != null)
            {
                postForm.PublicLink = "/Posts/" + GeneratePostLink(postForm.Title);
            }
            //Post post = new Post
            //{
            //    //Title = HttpContext.Request.Form["Title"],
            //    //ShortTitle = HttpContext.Request.Form["ShortTitle"],
            //    //Summary = HttpContext.Request.Form["Summary"],
            //    //Content = HttpContext.Request.Form["Content"],
            //    PublicLink = "/Posts/" + GeneratePostLink(HttpContext.Request.Form["Title"]),
            //    PostDate = DateTime.Parse(HttpContext.Request.Form["PostDate"])
            //};
            string userId = currUserId();
            if (userId != null)
            {
                
                postForm.ModifiedBy = postForm.CreatedBy = userId;
            }
            _postService.Save(postForm);

            /////////SaveMedia////////////////////

            //Medias from archive 
            var MediasFromArchive = new List<MediaData>();
            var med = HttpContext.Request.Form["mediaItemsPaths"];
            var testMed = postForm.mediaItemsPaths;
            if (med.ToString().IsNotNullOrEmpty())
            {
                MediasFromArchive = JsonSerializer.Deserialize<List<MediaData>>(med);
            }
            if (MediasFromArchive?.Count > 0)
            {
                foreach (var media in MediasFromArchive)
                {
                    _postService.InsertPostMedia(postForm.Id, media.Id);
                }

            }
            //New Medias ( from File)

            if (HttpContext.Request.Form.Files.Any())
            {
                var oneFile = HttpContext.Request.Form.Files[0];
                var message = UploadMedia(oneFile);
                List<MediaItem> postMedias = _mediaItemService.MediasList(HttpContext.Request.Form.Files,userId);

                //SavePostMedias(post);
                var result = SaveMedia(postMedias, postForm.Id);
                foreach (var media in result)
                {
                    _postService.InsertPostMedia(postForm.Id, media.Id);
                }
                //UploadMedia(post.PostMediaItems.FirstOrDefault());
            }
            postForm.PostMediaItems = _mediaItemService.GetMediasByPostId(postForm.Id);
            return RedirectToAction("Preview","Posts",new {id = postForm.Id});
        }

        [HttpPost]
        public ActionResult CreateQuickPost()
        {

            Post post = new Post
            {
                Title = HttpContext.Request.Form["Title"],
                Summary = HttpContext.Request.Form["Summary"],
                PublicLink = "/Posts/" + GeneratePostLink(HttpContext.Request.Form["Title"]),
                PostDate = DateTime.Now
            };
            var med = HttpContext.Request.Form["mediaItemPaths"];
            string userId = currUserId();
            if (userId != null)
            {
                post.ModifiedBy = post.CreatedBy = userId;
            }
            _postService.SaveQuickPost(post);
            if (HttpContext.Request.Form.Files.Any())
            {
                var oneFile = HttpContext.Request.Form.Files[0];
                var message = UploadMedia(oneFile);
                List<MediaItem> postMedias = _mediaItemService.MediasList(HttpContext.Request.Form.Files,userId);
                //SavePostMedias(post);
                var result = SaveMedia(postMedias, post.Id);
                //UploadMedia(post.PostMediaItems.FirstOrDefault());
            }
            post.PostDate = DateTime.Now;
            return RedirectToAction("Share", "Publish", new { area = "Social", postid = post.Id });
        }


        public List<MediaItem> SaveMedia(List<MediaItem> mediaItems, string postId)
        {
            try
            {
                var medias=_mediaItemService.InsertPostMedia(mediaItems, postId);
                return medias;
            }
            catch (Exception ex)
            {
                _logger.LogError("********************* Error in saving media, EXCEPTION \r\n " + ex + "\r\n*********************");
                return null;
            }
        }
        [HttpPost]
        [PermissionFilter("Posts_Update")]
        public ActionResult Update(Post updatedPost)
        {
            string userId = currUserId();
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
            var errors = new List<string>();

            if (postMedia.Count > 0)
                foreach (var med in postMedia)
                {
                    _mediaItemService.DeletePostMedia(med.Id, post.Id);
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
                //_logger.LogError();
                errors.Add($"\"********************* Error in deleting this post, EXCEPTION \\r\\n\" + {ex} + \"\\r\\n*********************\"");
                _logger.LogError($"********************* Error in deleting post with ID {post.Id}, EXCEPTION \r\n{ex}\r\n*********************");
            }
            if (errors.Any())
            {
                return BadRequest(errors);
            }
            return Ok();
        }

        public void Publish(string Id)
        {
            string userId = currUserId();

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
        public async Task<string> identityUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
        public string currUserId()
        {
            return identityUserId().GetAwaiter().GetResult();
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
            string userId = currUserId();

            try
            {
                var userPosts = _postService.GetCurrentUserPostsSearch(userId, "");
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


    }
}
