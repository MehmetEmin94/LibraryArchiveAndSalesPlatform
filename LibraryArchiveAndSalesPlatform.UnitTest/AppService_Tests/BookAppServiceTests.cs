using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.IRepositories;

namespace LibraryArchiveAndSalesPlatform.UnitTests.AppServices
{
    public class BookAppServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookAppService>> _loggerMock;
        private readonly Mock<IValidator<CreateBookDto>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateBookDto>> _updateValidatorMock;
        private readonly BookAppService _bookAppService;

        public BookAppServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookAppService>>();
            _createValidatorMock = new Mock<IValidator<CreateBookDto>>();
            _updateValidatorMock = new Mock<IValidator<UpdateBookDto>>();

            _bookAppService = new BookAppService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_Should_Create_New_Book()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            (
                "New Book",
                "Genre",
                "Desc",
                Guid.NewGuid(),
                Mock.Of<IFormFile>()
            );
            var book = new Book
            {
                Name = "New Book",
                Genre = "Genre",
                Description = "Desc",
                ShelfId = Guid.NewGuid()
            };
            using (var memoryStream = new MemoryStream())
            {
                await createBookDto.File.CopyToAsync(memoryStream);
                book.Image = new FileContent
                {
                    Content = memoryStream.ToArray(),
                    ContentType = createBookDto.File.ContentType,
                    Name = createBookDto.File.Name
                };
            }

            _createValidatorMock.Setup(v => v.ValidateAndThrow(createBookDto));
            _mapperMock.Setup(m => m.Map<Book>(createBookDto)).Returns(book);
            var bookRepositoryMock = new Mock<IBookRepository>();
            _unitOfWorkMock.Setup(u => u.BookRepository).Returns(bookRepositoryMock.Object);
            bookRepositoryMock.Setup(br => br.CreateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            await _bookAppService.CreateAsync(createBookDto);

            // Assert
            bookRepositoryMock.Verify(br => br.CreateAsync(It.IsAny<Book>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_Exception_When_Book_Not_Found()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookAppService.DeleteAsync(bookId));
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_Book()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId };

            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync(book);
            _unitOfWorkMock.Setup(u => u.BookRepository.DeleteAsync(book)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Complete()).ReturnsAsync(1);

            // Act
            await _bookAppService.DeleteAsync(bookId);

            // Assert
            _unitOfWorkMock.Verify(u => u.BookRepository.DeleteAsync(book), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_List_Of_Books()
        {
            // Arrange
            var query = new BookQueryObject();
            var shelfId= Guid.NewGuid();
            var shelfId_1= Guid.NewGuid();
            var bookId= Guid.NewGuid();
            var bookId_1= Guid.NewGuid();
            var books = new List<Book>
            {
                new Book 
                { 
                    Id=bookId,
                    Name = "New Book",
                    Genre = "Genre",
                    Description = "Desc",
                    ShelfId = shelfId
                },
                new Book 
                {
                    Id=bookId_1,
                    Name = "New Book 1",
                    Genre = "Genre1",
                    Description = "Desc1",
                    ShelfId = shelfId_1
                }
            };

            _unitOfWorkMock.Setup(u => u.BookRepository.GetAllFilteredAsync(query)).ReturnsAsync(books);
            _mapperMock.Setup(m => m.Map<List<BookDto>>(books)).Returns(new List<BookDto>
            {
                new BookDto ( bookId,"New Book" ,"Genre","Desc",shelfId,new List<NoteDto>()),
                new BookDto (bookId_1,"New Book 1", "Genre1" , "Desc1", shelfId_1 ,new List<NoteDto>())
            });

            // Act
            var result = await _bookAppService.GetAllAsync(query);

            // Assert
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Book1");
            result[1].Name.Should().Be("Book2");
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetAsync_Should_Throw_Exception_When_Book_Not_Found()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookAppService.GetAsync(bookId));
        }

        [Fact]
        public async Task GetAsync_Should_Return_BookDto_When_Book_Found()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var shelfId = Guid.NewGuid();
            var book = new Book 
             {
                Id = bookId,
                Name = "New Book",
                Genre = "Genre",
                Description = "Desc",
                ShelfId = shelfId
            };

            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(new BookDto(bookId, "New Book", "Genre", "Desc", shelfId, new List<NoteDto>()));

            // Act
            var result = await _bookAppService.GetAsync(bookId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(bookId);
            result.Name.Should().Be("Book");
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetBookImageAsync_Should_Throw_Exception_When_Book_Not_Found()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookAppService.GetBookImageAsync(bookId));
        }

        [Fact]
        public async Task GetBookImageAsync_Should_Throw_Exception_When_Image_Not_Exist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Image = null };

            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync(book);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookAppService.GetBookImageAsync(bookId));
        }

        [Fact]
        public async Task GetBookImageAsync_Should_Return_FileContent()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var shelfId = Guid.NewGuid();
            var image = new FileContent { Content = new byte[0], ContentType = "image/jpeg", Name = "image.jpg" };
            var book = new Book
            {
                Id = bookId,
                Name = "New Book",
                Genre = "Genre",
                Description = "Desc",
                ShelfId = shelfId,
                Image = image 
            };

            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _bookAppService.GetBookImageAsync(bookId);

            // Assert
            result.Should().NotBeNull();
            result.ContentType.Should().Be("image/jpeg");
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_When_Book_Not_Found()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var updateBookDto = new UpdateBookDto
            (
                "New Book",
                "Genre",
                "Desc",
                Guid.NewGuid(),
                Mock.Of<IFormFile>()
            );

            _unitOfWorkMock.Setup(u => u.BookRepository.GetAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _bookAppService.UpdateAsync(bookId, updateBookDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Book()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var updateBookDto = new UpdateBookDto
            (
                "New Book",
                "Genre",
                "Desc",
                Guid.NewGuid(),
                Mock.Of<IFormFile>()
            );
            var book = new Book
            {
                Name = "New Book",
                Genre = "Genre",
                Description = "Desc",
                ShelfId = Guid.NewGuid()
            };

            using (var memoryStream = new MemoryStream())
            {
                await updateBookDto.File.CopyToAsync(memoryStream);
                book.Image = new FileContent
                {
                    Content = memoryStream.ToArray(),
                    ContentType = updateBookDto.File.ContentType,
                    Name = updateBookDto.File.Name
                };
            }

            

            _updateValidatorMock.Setup(v => v.ValidateAndThrow(updateBookDto));
            _mapperMock.Setup(m => m.Map<Book>(updateBookDto)).Returns(book);
            var bookRepositoryMock = new Mock<IBookRepository>();
            _unitOfWorkMock.Setup(u => u.BookRepository).Returns(bookRepositoryMock.Object);
            bookRepositoryMock.Setup(br => br.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            await _bookAppService.UpdateAsync(bookId, updateBookDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.BookRepository.UpdateAsync(book), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            book.Name.Should().Be("Updated Title");
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }
    }
}
