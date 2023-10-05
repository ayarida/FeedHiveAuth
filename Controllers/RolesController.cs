using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeedHiveAuth.Controllers
{
    public class RolesController : Controller
    {
        public RolesController() { }

        [HttpPost]
        public void CreateRole()
        {
            Role role = new Role();
            role.Name = HttpContext.Request.Form["Name"];
            role.NormalizedName = HttpContext.Request.Form["NormalizedName"];
            Instances.Repositories.RoleRepository.CreateRole(role);
        }

    }
}
