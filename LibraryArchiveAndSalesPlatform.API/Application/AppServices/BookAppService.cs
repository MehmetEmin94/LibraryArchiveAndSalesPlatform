using AutoMapper;
using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;

namespace LibraryArchiveAndSalesPlatform.API.Application.AppServices
{
    public class BookAppService
        (
           IUnitOfWork unitOfWork, 
           IMapper _mapper, 
           ILogger<BookAppService> _logger,
           IValidator<CreateBookDto> _createValidator,
           IValidator<UpdateBookDto> _updateValidator
        ) : IBookAppService
    {
        public async Task CreateAsync(CreateBookDto bookDto)
        {
            _createValidator.ValidateAndThrow(bookDto);

            _logger.LogInformation("Creating a new book with title: {Title}", bookDto.Name);

            var book = _mapper.Map<Book>(bookDto);

            using (var memoryStream = new MemoryStream())
            {
                await bookDto.File.CopyToAsync(memoryStream);
                book.Image = new FileContent
                {
                    Content = memoryStream.ToArray(),
                    ContentType = bookDto.File.ContentType,
                    Name = bookDto.File.Name
                };
            }
            await unitOfWork.BookRepository.CreateAsync(book);
            await unitOfWork.Complete();

            _logger.LogInformation("Book with title: {Title} created successfully", bookDto.Name);
        }

        public async Task DeleteAsync(Guid Id)
        {
            _logger.LogInformation("Deleting book with Id: {Id}", Id);

            var book = await unitOfWork.BookRepository.GetAsync(Id);

            if (book == null)
                throw new ArgumentNullException("No book found with the given Id: {Id}", Id.ToString());

            await unitOfWork.BookRepository.DeleteAsync(book);
            await unitOfWork.Complete();

            _logger.LogInformation("Book with Id: {Id} deleted successfully", Id);
        }

        public async Task<List<BookDto>> GetAllAsync(BookQueryObject query)
        {
            _logger.LogInformation("Fetching all books with query: {Query}", query);

            var books= await unitOfWork.BookRepository.GetAllFilteredAsync(query);

            _logger.LogInformation("Fetched {Count} books", books.Count);

            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetAsync(Guid Id)
        {
            _logger.LogInformation("Fetching book with Id: {Id}", Id);

            var book = await unitOfWork.BookRepository.GetAsync(Id);

            if (book == null)
                throw new ArgumentNullException("No book found with the given Id: {Id}", Id.ToString());

            _logger.LogInformation("Fetched book with Id: {Id}", Id);

            return _mapper.Map<BookDto>(book);
        }

        public async Task<FileContent> GetBookImageAsync(Guid Id)
        {
            _logger.LogInformation("Fetching image for book with Id: {Id}", Id);
            var book = await unitOfWork.BookRepository.GetAsync(Id);

            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (book.Image == null)
                throw new ArgumentNullException("Image does not exist");

            _logger.LogInformation("Fetched image for book with Id: {Id}", Id);

            return book.Image;
        }

        public async Task UpdateAsync(Guid Id, UpdateBookDto bookDto)
        {
            _updateValidator.ValidateAndThrow(bookDto);

            _logger.LogInformation("Updating book with Id: {Id}", Id);

            var book = await unitOfWork.BookRepository.GetAsync(Id);
            if (book == null)
                throw new ArgumentNullException("No book found with the given Id: {Id}", Id.ToString());

            _mapper.Map(bookDto, book);

            using (var memoryStream = new MemoryStream())
            {
                await bookDto.File.CopyToAsync(memoryStream);
                book.Image = new FileContent
                {
                    Content = memoryStream.ToArray(),
                    ContentType = bookDto.File.ContentType,
                    Name = bookDto.File.Name
                };
            }

            await unitOfWork.BookRepository.UpdateAsync(book);
            await unitOfWork.Complete();

            _logger.LogInformation("Book with Id: {Id} updated successfully", Id);
        }
    }
}
