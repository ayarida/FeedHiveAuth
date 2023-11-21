using FeedHiveAuth.Data;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
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

        public void Delete(string id)
        {
            //var subscriptionId =
            //var currentUser = await _adminWorkContext.GetCurrentUserAsync();
            Instances.Repositories.UserRepository.Delete("9372960b-9d59-47c9-a7e5-de38bfb4aa38");
        }


        [HttpGet]
        public IActionResult UsersList()
        {

            List<IdentityUser> users = userManager.Users.ToList();
            return View("~/Views/Users/UsersList.cshtml", users);

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
