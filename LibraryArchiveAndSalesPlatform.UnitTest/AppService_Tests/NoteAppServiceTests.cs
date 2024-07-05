using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.Domain.Enums;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace LibraryArchiveAndSalesPlatform.UnitTests.AppService_Tests
{
    public class NoteAppServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILogger<NoteAppService>> _loggerMock;
        private readonly Mock<IValidator<CreateNoteDto>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateNoteDto>> _updateValidatorMock;
        private readonly NoteAppService _noteAppService;

        public NoteAppServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _userManagerMock = MockUserManager();
            _loggerMock = new Mock<ILogger<NoteAppService>>();
            _createValidatorMock = new Mock<IValidator<CreateNoteDto>>();
            _updateValidatorMock = new Mock<IValidator<UpdateNoteDto>>();

            _noteAppService = new NoteAppService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object
            );
        }

        // Mocking UserManager<User>
        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task ChangeNotePrivacyAsync_Should_Update_PrivacySetting()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var privacySetting = PrivacySetting.Private;
            var note = new Note { Id = noteId, PrivacySetting = PrivacySetting.Public };

            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync(note);

            // Act
            await _noteAppService.ChangeNotePrivacyAsync(noteId, privacySetting);

            // Assert
            _unitOfWorkMock.Verify(u => u.NoteRepository.UpdateAsync(note), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            note.PrivacySetting.Should().Be(privacySetting);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_New_Note()
        {
            // Arrange
            var createNoteDto = new CreateNoteDto ( Guid.NewGuid(), "Title","Content",false, PrivacySetting.Public );
            var userId = "user123";
            var note = new Note { 
                BookId=Guid.NewGuid(),UserId = userId, Title= "Title", Content="Content", IsPrivate = false, PrivacySetting = PrivacySetting.Public };
            

            _createValidatorMock.Setup(v => v.ValidateAndThrow(createNoteDto));
            _mapperMock.Setup(m => m.Map<Note>(createNoteDto)).Returns(note);
            _userServiceMock.Setup(us => us.GetUserId()).Returns(userId);

            // Act
            await _noteAppService.CreateAsync(createNoteDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.NoteRepository.CreateAsync(note), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            note.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_Exception_When_Note_Not_Found()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync((Note)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _noteAppService.DeleteAsync(noteId));
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_Note()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user123";
            var note = new Note { 
                BookId = Guid.NewGuid(), UserId = userId, Title = "Title", 
                Content = "Content", IsPrivate = false, PrivacySetting = PrivacySetting.Public };

            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync(note);

            // Act
            await _noteAppService.DeleteAsync(noteId);

            // Assert
            _unitOfWorkMock.Verify(u => u.NoteRepository.DeleteAsync(note), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_List_Of_Notes()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var noteId_1 = Guid.NewGuid();
            var query = new QueryObject();
            var userId = "user123";
            var notes = new List<Note> { 
                new Note {Id=noteId, BookId = Guid.NewGuid(), UserId = userId, Title = "Title",
                Content = "Content", IsPrivate = false, PrivacySetting = PrivacySetting.Public }, 
                new Note {Id=noteId_1, BookId = Guid.NewGuid(), UserId = userId, Title = "Title1",
                Content = "Content1", IsPrivate = false, PrivacySetting = PrivacySetting.Public } };

            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAllFilteredAsync(query)).ReturnsAsync(notes);
            _mapperMock.Setup(m => m.Map<List<NoteDto>>(notes)).Returns(new List<NoteDto>
            {
                new NoteDto ( noteId,Guid.NewGuid(), userId, "Title","Content",  false,  PrivacySetting.Public),
                new NoteDto (noteId_1,Guid.NewGuid(), userId, "Title1","Content1",  false,  PrivacySetting.Public )
            });

            // Act
            var result = await _noteAppService.GetAllAsync(query);

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Note1");
            result[1].Title.Should().Be("Note2");
        }

        [Fact]
        public async Task GetAsync_Should_Throw_Exception_When_Note_Not_Found()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync((Note)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _noteAppService.GetAsync(noteId));
        }

        [Fact]
        public async Task GetAsync_Should_Return_NoteDto_When_Note_Found()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user123";
            var note = new Note {
                Id = noteId,
                BookId = Guid.NewGuid(),
                UserId = userId,
                Title = "Title",
                Content = "Content",
                IsPrivate = false,
                PrivacySetting = PrivacySetting.Public
            };

            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync(note);
            _mapperMock.Setup(m => m.Map<NoteDto>(note))
                .Returns(new NoteDto (noteId,Guid.NewGuid(), userId, "Title", "Content", false, PrivacySetting.Public));

            // Act
            var result = await _noteAppService.GetAsync(noteId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(noteId);
            result.Title.Should().Be("Note");
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_When_Note_Not_Found()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var updateNoteDto = new UpdateNoteDto (Guid.NewGuid(), "Title", "Content", false, PrivacySetting.Public);

            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync((Note)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _noteAppService.UpdateAsync(noteId, updateNoteDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Note()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user123";
            var updateNoteDto = new UpdateNoteDto (Guid.NewGuid(), "Title", "Content", false, PrivacySetting.Public);
            var note = new Note { 
                Id = noteId, BookId=Guid.NewGuid(), UserId=userId, Title= "Title", 
                Content= "Content", IsPrivate =  false, PrivacySetting = PrivacySetting.Public };

            _updateValidatorMock.Setup(v => v.ValidateAndThrow(updateNoteDto));
            _unitOfWorkMock.Setup(u => u.NoteRepository.GetAsync(noteId)).ReturnsAsync(note);

            // Act
            await _noteAppService.UpdateAsync(noteId, updateNoteDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.NoteRepository.UpdateAsync(note), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            note.Title.Should().Be("Updated Title");
        }
    }
}
