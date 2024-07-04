using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using System.Security.Claims;

namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue("jti") ?? "Unknown";
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User? .FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        }
    }
}
