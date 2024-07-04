using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.Data;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Repositories
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        private readonly DbSet<Note> _dbSet;
        public NoteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.Set<Note>();
        }
        public override async Task<List<Note>> GetAllAsync()
        {
            return await _dbSet.Include(b => b.Book).ToListAsync();
        }

        public async Task<List<Note>> GetAllFilteredAsync(QueryObject query)
        {
            var notes = _dbSet.Include(b => b.Book)
                      .Where(b => b.IsDeleted == query.IsDeleted)
                      .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    notes = query.IsDecsending ? notes.OrderByDescending(s => s.CreatedAt) : notes.OrderBy(s => s.CreatedAt);
                }
            }
            return await notes
                         .Skip(query.PageSize * query.PageIndex)
                         .Take(query.PageSize)
                         .ToListAsync();
        }
    }
}
