using FeedHiveAuth.Areas.Identity.Pages.Account;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
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
        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger, RoleManager<IdentityRole> roleManager) : base(userManager, logger,roleManager)
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
            var user = await getApplicationUser();
            var isAdmin = this.isAdmin();           
            var isMaster = this.isMaster();
            var createUserVM = new AddUserViewModel
            {
                User = user,
                IsAdmin = isAdmin,
            };
            //SA can add admin/master users
            if (isSuperAdmin())
            {
                createUserVM.Roles = availableRoles.Where(r => r.Name.ToLower() == "admin" || r.Name.ToLower() == "master").ToList();
                return View("~/Views/Users/SuperAdmin/Create.cshtml",createUserVM);
            }
            else 
            {
                if (isMaster)
                {
                    var withAdminRoleAndMasterMembers = getMasterAdmins(user.Id);
                    var rolesToExclude = new List<string> { "master", "superadmin" };
                    createUserVM.ParentUsers = withAdminRoleAndMasterMembers.ToList();
                    createUserVM.Roles = availableRoles.Where(role => !rolesToExclude.ContainsIgnoreCase(role.Name)).ToList();
                }
                else
                {
                    createUserVM.Roles = availableRoles.Where(r => r.Name.ToLower() != "admin" && r.Name.ToLower() != "master").ToList();            
                }
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
            var currentuser = (ApplicationUser)await getUserById(model.Id);
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
                else {
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

        public async Task<IActionResult> RegisterNewUserSA(ApplicationUser userForm)
        {
            var currentuser = GetCurrentUserId();
            var currentUserRole = _roleService.userRole(currentuser.Result);
            var userFormRole = getRoleById(userForm.RoleId).Result;
            if (currentUserRole.Name.EqualsIgnoreCase("superadmin"))
            {
                //if superAdmin and registering Master then save it without parentId (adding new subscription)
                if (userFormRole.Name.EqualsIgnoreCase("master"))
                {
                    var result = await userCreateAsync(userForm, userForm.PasswordHash);
                    if (result.Succeeded && result.Errors.Count() == 0)
                    {
                        var assignRole = await addUserToRole(userForm, userFormRole.Name);
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
                }
                else
                {
                    //if superAdmin and registering Admin then save it with parentId as current user
                    var result = await userCreateAsync(userForm, userForm.PasswordHash);
                    if (result.Succeeded && result.Errors.Count() == 0)
                    {
                        var assignRole = await addUserToRole(userForm, userForm.RoleId);
                        if (assignRole.Succeeded)
                        {
                            _userService.AssignParentToUser(currentuser.Result, GetByName(userForm.UserName).Id);
                            return RedirectToAction("ListTree", "Users");
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return BadRequest(error.Description);
                    }
                }   
            }
            return View();
            
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
            List<ApplicationUser> users = allUsers();
            return View(users);
        }*/
        [PermissionFilter("Users_ListTree")]
        [HttpGet]
        public IActionResult ListTree()
        {
            var users = allUsers();
            var userList = new List<UsersWithRolesViewModel>();
            var masterArr = new List<ApplicationUser>();

            var subscriptions = new List<SubscriptionViewModel>();
            //if the role is superAdmin then fetch all aspnetusers db table and return the ones with master role
            if (isSuperAdmin())
            {
                foreach(var user in users)
                {
                    var uRole = _roleService.userRole(user.Id);
                    if(string.Equals(uRole.Name, "Master", StringComparison.OrdinalIgnoreCase))
                    {
                        var subscription = new SubscriptionViewModel
                        {
                            Master = user,
                            Admins = getChildren(user.Id), // Fetch Admins for this Master
                            Editors = new Dictionary<ApplicationUser, List<ApplicationUser>>() // Initialize Editors dictionary
                        };
                        //masterArr.Add(user);
                        //admins for master
                        //var admins = getChildren(user.Id);

                        //iterate each admin to return its childs
                        foreach (var admin in subscription.Admins)
                        {                            
                            var editors = getChildren(admin.Id);
                            subscription.Editors[admin] = editors;
                        }
                        subscriptions.Add(subscription);
                    }
                }
                //return View("~/Views/Users/SuperAdmin/MasterSubscriptions.cshtml", masterArr);
                return View("~/Views/Users/SuperAdmin/MasterSubscriptions.cshtml", subscriptions);
            }

            if (isMaster())
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

        //function that returns admin users that are related to a master
        public List<ApplicationUser> getChildren(string Parent)
        {
            var childs = _userService.GetUsersWhosParentId(Parent).ToList();
            if (childs.Any())
            {

            }
            return childs;
            /*var masterId = await GetCurrentUserId();
            var adminsOfMaster = _userService.GetWhosParentId(masterId).ToList(); //admins of the master
            var admins = new List<ApplicationUser>();
            foreach (var admin in adminsOfMaster)
            {
                admins.Add(_userService.GetById(admin));
            }
            return admins;*/
        }
        public List<ApplicationUser> getMasterAdmins(string masterId)
        {
            var childs = getChildren(masterId);
            var admins = new List<ApplicationUser>();
            foreach (var child in childs)
            {
                var userRole = _roleService.GetUserRole(child.Id);
                if (userRole == getRoleByName("Admin").Result.Id)
                {
                    admins.Add(child);
                }
            }
            //function that uses getchildren function, return those whose parentId is the masterId and whose rolename is "Admin"
            //return getChildren(masterId).Where(x => x.RoleId == getRoleByName("Admin").Result.Id).ToList();
            return admins;
        }

        [PermissionFilter("Users_GetById")]
        public ApplicationUser GetById(string id)
        {
            ApplicationUser ApplicationUser = allUsers().FirstOrDefault(x => x.Id == id);
            return ApplicationUser;
        }
        [PermissionFilter("Users_GetByName")]
        public ApplicationUser GetByName(string userName)
        {
            ApplicationUser ApplicationUser = allUsers().FirstOrDefault(x => x.UserName.Equals(userName));
            return ApplicationUser;
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
