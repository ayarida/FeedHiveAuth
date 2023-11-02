using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;

namespace FeedHiveAuth.Services
{
    public static class GlobalContext
    {
        public static HttpContext _globalHttpContext;
        public static string SubscriptionId;
        public static string currentUser;
       
        public static Dictionary<string, string> Application = new Dictionary<string, string>();
        public static string GetCurrentSubscription()
        {
            return Application["currentSubscription"];
        }

        public static string GetCurrentUser()
        {
            return Application["currentUser"];
        }

    }
}
