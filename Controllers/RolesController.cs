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
        private UserManager<IdentityUser> userManager;
        private readonly ILogger<RolesController> _logger;

        protected RoleRepository roleRepository = Instances.Repositories.RoleRepository;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ILogger<RolesController> logger)
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
        public async Task<IActionResult> SavePermissions([FromBody] Role roleData)
        {
            IdentityRole roleToEdit = await roleManager.FindByIdAsync(roleData.Id.ToString());
            var curr = await userManager.GetUserAsync(User);
            var role = new Role
            {
                Name = roleData.Name,
                Description = roleData.Description,
            };
            if (roleToEdit == null) // new role
            {
                if (ModelState.IsValid)
                {
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        var permissions = roleData.Permissions
                            .Select(p => new Permission { Name = $"{p.Controller}_{p.Action}" })
                            .ToList();

                        foreach (var permission in permissions)
                        {

                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission.Name));
                            //await userManager.AddClaimAsync(curr, new System.Security.Claims.Claim("Permission", permission.Name));
                        }
                        if (roleData.Description != null)
                        {
                            roleRepository.saveRoleDescription(roleData.Id, roleData.Description);
                        }
                        return Ok(); // or return a specific result based on your needs
                    }
                    else
                    {
                        // Handle role creation failure
                        return BadRequest(result.Errors);
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }//end if new
            else //update role
            {
                if (roleToEdit.Name != roleData.Name)
                {
                    roleToEdit.Name = roleData.Name;
                }

                IdentityResult result = await roleManager.UpdateAsync(roleToEdit);
                if (result.Succeeded)
                {
                    var permissions = roleData.Permissions
                           .Select(p => new Permission { Name = $"{p.Controller}_{p.Action}" })
                           .ToList(); //list of new permissions
                    //////////remove old permission
                    var oldpermissions = await roleManager.GetClaimsAsync(roleToEdit);
                    foreach(var oldclaim in oldpermissions)
                    {
                        var resultdelete = await roleManager.RemoveClaimAsync(roleToEdit,oldclaim);
                    }
                    ///
                    
                    foreach (var permission in permissions)
                    {
                        await roleManager.AddClaimAsync(roleToEdit, new System.Security.Claims.Claim("Permission", permission.Name));   
                    }
                    if (roleData.Description != null)
                    {
                        roleRepository.saveRoleDescription(roleData.Id, roleData.Description);
                    }

                    return RedirectToAction("Index", "Home", new { Id = roleData.Id });
                }
                return BadRequest(ModelState);

            }
            
        }
        [PermissionFilter("Roles_Update")]
        public async Task<IActionResult> Update(string id)
        {
            Role role = await GetRoleById(id);
            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();
            foreach (IdentityUser user in userManager.Users)
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
            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();
            foreach (IdentityUser user in userManager.Users)
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
        [PermissionFilter("Roles_Update")]
        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
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
