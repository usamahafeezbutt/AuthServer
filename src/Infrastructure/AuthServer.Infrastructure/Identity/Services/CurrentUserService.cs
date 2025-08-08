using AuthServer.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Portofolio.Infrastructure.Identity.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string Identifier => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public Guid UserId => !string.IsNullOrWhiteSpace(Identifier) ?
            Guid.TryParse(Identifier, out var id) ? id :Guid.Empty : Guid.Empty;
    }
}