using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;

namespace FeedHiveAuth.Services
{
    public class GlobalContext
    {
        public HttpContext _globalHttpContext;
        public string SubscriptionId;
        public string currentUser;

        public GlobalContext() {
        }

       
        public void SetSubscriptionId(string subId)
        {
            _globalHttpContext.Session.SetString("subscriptionId",subId);
        }

        public void SetCurrentUserId(string userId)
        {
            //this.currentUser = 
            //register new value in session
        }



    }
}
