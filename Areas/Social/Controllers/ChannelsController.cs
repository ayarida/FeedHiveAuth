using FeedHiveAuth.Controllers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class ChannelsController : BaseController<ChannelsController>
    {
        private readonly ApplicationDbContext _context;
        protected ChannelRepository _channelRepository = Instances.Repositories.ChannelRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<ChannelsController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public ChannelsController(UserManager<IdentityUser> userManager, ILogger<ChannelsController> logger) : base(userManager , logger)
        {
        }

        // GET: Social/Channels
        [HttpGet]
        [PermissionFilter("Channels_Index")]
        public IActionResult Index()
        {
            var channels = _channelRepository.GlobalGetAll();
            var masterId = "";
            if (isAdmin())
            {
                masterId = _userService.GetUserParent(currUserId());
                channels = channels.Where(ch => ch.ParentId == masterId);
            }
            else if(isMaster()) { 
                masterId = currUserId();
                channels = channels.Where(ch=> ch.ParentId == masterId); 
            }
            return View(channels);    
        }

        // GET: Social/Channels/Details/5
        [PermissionFilter("Channels_Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Channel == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // GET: Social/Channels/Create
        [PermissionFilter("Channels_Create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpGet]
        [PermissionFilter("Channels_GetNetworks")]
        public ActionResult GetNetworks(string networkName)
        {
            var channelName = networkName;
            return PartialView("~/Areas/Social/Views/Shared/_" + channelName + ".cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter("Channels_Create")]
        public async Task<IActionResult> Create([Bind("Name,Description,Network,Account,NetworkId,Code,NetworkUrl,OriginalName,ProfileImageUrl,Credentials,Settings,Order,Configs,Id,Status,CreationDate,LastModified,SubscriptionId")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(channel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(channel);
        }

        // GET: Social/Channels/Edit/5
        [PermissionFilter("Channels_Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Channel == null)
            {
                return NotFound();
            }


            var channel = await _context.Channel.FindAsync(id);
            if (channel == null)
            {
                return NotFound();
            }
            return View(channel);
        }

        // POST: Social/Channels/Edit/5
        [HttpPost]
        [PermissionFilter("Channels_Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Description,Network,Account,NetworkId,Code,NetworkUrl,OriginalName,ProfileImageUrl,Credentials,Settings,Order,Configs,Id,Status,CreationDate,LastModified,SubscriptionId")] Channel channel)
        {
            if (id != channel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(channel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChannelExists(channel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(channel);
        }

        // GET: Social/Channels/Delete/5
        [PermissionFilter("Channels_Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Channel == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // POST: Social/Channels/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Channel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Channel'  is null.");
            }
            var channel = await _context.Channel.FindAsync(id);
            if (channel != null)
            {
                _context.Channel.Remove(channel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChannelExists(string id)
        {
            return (_context.Channel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<string> identityUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
       
    }
}
