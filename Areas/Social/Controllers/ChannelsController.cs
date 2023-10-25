using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Areas.Social.Controllers
{
    [Area("Social")]
    public class ChannelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChannelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Social/Channels
        public async Task<IActionResult> Index()
        {
              return _context.Channel != null ? 
                          View(await _context.Channel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Channel'  is null.");
        }

        // GET: Social/Channels/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Social/Channels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
        [HttpPost, ActionName("Delete")]
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
    }
}
