using AutoMapper;
using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryArchiveAndSalesPlatform.API.Application.AppServices
{
    public class ShelfAppService
        (
           IUnitOfWork unitOfWork, 
           IMapper _mapper,
           ILogger<BookAppService> _logger,
           IValidator<CreateShelfDto> _createValidator,
           IValidator<UpdateShelfDto> _updateValidator
        ) : IShelfAppService
    {
        public async Task CreateAsync(CreateShelfDto shelfDto)
        {
            _createValidator.ValidateAndThrow(shelfDto);
            _logger.LogInformation("Creating a new shelf");

            var shelf = _mapper.Map<Shelf>(shelfDto);
            await unitOfWork.ShelfRepository.CreateAsync(shelf);
            await unitOfWork.Complete();

            _logger.LogInformation("Shelf created successfully");
        }

        public async Task DeleteAsync(Guid Id)
        {
            _logger.LogInformation("Deleting shelf with Id: {Id}", Id);

            var shelf = await unitOfWork.ShelfRepository.GetAsync(Id);

            if (shelf is null)
                throw new ArgumentNullException("No shelf found with the given Id: {Id}", Id.ToString());
            
            await unitOfWork.ShelfRepository.DeleteAsync(shelf);

            await unitOfWork.Complete();

            _logger.LogInformation("Shelf with Id: {Id} deleted successfully", Id);
        }

        public async Task<List<ShelfDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all shelfs");
            var shelfs = await unitOfWork.ShelfRepository.GetAllAsync();

            _logger.LogInformation("Fetched {Count} shelfs", shelfs.Count);
            return _mapper.Map<List<ShelfDto>>(shelfs);
        }

        public async Task<ShelfDto> GetAsync(Guid Id)
        {
            _logger.LogInformation("Fetching shelf with Id: {Id}", Id);

            var shelf = await unitOfWork.ShelfRepository.GetAsync(Id);

            if (shelf == null)
                throw new ArgumentNullException("No shelf found with the given Id: {Id}", Id.ToString());

            _logger.LogInformation("Fetched shelf with Id: {Id}", Id);
            return _mapper.Map<ShelfDto>(shelf);         
        }

        public async Task UpdateAsync(Guid Id,UpdateShelfDto shelfDto)
        {
            _updateValidator.ValidateAndThrow(shelfDto);
            _logger.LogInformation("Updating shelf with Id: {Id}", Id);

            var shelf =await unitOfWork.ShelfRepository.GetAsync(Id);
            if (shelf == null)
                throw new ArgumentNullException("No shelf found with the given Id: {Id}", Id.ToString());

            _mapper.Map(shelfDto, shelf);

            await unitOfWork.ShelfRepository.UpdateAsync(shelf);
            await unitOfWork.Complete();

            _logger.LogInformation("Shelf with Id: {Id} updated successfully", Id);
        }
    }
}
