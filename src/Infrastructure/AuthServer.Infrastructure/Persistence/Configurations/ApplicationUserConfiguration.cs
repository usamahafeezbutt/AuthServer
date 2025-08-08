using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AuthServer.Domain.Entities;

namespace AuthServer.Infrastructure.Persistence.Configuration
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(u => u.ApplicationToken)
               .WithOne(t => t.User)
               .HasForeignKey<ApplicationToken>(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
