using AuthServer.Application.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Application.Common.Models.Responses;
using AuthServer.Application.Models.Identity;
using AuthServer.Domain.Entities;

namespace Portofolio.Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
        }

        public async Task<ResultResponse<AuthResponse>> GenerateUserToken(ApplicationUser user)
        =>  new(
                success: true,
                message: "Token Generated Successfull",
                result: new AuthResponse
                {
                    Token = await PrepareAuthToken(user),
                    RefreshToken = Guid.NewGuid(),
                    Email = user.Email!,
                    Expiry = DateTime.UtcNow.AddHours(_jwtSettings.Expiry)
                }
            );

        private async Task<string> PrepareAuthToken(ApplicationUser user)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = await PrepareTokenDescritpionAsync(user, key);
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<SecurityTokenDescriptor> PrepareTokenDescritpionAsync(ApplicationUser user, byte[] key)
        => new ()
            {
                Subject = await GetClaimsIdentityAsync(user),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(_jwtSettings.Expiry),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

        private async Task<ClaimsIdentity> GetClaimsIdentityAsync(ApplicationUser user)
        {
            // Here we can save some values to token.
            // For example we are storing here user id and email
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Name, user.Name!),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            ClaimsIdentity claimsIdentity = new(claims, "Token");
            
            var roles = await _userManager.GetRolesAsync(user);

            // Adding roles code
            // Roles property is string collection but you can modify Select code if it it's not
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claimsIdentity;
        }
    }
}
