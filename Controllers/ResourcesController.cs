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
        
        [HttpGet]
        [PermissionFilter("Resources_Index")]
        public ActionResult Index()
        {
            var Resources = _resourcesService.GetAll().OrderBy(r => r.Key);
            return View(Resources);
        }
        [PermissionFilter("Resources_Create")]
        public ActionResult Create()
        {
            return View("Edit");
        }

        [PermissionFilter("Resources_Edit")]
        public ActionResult Edit(Guid id)
        {
            var resource = _resourcesService.GetById(id.ToString());
            return View("Edit",resource);
        }
        public ActionResult Save(ResourceModel resourceForm)
        {
            var resource = new Models.Resources();
                resource.Value = resourceForm.Value;
                resource.Language = resourceForm.Language;
                resource.SubscriptionId = null;
                resource.Key = resourceForm.Key;
                resource.Status = StatusEnum.Active.Value();
            if(resourceForm.Id==Guid.Empty)
            {
                
                _resourcesService.SaveResource(resource);
                
            }
            else
            {
                resource.Id = resourceForm.Id.ToString();
                _resourcesService.UpdateResource(resource);
            }
              return RedirectToAction("index");
        }
    }
}
