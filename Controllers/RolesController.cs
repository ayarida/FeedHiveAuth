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


        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<Role> GetRoleById(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            var roleClaims = Instances.Repositories.RoleRepository.GetUserClaims(roleId);
            if (role == null)
            {
                return null;
            }
            return new Role
            {
                Id = roleId,
                Name = role.Name,
                Permissions=MapPermission(roleClaims.ToList())

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
        public ViewResult Index() => View(roleManager.Roles);

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
            Role role = await GetRoleById(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Index", roleManager.Roles);
        }

        [PermissionFilter("Roles_ReadRoles")]
        [HttpGet]
        public void ReadRoles()
        {
             
        }
        [PermissionFilter("Roles_CreateRole")]
        [HttpPost]
        public void CreateRole()
        {
            Role role = new Role();
            role.Name = HttpContext.Request.Form["Name"];
            role.NormalizedName = HttpContext.Request.Form["NormalizedName"];
            //Instances.Repositories.RoleRepository.CreateRole(role);
        }


        private async Task CreateRolesandUsers(IServiceProvider serviceProvider)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Admin", "Manager", "Member" };
            IdentityResult roleResult;

            bool x = await _roleManager.RoleExistsAsync("Admin");
            if (!x)
            {
                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);

                //Here we create a Admin super user who will maintain the website                   

                var user = new IdentityUser();
                user.UserName = "default";
                user.Email = "default@default.com";
                string userPWD = "somepassword";

                IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // creating Creating Manager role     
            x = await _roleManager.RoleExistsAsync("Manager");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                await _roleManager.CreateAsync(role);
            }

            // creating Creating Employee role     
            x = await _roleManager.RoleExistsAsync("Employee");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Employee";
                await _roleManager.CreateAsync(role);
            }
        }

    }
}
