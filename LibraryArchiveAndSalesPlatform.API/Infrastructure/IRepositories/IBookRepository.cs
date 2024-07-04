using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<List<Book>> GetAllFilteredAsync(BookQueryObject query);
    }
}
