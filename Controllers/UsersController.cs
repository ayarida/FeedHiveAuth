using FeedHiveAuth.Areas.Identity.Pages.Account;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestSharp.Extensions;
using System.Security.Claims;

namespace FeedHiveAuth.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public UserRepository _userService = Instances.Repositories.UserRepository;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;

        }
        [PermissionFilter("Users_Create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var availableRoles = roleManager.Roles.ToList();
            var role = await roleManager.FindByNameAsync("Master Subscription");
            var usersInRole = (role != null) ? await userManager.GetUsersInRoleAsync(role.Name) : null;
            var createUserVM = new AddUserViewModel
            {
                Roles = availableRoles,
                ParentUsers = usersInRole
            };
            return View(createUserVM);
        }

        [PermissionFilter("Users_Edit")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var availableRoles = roleManager.Roles.ToList();
            //ViewData["availableRoles"] = availableRoles;
            var model = new ApplicationUser();
            var user = userManager.Users.FirstOrDefault(x => x.Id == id);
            model.UserName = user.UserName;
            model.PasswordHash = user.PasswordHash;
            model.Email = user.Email;
            model.RoleId = Instances.Repositories.RoleRepository.GetUserRole(id);
            return View(model);
        }
        [PermissionFilter("Users_SaveUser")]
        [HttpPost]
        public async Task<IActionResult> SaveUser(ApplicationUser model)
        {
            var currentuser = await userManager.FindByIdAsync(model.Id);
            currentuser.UserName = model.UserName;
            currentuser.Email = model.Email;
            currentuser.EmailConfirmed = true;

            currentuser.NormalizedUserName = model.UserName.ToUpper();
            currentuser.NormalizedEmail = model.Email.ToUpper();

            var result = await userManager.UpdateAsync(currentuser);
            var getRole = model.RoleId.HasValue() ? await roleManager.FindByIdAsync(model.RoleId) : await roleManager.FindByNameAsync("Editor");
            if (result.Succeeded && result.Errors.Count() == 0 && getRole != null)
            {
                var oldroleid = Instances.Repositories.RoleRepository.GetUserRole(model.Id) != null ? Instances.Repositories.RoleRepository.GetUserRole(model.Id) : getRole.Id;
                var oldrole = (await roleManager.FindByIdAsync(oldroleid)).Name;
                var resultdeleted = await userManager.RemoveFromRoleAsync(currentuser, oldrole);
                var assignRole = await userManager.AddToRoleAsync(currentuser, getRole.Name);
                if (assignRole.Succeeded)
                {
                    return RedirectToAction("List", "Users");
                }
            }
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, error.Description);
            //    return BadRequest(error.Description);
            //}
            return RedirectToAction("List", "Users");
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
            var result = await userManager.CreateAsync(customUser, model.PasswordHash);
            var getRole = model.RoleId.HasValue() ? await roleManager.FindByIdAsync(model.RoleId) : await roleManager.FindByNameAsync("Editor");
            if (result.Succeeded && result.Errors.Count() == 0)
            {

                var assignRole = await userManager.AddToRoleAsync(customUser, getRole.Name);
                var assignParent = _userService.AssignParentToUser(model.ParentId, GetByName(model.UserName).Id);
                if (assignRole.Succeeded)
                {
                    return RedirectToAction("List", "Users");
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(error.Description);
            }
            return RedirectToAction("List", "Users");
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

            // Your existing login logic
            return RedirectToAction("Register", "Account", new { area = "Identity" });
        }

        /* [HttpGet]
         public async Task<IActionResult> Register(LoginModel model)
         {
             // Your existing login logic

             return RedirectToAction("Index", "Home");
         }*/
        //[PermissionFilter("Users_RegisterNewUser")]
        //[HttpPost]
        //public async Task<IActionResult> RegisterNewUser(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var newUser = new IdentityUser { UserName = model.Input.Email, Email = model.Input.Email };
        //        var result = await userManager.CreateAsync(newUser, model.Input.Password);

        //        if (result.Succeeded)
        //        {
        //            // You can sign in the new user if needed
        //            // await _signInManager.SignInAsync(newUser, isPersistent: false);

        //            // Your additional logic after successful registration
        //            return RedirectToAction("Index", "Home");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    // If registration fails, redisplay the form
        //    return View("Register", model);
        //}
        [PermissionFilter("Users_List")]
        [HttpGet]
        public IActionResult List()
        {
            //TODO: Update users returned based on current subscriptionId
            List<IdentityUser> users = userManager.Users.ToList();
            return View(users);
        }
        [PermissionFilter("Users_GetById")]
        public IdentityUser GetById(string id)
        {
            IdentityUser identityUser = userManager.Users.FirstOrDefault(x => x.Id == id);
            return identityUser;
        }
        [PermissionFilter("Users_GetByName")]
        public IdentityUser GetByName(string userName)
        {
            IdentityUser identityUser = userManager.Users.FirstOrDefault(x => x.UserName.Equals(userName));
            return identityUser;
        }
        [PermissionFilter("Users_Delete")]
        [HttpPost]
        public void Delete(string id)
        {
            /*    var subscriptionId =
                var currentUser = await _adminWorkContext.GetCurrentUserAsync();*/
            Instances.Repositories.UserRepository.Delete(id);
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

        public void getParentUsers(string parentId)
        {

        }
    }
}
