using FeedHiveAuth.Data.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Telegram.Bot.Types;

namespace FeedHiveAuth.Services
{
    public static class GlobalContext
    {
        public static UserConfigs UserConfigs {  get; set; }

/*        public static 
*/        
        public static UserConfigs Construct(IdentityUser user)
        {
            UserConfigs = new UserConfigs
            {
                UserData = new UserData
                {
                    Name = user.UserName,
                    Email = user.Email,
                    Id = user.Id
                }
            };
            return UserConfigs;
        }
    }
    public class UserConfigs
    {
        public UserData UserData { get; set; }
    }
    public class UserData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string Id { get; set; }
    }
}
