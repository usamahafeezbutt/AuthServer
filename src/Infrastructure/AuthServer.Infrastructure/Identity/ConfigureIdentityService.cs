using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Infrastructure.Identity.Services;
using Microsoft.Extensions.DependencyInjection;
using Portofolio.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Infrastructure.Identity
{
    public static class ConfigureIdentityService
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
