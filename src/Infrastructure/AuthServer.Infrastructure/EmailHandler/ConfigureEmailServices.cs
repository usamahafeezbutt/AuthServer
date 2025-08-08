using Microsoft.Extensions.DependencyInjection;
using AuthServer.Application.Common.Interfaces.Email;
using AuthServer.Infrastructure.EmailHandler.Services;

namespace AuthServer.Infrastructure.EmailHandler
{
    public static class ConfigureEmailServices
    {
        public static IServiceCollection AddEmail(
            this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
