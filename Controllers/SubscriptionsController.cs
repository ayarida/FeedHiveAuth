using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models;
using FeedHiveAuth.Services;
using FeedHiveAuth.Data.Repositories;

namespace FeedHiveAuth.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        protected SubscriptionRepository _subscriptionService = Instances.Repositories.SubscriptionRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        public SubscriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        [PermissionFilter("Subscriptions_Index")]
        public async Task<IActionResult> Index()
        {
              return _context.Subscription != null ? 
                          View(await _context.Subscription.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Subscription'  is null.");
        }

        // GET: Subscriptions/Details/5
        [PermissionFilter("Subscriptions_Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Subscription == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscription
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create
        [PermissionFilter("Subscriptions_Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [PermissionFilter("Subscriptions_Create")]
        public async Task<IActionResult> Create([Bind("Code,Name,Hosts,Master,Id,Status,CreationDate")] Subscription subscription)
        {
            var ms = ModelState;
                subscription.Id = Guid.NewGuid().ToString();
                
                _context.Add(subscription);
                HttpContext.Items.Add("SubscriptionId", subscription.Id);
                HttpContext.Session.SetString("subscriptionId", subscription.Id);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            //return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        [PermissionFilter("Subscriptions_Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Subscription == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscription.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter("Subscriptions_Edit")]
        public async Task<IActionResult> Edit(string id, [Bind("Code,Name,Description,Hosts,Master,ParentId,Id,Status,CreationDate,LastModified,SubscriptionId")] Subscription subscription)
        {
            if (id != subscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.Id))
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
            return View(subscription);
        }
        [PermissionFilter("Subscriptions_Delete")]
        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Subscription == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscription
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [PermissionFilter("Subscriptions_DeleteConfirmed")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Subscription == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Subscription'  is null.");
            }
            var subscription = await _context.Subscription.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscription.Remove(subscription);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(string id)
        {
          return (_context.Subscription?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [PermissionFilter("Subscriptions_RegisterSubscriptionUser")]
        [HttpGet]
        public async Task<IActionResult> RegisterSubscriptionUser(string id)
        {
            if (_context.Subscription == null)
                return NotFound();

var subscription = await _context.Subscription
                .FirstOrDefaultAsync(m => m.Id == id);
            //ViewData["subscriptionId"] = subscription.Id;
            //var regUser = _userService.RegisterSubscriptionUser(id);\
            
            return View("~/Areas/Identity/Pages/Account/Register.cshtml",subscription);
    }

    [HttpGet]
        [PermissionFilter("Subscriptions_GetById")]
        public Subscription GetById(string id)
        {
            return _subscriptionService.Get(id);
        }
    }
}
