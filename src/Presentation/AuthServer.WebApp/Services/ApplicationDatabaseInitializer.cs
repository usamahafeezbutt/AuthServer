using Microsoft.EntityFrameworkCore;
using AuthServer.Infrastructure.Persistence.DatabaseContexts;
using AuthServer.Infrastructure.Persistence.Seedings;
using Microsoft.AspNetCore.Identity;
using AuthServer.Application.Common.Interfaces.Identity;

namespace AuthServer.Infrastructure.Persistence
{
    public static class ApplicationDatabaseInitializer
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                await initialiser.Database.MigrateAsync();
                await ApplicationDataSeeding.SeedApplicationData(initialiser, identityService, roleManager);
            }
            catch (Exception ex)
            {
                logger.LogError("Error occured while seeding data in database {0}", ex);
            }
        }
    }
}