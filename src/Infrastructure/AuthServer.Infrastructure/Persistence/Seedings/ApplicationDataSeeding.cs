using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Infrastructure.Persistence.DatabaseContexts;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Infrastructure.Persistence.Seedings
{
    public static class ApplicationDataSeeding
    {
        public static async Task SeedApplicationData(
            ApplicationDbContext applicationDbContext,
            IIdentityService identityService,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            try
            {
                await CreateRoles(roleManager);
                await CreateUsers(identityService);
            }
            catch (Exception) { }
        }

        private static async Task CreateUsers(IIdentityService identityService)
        {
            if (identityService.FindByEmailAsync("admin1@example.com").GetAwaiter().GetResult() is null)
            {
                var admin1 = new Domain.Entities.ApplicationUser{
                    Name = "Admin 1",
                    UserName = "admin1@example.com",
                    Email = "admin1@example.com",
                    PhoneNumber = "090078601"
                };

                await identityService.CreateUserAsync(admin1, "123456@ABCdef");

                await identityService.AssignRoleAsync(admin1, "Admin");
            }

            if (identityService.FindByEmailAsync("admin2@example.com").GetAwaiter().GetResult() is null)
            {
                var admin2 = new Domain.Entities.ApplicationUser
                {
                    Name = "Admin 2",
                    UserName = "admin2@example.com",
                    Email = "admin2@example.com",
                    PhoneNumber = "090078601"
                };

                await identityService.CreateUserAsync(admin2, "123456@ABCdef");

                await identityService.AssignRoleAsync(admin2, "Admin");
            }
        }

        private static async Task CreateRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Staff"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Staff"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }
        }
    }
}
