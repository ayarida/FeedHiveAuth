using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedHiveAuth.Controllers
{
    public class OrganizationController : BaseController<OrganizationController>
    {
        protected OrganizationRepository _organizationService = Instances.Repositories.OrganizationRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<OrganizationController> _logger;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> _userManager;

        public OrganizationController(UserManager<ApplicationUser> userManager, ILogger<OrganizationController> logger)
            : base(userManager, logger)
        {
            _logger = logger;
            _userManager = userManager;
        }
        //////////////////////////////// SUPER ADMIN ////////////////////////////////

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [PermissionFilter("Organization_Index")]

        public async Task<IActionResult> Index()
        {
            var organizations = _organizationService.GetAll().Where(o=> !o.IsDeleted);
            var model = new List<OrganizationViewModel>();
            foreach (var org in organizations)
            {
                var users = _userService.FindByOrgId(org.Id);

                model.Add(new OrganizationViewModel
                {
                    Id = org.Id,
                    Name = org.Name,
                    CreationDate = org.CreationDate,
                    Status = org.IsActive ? "Active" : "Inactive",
                    OrgAdminsCount = users.Count(u => _userManager.IsInRoleAsync(u, "OrgAdmin").Result),
                    EditorsCount = users.Count(u => _userManager.IsInRoleAsync(u, "Editor").Result)
                });
            }

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [PermissionFilter("Organization_Create")]
        public IActionResult Create()
        {
            return View(new OrganizationCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrganizationCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var organization = new Organization
                {
                    Name = model.Name,
                    SubscriptionType = model.SubscriptionType,
                    IsActive = model.IsActive,
                    Description = model.Description,
                    CreationDate = DateTime.UtcNow
                };

                _organizationService.Save(organization);
                TempData["Success"] = "Organization created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating organization: {ex}");
                ModelState.AddModelError("", "Error saving organization");
                return View(model);
            }
        }


        [HttpGet]
        [PermissionFilter("Organization_Edit")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(string id)
        {
            var org = _organizationService.GetById(id);
            if (org == null)
                return NotFound();

            var model = new OrganizationEditViewModel
            {
                Id = org.Id,
                Name = org.Name,
                IsActive = org.IsActive
            };

            return View(model);
        }
        [HttpPost]
        [PermissionFilter("Organization_Update")]
        public async Task<IActionResult> Update(OrganizationEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var org =  _organizationService.GetById(model.Id);
            if (org == null)
                return NotFound();

            org.Name = model.Name;
            org.IsActive = model.IsActive;

            _organizationService.Update(org);
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[PermissionFilter("Organization_Edit")]
        //public IActionResult Edit(Organization model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    try
        //    {
        //        _organizationService.UpdateOrganization(model);
        //        TempData["Success"] = "Organization updated successfully!";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error updating organization: {ex}");
        //        ModelState.AddModelError("", "Error updating organization");
        //        return View(model);
        //    }
        //}


        [Authorize(Roles = "SuperAdmin")]
        [PermissionFilter("Organization_OrgDetails")]
        public async Task<IActionResult> OrgDetails(string orgId)
        {
            var org = _organizationService.GetById(orgId);
            if (org == null)
                return NotFound();
            var users = _userService.FindByOrgId(orgId);
            var orgAdmins = new List<UserViewModel>();
            var editors = new List<UserViewModel>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "OrgAdmin"))
                {
                    orgAdmins.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.UserName,
                        Email = user.Email
                    });
                }

                if (await _userManager.IsInRoleAsync(user, "Editor"))
                {
                    editors.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.UserName,
                        Email = user.Email,
                        ParentId = user.ParentId  // this links editor → orgadmin
                    });
                }
            }

            var model = new OrgDetailsViewModel
            {
                OrganizationName = org.Name,
                OrgAdmins = orgAdmins,
                Editors = editors
            };

            return View(model);
        }

        //[HttpGet]
        //[PermissionFilter("Organization_Delete")]
        //public IActionResult Delete(string id)
        //{
        //    var org = _organizationService.GetById(id);
        //    if (org == null)
        //        return NotFound();

        //    return View(org);
        //}


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter("Organization_DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var result = _organizationService
                .TryDeleteOrganization(id, currentUser.Id);

            if (!result.IsSuccess)
            {
                TempData["SwalError"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SwalSuccess"] = "Organization deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [PermissionFilter("Organization_ListAdmins")]
        public async Task<IActionResult> ListAdmins(string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                return RedirectToAction("Index");

            var org = _organizationService.GetById(orgId);
            if (org == null)
                return NotFound();

            ViewBag.Organization = org;

            // get all users in OrgAdmin role
            var allAdmins = await _userManager.GetUsersInRoleAsync("OrgAdmin");

            // filter by this organization
            var admins = allAdmins.Where(x => x.OrganizationId == orgId).ToList();

            return View(admins);
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [PermissionFilter("Organization_CreateAdmin")]
        public IActionResult CreateAdmin(string orgId)
        {
            var org = _organizationService.GetById(orgId);
            if (org == null)
                return NotFound();

            var model = new CreateOrgAdminViewModel
            {
                OrganizationId = orgId
            };

            ViewBag.Organization = org;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAdmin(CreateOrgAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Organization = _organizationService.GetById(model.OrganizationId);
                return View(model);
            }

            var org = _organizationService.GetById(model.OrganizationId);
            if (org == null)
                return NotFound();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    OrganizationId = model.OrganizationId,
                    //IsActive = model.IsActive
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    ViewBag.Organization = org;
                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, "OrgAdmin");

                TempData["Success"] = "Organization Admin created successfully!";
                return RedirectToAction("ListAdmins", new { orgId = model.OrganizationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating organization admin");
                ModelState.AddModelError("", "An error occurred while creating admin.");

                ViewBag.Organization = org;
                return View(model);
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [PermissionFilter("Organization_CreateOrgAdmin")]

        public IActionResult CreateOrgAdmin()
        {
            var orgs = _organizationService.GetAll();
            ViewBag.Organizations = orgs;
            return View(new CreateOrgAdminViewModel {} );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter("Organization_CreateOrgAdmin")]
        public async Task<IActionResult> CreateOrgAdmin(CreateOrgAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Organizations = _organizationService.GetAll();
                return View(model);
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    OrganizationId = model.OrganizationId,
                    //isActive = model.isActive
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    ViewBag.Organizations = _organizationService.GetAll();
                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, "OrgAdmin");

                TempData["Success"] = "Organization Admin created successfully!";
                return RedirectToAction("ListAdmins", new {orgId = model.OrganizationId});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the organization admin.");
                ViewBag.Organizations = _organizationService.GetAll();
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "OrgAdmin")]
        [PermissionFilter("Organization_ListEditors")]
        public IActionResult ListEditors()
        {
            var orgAdmin = _userManager.GetUserAsync(User).Result;

            var editors = _userManager.Users
                .Where(x => x.OrganizationId == orgAdmin.OrganizationId && x.ParentId == orgAdmin.Id)
                .ToList();

            ViewBag.OrganizationName = _organizationService.GetById(orgAdmin.OrganizationId)?.Name;

            return View(editors);
        }

        [HttpGet]
        [Authorize(Roles = "OrgAdmin")]
        [PermissionFilter("Organization_CreateEditor")]
        public IActionResult CreateEditor()
        
        {
            var orgAdmin = _userManager.GetUserAsync(User).Result;

            var model = new CreateEditorViewModel
            {
                OrganizationId = orgAdmin.OrganizationId,
                ParentId = orgAdmin.Id
            };

            ViewBag.OrganizationName = _organizationService.GetById(orgAdmin.OrganizationId)?.Name;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter("Organization_CreateEditor")]
        public async Task<IActionResult> CreateEditor(CreateEditorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    OrganizationId = model.OrganizationId,
                    ParentId = model.ParentId,
                    //IsActive = model.IsActive
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, "Editor");

                TempData["Success"] = "Editor created successfully!";
                return RedirectToAction("ListEditors");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating editor");
                ModelState.AddModelError("", "An error occurred while creating the editor.");
                return View(model);
            }
        }


        

    }
}
