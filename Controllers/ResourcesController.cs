using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Data;
using Microsoft.AspNetCore.Mvc;
using Humanizer.Localisation;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Controllers
{
    public class ResourcesController : Controller
    {
        protected ResourcesRepository _resourcesService = Instances.Repositories.ResourcesRepository;
        public ActionResult Index()
        {
            var Resources = _resourcesService.GetAll();
            return View(Resources);
        }
        public ActionResult Create()
        {
            return View("Edit");
        }
        public ActionResult Save(ResourceModel resourceForm)
        {
            var resource = new Models.Resources();
            resource.Value = resourceForm.Value;
            resource.Language = resourceForm.Language;
            resource.SubscriptionId = null;
            resource.Key = resourceForm.Key;
            resource.Status = StatusEnum.Active.Value();
            _resourcesService.SaveResource(resource);
            return RedirectToAction ("index");
        }
    }
}
