using LibraryArchiveAndSalesPlatform.API.Application.Dtos;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks;

namespace LibraryArchiveAndSalesPlatform.API.Application.IAppServices
{
    public interface IShelfAppService
    {
        Task CreateAsync(CreateShelfDto shelfDto);
        Task UpdateAsync(Guid Id, UpdateShelfDto shelfDto);
        Task DeleteAsync(Guid Id);
        Task<ShelfDto> GetAsync(Guid Id);
        Task<List<ShelfDto>> GetAllAsync();
    }
}
