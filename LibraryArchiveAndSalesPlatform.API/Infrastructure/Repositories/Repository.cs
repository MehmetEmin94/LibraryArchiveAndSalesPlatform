using LibraryArchiveAndSalesPlatform.API.Infrastructure.Data;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        DbSet<TEntity> _dbSet;
        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }
        public virtual async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<TEntity?> GetAsync(Guid Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}
