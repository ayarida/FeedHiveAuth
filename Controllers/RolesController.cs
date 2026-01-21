using FeedHiveAuth.Areas.Social.Controllers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace FeedHiveAuth.Controllers
{

    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        private readonly ILogger<RolesController> _logger;

        protected RoleRepository roleRepository = Instances.Repositories.RoleRepository;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RolesController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._logger = logger;
        }

        public async Task<Role> GetRoleById(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            var roleClaims = roleRepository.GetUserClaims(roleId);
            if (role == null)
            {
                return null;
            }
            return new Role
            {
                Id = roleId,
                Name = role.Name,
                Permissions = MapPermission(roleClaims.ToList()),
                Description = roleRepository.getRoleDescription(roleId)

            };
        }
        private List<RolePermission> MapPermission(List<string> permissions)
        {
            var list = new List<RolePermission>();
            foreach(var permission in permissions)
            {
                var x = new RolePermission();
                x.Controller = permission.Split('_')[0];
                x.Action = permission.Split('_')[1];
                list.Add(x);    
            }
            return list;
        }
        [HttpPost]
        public async Task<IActionResult> SavePermissions([FromBody] RolePermissionDto roleData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await roleManager.FindByIdAsync(roleData.Id);
            if (role == null)
                return NotFound();

            // Update role name if changed
            if (role.Name != roleData.Name)
            {
                role.Name = roleData.Name;
                var updateResult = await roleManager.UpdateAsync(role);
                if (!updateResult.Succeeded)
                    return BadRequest(updateResult.Errors);
            }

            // Build NEW permission set
            var newPermissions = roleData.Permissions
                .Select(p => $"Permission:{p.Controller}_{p.Action}")
                .ToHashSet();

            // Get CURRENT claims
            var currentClaims = await roleManager.GetClaimsAsync(role);
            var currentPermissions = currentClaims
                .Where(c => c.Type == "Permission")
                .Select(c => $"{c.Type}:{c.Value}")
                .ToHashSet();

            // DIFF calculation (FAST)
            var permissionsToAdd = newPermissions.Except(currentPermissions);
            var permissionsToRemove = currentPermissions.Except(newPermissions);

            // Remove only removed permissions
            foreach (var perm in permissionsToRemove)
            {
                var value = perm.Split('_')[1];
                await roleManager.RemoveClaimAsync(role, new Claim("Permission", value));
            }

            // Add only new permissions
            foreach (var perm in permissionsToAdd)
            {
                var value = perm.Split(':')[1];
                await roleManager.AddClaimAsync(role, new Claim("Permission", value));
            }

            // Save description (non-identity)
            if (!string.IsNullOrWhiteSpace(roleData.Description))
            {
                roleRepository.saveRoleDescription(roleData.Id, roleData.Description);
            }

            return Ok(new { success = true });
        }

        [PermissionFilter("Roles_Update")]
        public async Task<IActionResult> Update(string id)
        {
            Role role = await GetRoleById(id);
            List<ApplicationUser> members = new List<ApplicationUser>();
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();
            foreach (ApplicationUser user in userManager.Users)
            {
                var checkIfHasRole = await userManager.GetRolesAsync(user);
                if (checkIfHasRole.Any())
                {
                    var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                    list.Add(user);
                }        
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }
        [PermissionFilter("Roles_Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            Role role = await GetRoleById(id);
            var permissionSet = role.Permissions.Select(p => $"{p.Controller}:{p.Action}")
        .ToHashSet();
            //List<ApplicationUser> members = new List<ApplicationUser>();
            //List<ApplicationUser> nonMembers = new List<ApplicationUser>();

            //foreach (ApplicationUser user in userManager.Users)
            //{
            //    var checkIfHasRole = await userManager.GetRolesAsync(user);
            //    if (checkIfHasRole.Any())
            //    {
            //        var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
            //        list.Add(user);
            //    }

            //}
            return View(new RoleEdit
            {
                Role = role,
                PermissionSet = permissionSet
            });
        }
        [PermissionFilter("Roles_Update")]
        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    ApplicationUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    ApplicationUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return await Update(model.RoleId);
        }

        [PermissionFilter("Roles_Index")]
        public ViewResult Index() {
            var identityRoles = roleManager.Roles;
            var roles = new List<Role>();
            foreach(var role in identityRoles)
            {
                roles.Add(new Role
                {
                    Id = role.Id,
                    Name = role.Name,
                    NormalizedName = role.NormalizedName,
                    Description = roleRepository.getRoleDescription(role.Id)
                });
            }
            
            return View(roles); }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
        [PermissionFilter("Roles_Create")]
        public IActionResult Create() => View();


        [PermissionFilter("Roles_GetIdentityRole")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IdentityRole>> GetIdentityRole(string id)
        {
            var identityRole = await roleManager.FindByIdAsync(id);

            if (identityRole == null)
            {
                return NotFound();
            }

            return identityRole;
        }
       [PermissionFilter("Roles_Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                try { 
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
                }catch(Exception ex)
                {
                    _logger.LogError(ex, "********************* Error occurred while deleting role. *********************");
                    ModelState.AddModelError("", "An error occurred while deleting the role.");
                }
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Index", roleManager.Roles);
        }

    }
}
