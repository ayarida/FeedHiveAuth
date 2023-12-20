using System.Security.Claims;

namespace FeedHiveAuth.Models
{
    public interface IUserService
    {
        string GetCurrentUserId();
        string GetCurrentUserName();
    }
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User.Identity.Name;
        }
    }
}
