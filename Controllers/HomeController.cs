using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FeedHiveAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SignInManager<ApplicationUser> SignInManager;
        public UserManager<ApplicationUser> UserManager;
        protected PostRepository _postRepository = Instances.Repositories.PostRepository;
        protected MediaItemRepository _mediaRepository = Instances.Repositories.MediaItemRepository;
        //private readonly IConfiguration configuration;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            var posts = _postRepository.GetPosts();
            string userId = GetCurrentUserId().GetAwaiter().GetResult();

            foreach (var post in posts)
            {
                post.PostMediaItems = _mediaRepository.GetMediasByPostId(post.Id);
            }
            var userPosts = _postRepository.GetCurrentUserPosts(userId);
            var currentDate = DateTime.Now.Date;
            var todaysPosts = _postRepository.GetByDate(currentDate,userId);
            UserDataViewModel uvm = new UserDataViewModel
            {
                allPosts = posts,
                userPostsCount = userPosts.Count(),
                userPosts = userPosts,
                userTotalTodayCount = todaysPosts.Count()
            };
            return View(uvm);
        }
        public async Task<string> GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await _userManager.GetUserAsync(User);

            return userId;
        }
        public IActionResult Welcome()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ReceiveChannels([FromBody] List<Channel> channels)
        {
            if (channels != null && channels.Any())
            {
                SaveNewChannels(channels);
                return Ok("Channels received successfully!");
            }

            return BadRequest("No channels data received.");
        }

        private async Task<IEnumerable<Channel>> SaveNewChannels(IEnumerable<Channel> channels)
        {
            //var user = await _userManager.GetUserAsync(User);

            var masterId = "";
            foreach (var channel in channels)
            {
                var oldChannel = Collections.Channels().FirstOrDefault(c =>
                    c.NetworkId.EqualsIgnoreCase(channel.NetworkId) && c.Network.EqualsIgnoreCase(channel.Network));
                if (oldChannel == null)
                {
                    /*if (isAdmin() || isMaster())
                    {
                        masterId = currUserId();
                        channel.ParentId = masterId;
                    }*/

                    var newChannel = Instances.Repositories.ChannelRepository.AddChannel(channel);
                    channel.Id = newChannel.Id;
                }
                else
                {
                    oldChannel.OriginalName = channel.OriginalName;
                    oldChannel.NetworkUrl = channel.NetworkUrl;
                    oldChannel.Credentials = channel.Credentials;
                    oldChannel.Status = StatusEnum.Active.Value();
                    if (oldChannel.Status.Equals(StatusEnum.Deleted.Value()))
                    {
                        oldChannel.Name = channel.Name;
                        oldChannel.Description = channel.Description;
                        oldChannel.ProfileImageUrl = channel.ProfileImageUrl;
                        oldChannel.CreationDate = channel.CreationDate;
                        oldChannel.Settings = channel.Settings;
                    }

                    Instances.Repositories.ChannelRepository.Update(oldChannel);
                    channel.Id = oldChannel.Id;
                }
            }
            return channels;
        }


    }
}