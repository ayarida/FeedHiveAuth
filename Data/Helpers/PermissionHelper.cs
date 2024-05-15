using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FeedHiveAuth.Data.Helpers
{
    public static class PermissionHelper
    {
        public static Dictionary<string, Tuple<IEnumerable<string>, string>> Controllers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var controllersToExclude = new List<string> {"HomeController", "PageResult", "SubscriptionsController" };
            var controllers = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Controller)) && !controllersToExclude.Contains(type.Name)).ToList();

            return controllers.Select(controller => new 
            { 
            Controller = controller, 
            Actions = Actions(controller)
            
            }).ToDictionary(t=>t.Controller.Name.Replace("Controller",""), t=>Tuple.Create(t.Actions,t.Controller.Name));
        }
        public static IEnumerable<string> Actions(Type controller)
        {
            return controller.GetMethods()
                .Where(method=>method.IsPublic && method.DeclaringType == controller && !method.IsDefined(typeof(NonActionAttribute)))
                .Select(method => method.Name).Distinct();
        }
    }
}
