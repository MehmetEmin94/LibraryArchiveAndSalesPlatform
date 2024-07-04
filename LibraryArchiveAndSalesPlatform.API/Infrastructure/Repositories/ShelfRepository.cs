using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.Data;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Repositories
{
    public class ShelfRepository : Repository<Shelf>, IShelfRepository
    {
        private readonly DbSet<Shelf> _dbSet;
        public ShelfRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.Set<Shelf>();
        }
        public override async Task<List<Shelf>> GetAllAsync()
        {
            return await _dbSet.Include(b => b.Books).ToListAsync();
        }

        public override async Task<Shelf?> GetAsync(Guid Id)
        {
            return await _dbSet.Include(b => b.Books).FirstOrDefaultAsync(s => s.Id.Equals(Id));
        }
    }
}
