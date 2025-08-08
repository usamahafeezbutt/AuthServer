using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthServer.Application.Common.Interfaces.DbContexts;
using AuthServer.Domain.Entities;
using System.Reflection;
using AuthServer.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Infrastructure.Persistence.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Infrastructure.Persistence.DatabaseContexts
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options),
        IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationAuditProperties(entry.Entity);
                        break;

                    case EntityState.Modified:
                        SetModificationAuditProperties(entry.Entity);
                        break;

                    case EntityState.Deleted:
                        CancelDeletionForSoftDelete(entry);
                        SetModificationAuditProperties(entry.Entity);
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private static void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (entry.Entity is not ISoftDelete)
            {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        private void SetModificationAuditProperties(object entity)
        {
            if (entity is not IModificationAudit entityWithModifiedAudit)
            {
                return;
            }

            SetModifiedAuditProperty(entityWithModifiedAudit);

            SetModifiedByAuditProperty(_currentUserService.UserId, entityWithModifiedAudit);
        }

        private void SetCreationAuditProperties(object entity)
        {
            if (entity is not ICreationAudit entityWithCreateAudit)
            {
                return;
            }

            SetCreatedAuditProperty(entityWithCreateAudit);

            SetCreatedByAuditProperty(_currentUserService.UserId, entityWithCreateAudit);
        }

        private static void SetCreatedAuditProperty(ICreationAudit? entityWithCreateAudit)
        {
            if (entityWithCreateAudit!.Created == default)
            {
                entityWithCreateAudit.Created = DateTime.Now;
            }
        }

        private static void SetCreatedByAuditProperty(Guid userId, ICreationAudit? entityWithCreateAudit)
        {
            if (userId != Guid.Empty && entityWithCreateAudit!.CreatedBy != null)
            {
                //Unknown user or Id already set in database table
                return;
            }

            //Finally, set CreatorUserId!
            entityWithCreateAudit!.CreatedBy = userId;
        }

        private static void SetModifiedAuditProperty(IModificationAudit? entityWithModifiedAudit)
        {
            if (entityWithModifiedAudit!.LastModified == default)
            {
                entityWithModifiedAudit.LastModified = DateTime.Now;
            }
        }

        private static void SetModifiedByAuditProperty(Guid userId, IModificationAudit? entityWithModifiedAudit)
        {
            if (userId != Guid.Empty && entityWithModifiedAudit!.LastModifiedBy != null)
            {
                //Unknown user or Id already set in database table
                return;
            }

            //Finally, set CreatorUserId!
            entityWithModifiedAudit!.LastModifiedBy = userId;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.ApplyGlobalQueryFilters<ISoftDelete>(entity => !entity.IsDeleted);

            base.OnModelCreating(builder);
        }
    }
}
