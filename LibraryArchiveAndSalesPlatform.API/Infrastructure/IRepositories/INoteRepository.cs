using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;

namespace LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<List<Note>> GetAllFilteredAsync(QueryObject query);
    }
}
