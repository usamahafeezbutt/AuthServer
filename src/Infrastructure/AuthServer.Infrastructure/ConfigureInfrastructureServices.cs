using CleanArchitecture.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthServer.Infrastructure.EmailHandler;
using AuthServer.Infrastructure.Persistence;

namespace AuthServer.Infrastructure
{
    public static class ConfigureInfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configurations)
        {
            services.AddEmail();
            services.AddPersistence(configurations);
            services.AddIdentityServices();
            return services;
        }
    }
}
