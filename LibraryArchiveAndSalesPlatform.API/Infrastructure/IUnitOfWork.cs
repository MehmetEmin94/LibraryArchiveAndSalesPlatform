using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository BookRepository { get; }
        IShelfRepository ShelfRepository { get; }
        INoteRepository NoteRepository { get; }
        Task<int> Complete();
    }
}
