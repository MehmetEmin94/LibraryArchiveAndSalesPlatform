using LibraryArchiveAndSalesPlatform.API.Infrastructure.Data;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure
{
    public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
    {
        private IBookRepository _bookRepository;

        public IBookRepository BookRepository 
        {
            get
            {
                return _bookRepository ??= new BookRepository(dbContext);
            }
        }

        private IShelfRepository _shelfRepository;
        public IShelfRepository ShelfRepository
        {
            get
            {
                return _shelfRepository ??= new ShelfRepository(dbContext);
            }
        }

        private INoteRepository _noteRepository;
        public INoteRepository NoteRepository
        {
            get
            {
                return _noteRepository ??= new NoteRepository(dbContext);
            }
        }

        public async Task<int> Complete()
        {
            return await dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
