using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FeedHiveAuth.Data.Helpers
{
    public static class PermissionHelper
    {
        private static Dictionary<string, Tuple<IEnumerable<string>, string>> _cachedControllers;

        public static Dictionary<string, Tuple<IEnumerable<string>, string>> Controllers()
        {
            if (_cachedControllers != null)
                return _cachedControllers;

            var assembly = Assembly.GetExecutingAssembly();
            var controllersToExclude = new List<string> { "HomeController", "PageResult", "SubscriptionsController", "BaseController`1", "OrganizationController" };
            var controllers = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Controller)) && !controllersToExclude.Contains(type.Name)).ToList();

            //return controllers.Select(controller => new 
            //{ 
            //Controller = controller, 
            //Actions = Actions(controller)

            //}).ToDictionary(t=>t.Controller.Name.Replace("Controller",""), t=>Tuple.Create(t.Actions,t.Controller.Name));

            _cachedControllers = controllers
            .Select(controller => new
            {
                Controller = controller,
                Actions = Actions(controller)
            })
            .ToDictionary(
                t => t.Controller.Name.Replace("Controller", ""),
                t => Tuple.Create(t.Actions, t.Controller.Name)
            );

            return _cachedControllers;
        }
        public static IEnumerable<string> Actions(Type controller)
        {
            return controller.GetMethods()
                .Where(method => method.IsPublic && method.DeclaringType == controller && !method.IsDefined(typeof(NonActionAttribute)))
                .Select(method => method.Name).Distinct();
        }
    }
}
