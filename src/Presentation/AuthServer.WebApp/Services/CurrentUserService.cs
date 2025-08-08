using AuthServer.Application.Common.Interfaces.Identity;
using System.Security.Claims;

namespace AuthServer.Infrastructure.Identity.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string Identifier => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        public Guid UserId => !string.IsNullOrWhiteSpace(Identifier) ? new Guid(Identifier) : Guid.Empty;
    }
}