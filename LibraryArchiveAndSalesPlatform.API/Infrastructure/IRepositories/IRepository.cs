using System.Linq.Expressions;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task<TEntity?> GetAsync(Guid Id);
        Task<List<TEntity>> GetAllAsync();
    }
}
