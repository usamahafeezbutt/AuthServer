using Microsoft.EntityFrameworkCore;

namespace AuthServer.Application.Common.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> Add(T entity);

        Task<bool> Add(IList<T> entities);

        Task<bool> Update(T entity);

        Task<bool> Update(IList<T> entities);

        Task<bool> Delete(T entity);

        Task<bool> Delete(IList<T> entities);

        DbSet<T> Table { get; }

        IQueryable<T> TableNoTracking { get; }

        Task<bool> ExecuteNonQuery(string sqlQuery);
    }
}
