using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.Data;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private readonly DbSet<Book> _dbSet;
        public BookRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.Set<Book>();
        }

        public override async Task<List<Book>> GetAllAsync()
        {
            return await _dbSet.Include(b => b.Notes).ToListAsync();
        }

        public async Task<List<Book>> GetAllFilteredAsync(BookQueryObject query)
        {
            var books = _dbSet.Include(b => b.Notes)
                      .Where(b => b.IsDeleted == query.QueryObject.IsDeleted)
                      .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Genre))
            {
                books = books.Where(b => b.Genre.Contains(query.Genre));
            };

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                books = books.Where(b => b.Name.Contains(query.Name));
            };

            if (!string.IsNullOrWhiteSpace(query.QueryObject.SortBy))
            {
                if (query.QueryObject.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    books = query.QueryObject.IsDecsending ? books.OrderByDescending(s => s.Name) : books.OrderBy(s => s.Name);
                }
            }

            return await books
                         .Skip(query.QueryObject.PageSize * query.QueryObject.PageIndex)
                         .Take(query.QueryObject.PageSize)
                         .ToListAsync();

        }
    }
}
