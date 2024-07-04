using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects;

namespace LibraryArchiveAndSalesPlatform.API.Application.IAppServices
{
    public interface IBookAppService
    {
        Task CreateAsync(CreateBookDto bookDto);
        Task UpdateAsync(Guid Id, UpdateBookDto bookDto);
        Task DeleteAsync(Guid Id);
        Task<BookDto> GetAsync(Guid Id);
        Task<FileContent> GetBookImageAsync(Guid Id);
        Task<List<BookDto>> GetAllAsync(BookQueryObject query);
    }
}
