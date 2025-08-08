using AuthServer.Application.Common.Models.Identity;
using AuthServer.Application.Common.Models.Responses;
using AuthServer.Domain.Entities;

namespace AuthServer.Application.Common.Interfaces.Identity
{
    public interface ITokenService
    {
        public Task<ResultResponse<AuthResponse>> GenerateUserToken(ApplicationUser user);
    }
}
