using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace AuthServer.Infrastructure.Persistence.Filters
{
    public static class ModelBuilderFilters
    {
        public static void ApplyGlobalQueryFilters<TBaseEntity>(this ModelBuilder builder,
        Expression<Func<TBaseEntity, bool>> filter)
        {
            var acceptableItems = builder.Model.GetEntityTypes()
                .Where(et => typeof(TBaseEntity).IsAssignableFrom(et.ClrType))
                .ToList();

            foreach (var entityType in acceptableItems)
            {
                var entityParam = Expression.Parameter(entityType.ClrType, "e");

                // replacing parameter with actual type
                var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters[0], entityParam, filter.Body);

                var filterLambda = entityType.GetQueryFilter();
                // Other filter already present, combine them
                if (filterLambda != null)
                {
                    filterBody = ReplacingExpressionVisitor.Replace(entityParam, filterLambda.Parameters[0], filterBody);
                    filterBody = Expression.AndAlso(filterLambda.Body, filterBody);
                    filterLambda = Expression.Lambda(filterBody, filterLambda.Parameters);
                }
                else
                {
                    filterLambda = Expression.Lambda(filterBody, entityParam);
                }

                entityType.SetQueryFilter(filterLambda);
            }
        }

    }
}
