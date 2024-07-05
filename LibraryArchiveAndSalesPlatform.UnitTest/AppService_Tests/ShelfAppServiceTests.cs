using Xunit;
using Moq;
using FluentValidation;
using AutoMapper;
using Microsoft.Extensions.Logging;
using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;

namespace LibraryArchiveAndSalesPlatform.UnitTests.AppService_Tests
{
    public class ShelfAppServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ShelfAppService>> _loggerMock;
        private readonly Mock<IValidator<CreateShelfDto>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateShelfDto>> _updateValidatorMock;
        private readonly ShelfAppService _shelfAppService;

        public ShelfAppServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ShelfAppService>>();
            _createValidatorMock = new Mock<IValidator<CreateShelfDto>>();
            _updateValidatorMock = new Mock<IValidator<UpdateShelfDto>>();

            _shelfAppService = new ShelfAppService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ValidShelfDto_ShouldCreateShelf()
        {
            // Arrange
            var createShelfDto = new CreateShelfDto("Third", "First", "Second");
            _createValidatorMock.Setup(v => v.ValidateAndThrow(It.IsAny<CreateShelfDto>()));
            var shelf = new Shelf { Id = Guid.NewGuid(), Section = createShelfDto.Section, Position = createShelfDto.Position, Row = createShelfDto.Row };
            _mapperMock.Setup(m => m.Map<Shelf>(It.IsAny<CreateShelfDto>())).Returns(shelf);
            _unitOfWorkMock.Setup(u => u.ShelfRepository.CreateAsync(It.IsAny<Shelf>())).Returns(Task.CompletedTask);

            // Act
            await _shelfAppService.CreateAsync(createShelfDto);

            // Assert
            _createValidatorMock.Verify(v => v.ValidateAndThrow(createShelfDto), Times.Once);
            _unitOfWorkMock.Verify(u => u.ShelfRepository.CreateAsync(shelf), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ShouldDeleteShelf()
        {
            // Arrange
            var shelfId = Guid.NewGuid();
            var shelf = new Shelf { Id = shelfId, Section = "Third", Position = "Third", Row = "Third" };
            _unitOfWorkMock.Setup(u => u.ShelfRepository.GetAsync(shelfId)).ReturnsAsync(shelf);
            _unitOfWorkMock.Setup(u => u.ShelfRepository.DeleteAsync(shelf)).Returns(Task.CompletedTask);

            // Act
            await _shelfAppService.DeleteAsync(shelfId);

            // Assert
            _unitOfWorkMock.Verify(u => u.ShelfRepository.GetAsync(shelfId), Times.Once);
            _unitOfWorkMock.Verify(u => u.ShelfRepository.DeleteAsync(shelf), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllShelves()
        {
            // Arrange
            var shelves = new List<Shelf>
        {
            new Shelf { Id = Guid.NewGuid(), Section = "Third", Position = "First", Row = "Third" },
            new Shelf { Id = Guid.NewGuid(), Section = "First", Position = "Third", Row = "Second" }
        };
            var shelfDtos = new List<ShelfDto>
        {
            new ShelfDto ( shelves[0].Id,  shelves[0].Section,  shelves[0].Position, shelves[0].Row ,new List<BookDto>()),
            new ShelfDto ( shelves[1].Id,  shelves[1].Section, shelves[1].Position,shelves[1].Row ,new List<BookDto>())
        };

            _unitOfWorkMock.Setup(u => u.ShelfRepository.GetAllAsync()).ReturnsAsync(shelves);
            _mapperMock.Setup(m => m.Map<List<ShelfDto>>(shelves)).Returns(shelfDtos);

            // Act
            var result = await _shelfAppService.GetAllAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.ShelfRepository.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<ShelfDto>>(shelves), Times.Once);
            Assert.Equal(shelfDtos, result);
        }

        [Fact]
        public async Task GetAsync_ExistingId_ShouldReturnShelf()
        {
            // Arrange
            var shelfId = Guid.NewGuid();
            var shelf = new Shelf { Id = shelfId, Section = "Third", Position = "First", Row = "Third" };
            var shelfDto = new ShelfDto(shelfId, shelf.Section, shelf.Position, shelf.Row, new List<BookDto>());

            _unitOfWorkMock.Setup(u => u.ShelfRepository.GetAsync(shelfId)).ReturnsAsync(shelf);
            _mapperMock.Setup(m => m.Map<ShelfDto>(shelf)).Returns(shelfDto);

            // Act
            var result = await _shelfAppService.GetAsync(shelfId);

            // Assert
            _unitOfWorkMock.Verify(u => u.ShelfRepository.GetAsync(shelfId), Times.Once);
            _mapperMock.Verify(m => m.Map<ShelfDto>(shelf), Times.Once);
            Assert.Equal(shelfDto, result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingId_ShouldUpdateShelf()
        {
            // Arrange
            var shelfId = Guid.NewGuid();
            var updateShelfDto = new UpdateShelfDto("Third", "First", "Second");
            var shelf = new Shelf { Id = shelfId, Section = updateShelfDto.Section, Position = updateShelfDto.Position, Row = updateShelfDto.Row };

            _updateValidatorMock.Setup(v => v.ValidateAndThrow(updateShelfDto));
            _unitOfWorkMock.Setup(u => u.ShelfRepository.GetAsync(shelfId)).ReturnsAsync(shelf);
            _unitOfWorkMock.Setup(u => u.ShelfRepository.UpdateAsync(shelf)).Returns(Task.CompletedTask);

            // Act
            await _shelfAppService.UpdateAsync(shelfId, updateShelfDto);

            // Assert
            _updateValidatorMock.Verify(v => v.ValidateAndThrow(updateShelfDto), Times.Once);
            _unitOfWorkMock.Verify(u => u.ShelfRepository.GetAsync(shelfId), Times.Once);
            _unitOfWorkMock.Verify(u => u.ShelfRepository.UpdateAsync(shelf), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }
    }
}
