using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthServer.Application.Common.Constants.Database;
using AuthServer.Application.Common.Interfaces.Repositories;
using AuthServer.Infrastructure.Persistence.DatabaseContexts;
using AuthServer.Infrastructure.Persistence.Repositories;
using AuthServer.Domain.Entities;

namespace AuthServer.Infrastructure.Persistence
{
    public static class ConfigurePersistenceServices
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configurations)
        {
            if (configurations.GetValue<bool>(DbContextConstants.UseInMemoryDatabase))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(DbContextConstants.AuthServerDb));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                                options.UseSqlServer(
                                    configurations
                                    .GetConnectionString(
                                        DbContextConstants.DefaultConnection)));
            }

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            });

            services.AddIdentityCore<ApplicationUser>(config =>
            {
                config.Password.RequiredLength = 8;
            }).AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}