using System.Reflection;
using AuthServer.Application.Common.Interfaces.Services;
using AuthServer.Application.Services.Account;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthServer.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            services.AddSingleton(
                provider => new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(Assembly.GetExecutingAssembly());
                },
                provider.GetRequiredService<ILoggerFactory>()).CreateMapper()
            );

            services.AddMemoryCache();

            services.AddValidatorsFromAssembly(thisAssembly);

            services.AddScoped<IAccountService, AccountService>();
            
            return services;
        }
    }
}