using FeedHiveAuth.Areas.Identity.Pages.Account;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp.Extensions;
using System.Data;
using System.Security.Claims;

namespace FeedHiveAuth.Controllers
{
    public class UsersController : BaseController<UsersController>
    {
        public UsersController(UserManager<IdentityUser> userManager, ILogger<UsersController> logger, RoleManager<IdentityRole> roleManager) : base(userManager, logger,roleManager)
        {
        }
        public UserRepository _userService = Instances.Repositories.UserRepository;
        public RoleRepository _roleService = Instances.Repositories.RoleRepository;
        
        [PermissionFilter("Users_Create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var availableRoles = allRoles();
            
            //if master : show only admins and whos parentId is this master
            //else: assign current admin as parentId
            var user = await getIdentityUser();
            var isAdmin = this.isAdmin();           
            var isInMasterRole = this.isMaster();
            var createUserVM = new AddUserViewModel
            {
                User = user,
                IsAdmin = isAdmin,
            };
            if (isInMasterRole)
            {
                var withAdminRoleAndMasterMembers = await getAdminUsers();
                createUserVM.ParentUsers = withAdminRoleAndMasterMembers.ToList();
                createUserVM.Roles = availableRoles.ToList();
            }
            else
            {
                createUserVM.Roles = availableRoles.Where(r => r.Name.ToLower() != "admin" && r.Name.ToLower() != "master").ToList();            
            }
            return View(createUserVM);
        }

        [PermissionFilter("Users_Edit")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var currentuser = _userService.GetById(id);
            var availableRoles = allRoles();
            //ViewData["availableRoles"] = availableRoles;
            var model = new ApplicationUser();
            //var user = userManager.Users.FirstOrDefault(x => x.Id == id);
            model.UserName = currentuser.UserName;
            model.PasswordHash = currentuser.PasswordHash;
            model.Email = currentuser.Email;
            model.RoleId = _roleService.GetUserRole(id);
            model.Status = currentuser.Status;
            model.Id= id;
            return View(model);
        }
        [PermissionFilter("Users_SaveUser")]
        [HttpPost]
        public async Task<IActionResult> SaveUser(ApplicationUser model)
        {
            var currentuser = await getUserById(model.Id);
            currentuser.UserName = model.UserName;
            currentuser.Email = model.Email;
            currentuser.EmailConfirmed = true;
            currentuser.NormalizedUserName = model.UserName.ToUpper();
            currentuser.NormalizedEmail = model.Email.ToUpper();
            var result = await userUpdateAsync(currentuser);
            var getRole = model.RoleId.HasValue() ? await getRoleById(model.RoleId) : await getRoleByName("Master");
            if (result.Succeeded && result.Errors.Count() == 0 && getRole != null)
            {
                var oldroleid = _roleService.GetUserRole(model.Id) != null ? _roleService.GetUserRole(model.Id) : getRole.Id;
                var oldrole = (await getRoleById(oldroleid)).Name;
                var resultdeleted = await removeUserFromRoleAsync(currentuser, oldrole);
                var assignRole = await addUserToRole(currentuser, getRole.Name);
                if (assignRole.Succeeded)
                {
                    return RedirectToAction("ListTree", "Users");
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(error.Description);
            }
            return RedirectToAction("ListTree", "Users");
        }

        [PermissionFilter("Users_RegisterNewUser")]
        [HttpPost]
        public async Task<IActionResult> RegisterNewUser(ApplicationUser model)
        {
            var customUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true,
                PasswordHash = model.PasswordHash,
                //ParentId = model.ParentId
            };
            var currentuser = GetCurrentUserId();
            var result = await userCreateAsync(customUser, model.PasswordHash);
            var getRole = model.RoleId.HasValue() ? await getRoleById(model.RoleId) : await getRoleByName("Editor");
            if (result.Succeeded && result.Errors.Count() == 0)
            {
                var assignRole = await addUserToRole(customUser, getRole.Name);
                if (model.ParentId != null) { //Master adding a editor
                    _userService.AssignParentToUser(model.ParentId, GetByName(model.UserName).Id);
                }
                else { //Adding an editor
                    _userService.AssignParentToUser(currentuser.Result, GetByName(model.UserName).Id);
                }

                if (assignRole.Succeeded)
                {
                    return RedirectToAction("ListTree", "Users");
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(error.Description);
            }
            return RedirectToAction("ListTree", "Users");
        }
        
        [PermissionFilter("Users_Login")]
        public IActionResult Login()
        {
            return View("~/Areas/Identity/Pages/Account/Login.cshtml");
        }
        [PermissionFilter("Users_Register")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return RedirectToAction("Register", "Account", new { area = "Identity" });
        }

/*        [PermissionFilter("Users_List")]
        [HttpGet]
        public IActionResult List()
        {
            //TODO: Update users returned based on current subscriptionId
            List<IdentityUser> users = allUsers();
            return View(users);
        }*/
        [PermissionFilter("Users_ListTree")]
        [HttpGet]
        public IActionResult ListTree()
        {
            var users = allUsers();
            var userList = new List<UsersWithRolesViewModel>();
           
            if(isMaster())
            {
                var MasterViewModel = new UsersWithRolesViewModel();
                MasterViewModel.user = _userService.GetByUsername(User.Identity.Name);
                MasterViewModel.role = "Master";//add master to list
                userList.Add(MasterViewModel);
                var admins = _userService.GetWhosParentId(allUsers().FirstOrDefault(x => x.UserName.Equals(User.Identity.Name)).Id);
                
                if(admins != null)
                {
                    //get user by id admin
                    foreach(var  admin in admins)
                    {
                        //add admins to list
                        var AdminViewModel = new UsersWithRolesViewModel();
                        AdminViewModel.user = _userService.GetById(admin);
                        AdminViewModel.role = "Admin";
                        userList.Add(AdminViewModel);
                        var childs = _userService.GetWhosParentId(AdminViewModel.user.Id);
                        if(childs != null)
                        {
                            foreach(var child in childs)
                            {
                                var userViewModel=new UsersWithRolesViewModel();
                                userViewModel.user= _userService.GetById(child);
                                userViewModel.role = "Editor";
                                userViewModel.ParentId=AdminViewModel.user.Id;
                                userList.Add(userViewModel);
                            }
                        }
                    }
                    
                }
            }
            else if(isAdmin())
            {
                
                var AdminViewModel = new UsersWithRolesViewModel();
                AdminViewModel.user = _userService.GetByUsername(User.Identity.Name);
                AdminViewModel.role = "Admin";
                userList.Add(AdminViewModel);
                var childs = _userService.GetWhosParentId(AdminViewModel.user.Id);
                if (childs != null)
                {
                    foreach (var child in childs)
                    {
                        var userViewModel = new UsersWithRolesViewModel();
                        userViewModel.user = _userService.GetById(child);
                        userViewModel.role = "Editor";
                        userViewModel.ParentId = AdminViewModel.user.Id;
                        userList.Add(userViewModel);
                    }
                }
            }
            return View(userList);
        }
        [PermissionFilter("Users_GetById")]
        public IdentityUser GetById(string id)
        {
            IdentityUser identityUser = allUsers().FirstOrDefault(x => x.Id == id);
            return identityUser;
        }
        [PermissionFilter("Users_GetByName")]
        public IdentityUser GetByName(string userName)
        {
            IdentityUser identityUser = allUsers().FirstOrDefault(x => x.UserName.Equals(userName));
            return identityUser;
        }
        [PermissionFilter("Users_Deactivate")]
        [HttpGet]
        public IActionResult Deactivate(string id)
        {
            _userService.ChangeStatus(id, false);

            return RedirectToAction("Index");
        }
        [PermissionFilter("Users_Delete")]
        [HttpGet]
        public void Delete(string id)
        {
            _userService.ChangeStatus(id, false);
            _userService.Delete(id);
        }
        [PermissionFilter("Users_MultipleDelete")]
        public void MultipleDelete([FromQuery] string userList)
        {


            Guid[] idsArray = userList?.Split(',').Select(Guid.Parse).ToArray() ?? Array.Empty<Guid>();
            List<string> stringList = new List<string>();
            foreach (Guid guid in idsArray)
            {
                stringList.Add(guid.ToString());
            }


            foreach (var user in stringList)
            {
                try
                {
                    Delete(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while deleting this post", user);
                }
            }

        }

        public void getParentUsers()
        {
            var users = _userService.GetAll();
        }

        public async Task<string> GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

    }
}
