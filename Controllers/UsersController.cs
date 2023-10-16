using FeedHiveAuth.Data;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public void Delete(string id)
        {
            //var subscriptionId =
            //var currentUser = await _adminWorkContext.GetCurrentUserAsync();
            Instances.Repositories.UserRepository.Delete("9372960b-9d59-47c9-a7e5-de38bfb4aa38");
        }
    }
}
