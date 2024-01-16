using FeedHiveAuth.Areas.Identity.Pages.Account;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestSharp.Extensions;

namespace FeedHiveAuth.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRepository _userRepository;
        private readonly SignInManager<IdentityUser> signInManager;

        //get current subscription value

        public UsersController(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User model)
        {
            var customUser = new IdentityUser
            {
                UserName = model.Username,
                EmailConfirmed = true,                
            }; 
            var result = await userManager.CreateAsync(customUser,model.PasswordHash);

            if(result.Succeeded)
            {
                var getRole = model.RoleId.HasValue() ? await roleManager.FindByIdAsync(model.RoleId) : null;
                await userManager.AddToRoleAsync(customUser, getRole?.Name);

                return RedirectToAction("Index", "Home");

            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        public IActionResult Login()
        {
            return View("~/Areas/Identity/Pages/Account/Login.cshtml");
        }

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

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser { UserName = model.Input.Email, Email = model.Input.Email };
                var result = await userManager.CreateAsync(newUser, model.Input.Password);

                if (result.Succeeded)
                {
                    // You can sign in the new user if needed
                    // await _signInManager.SignInAsync(newUser, isPersistent: false);

                    // Your additional logic after successful registration
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If registration fails, redisplay the form
            return View("Register", model);
        }

        [HttpGet]
        public IActionResult List()
        {
            //TODO: Update users returned based on current subscriptionId
            List<IdentityUser> users = userManager.Users.ToList();
            return View(users);
        }

        public IdentityUser GetById(string id)
        {
            IdentityUser identityUser = userManager.Users.FirstOrDefault(x => x.Id == id);
            return identityUser;
        }

        public IdentityUser GetByName(string userName)
        {
            IdentityUser identityUser = userManager.Users.FirstOrDefault(x => x.UserName.Equals(userName));
            return identityUser;
        }

        [HttpPost]
        public void Delete(string id)
        {
            /*    var subscriptionId =
                var currentUser = await _adminWorkContext.GetCurrentUserAsync();*/
            Instances.Repositories.UserRepository.Delete(id);
        }

        public void MultipleDelete([FromForm] string userList)
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



        /* public async Task<List<User>> GetUsersAsync()
         {
             using (var context = new ApplicationDbContext())
             {
                 return await context.Users.ToList();
             }
         }*/



    }
}
