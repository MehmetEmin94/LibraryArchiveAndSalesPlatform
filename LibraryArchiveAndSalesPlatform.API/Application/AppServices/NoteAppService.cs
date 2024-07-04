using AutoMapper;
using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Services;
using LibraryArchiveAndSalesPlatform.API.Domain.Enums;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace LibraryArchiveAndSalesPlatform.API.Application.AppServices
{
    public class NoteAppService
        (
           IUnitOfWork unitOfWork, IMapper _mapper, 
           IUserService userService, 
           EmailProducer _emailProducer,
           UserManager<User> userManager,
           Logger<NoteAppService> _logger,
           IValidator<CreateNoteDto> _createValidator,
           IValidator<UpdateNoteDto> _updateValidator
        ) 
        : INoteAppService
    {
        public async Task ChangeNotePrivacyAsync(Guid noteId, PrivacySetting privacySetting)
        {
            _logger.LogInformation("Changing privacy setting of note with Id: {NoteId}", noteId);

            var note = await unitOfWork.NoteRepository.GetAsync(noteId);

            note.PrivacySetting = privacySetting;

            await unitOfWork.NoteRepository.UpdateAsync(note);
            await unitOfWork.Complete();

            _logger.LogInformation("Successfully changed privacy setting of note with Id: {NoteId}", noteId);
        }

        public async Task CreateAsync(CreateNoteDto noteDto)
        {
            _createValidator.ValidateAndThrow(noteDto);

            _logger.LogInformation("Creating a new note with title: {Title}", noteDto.Title);

            var note = _mapper.Map<Note>(noteDto);

            string userId = userService.GetUserId();

            note.UserId = userId;

            await unitOfWork.NoteRepository.CreateAsync(note);
            await unitOfWork.Complete();

            _logger.LogInformation("Note with title: {Title} created successfully", noteDto.Title);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting note with Id: {id}", id);

            var note = await unitOfWork.NoteRepository.GetAsync(id);

            if (note is null)
                throw new ArgumentNullException("No note found with the given Id: {id}", id.ToString());

            await unitOfWork.NoteRepository.DeleteAsync(note);
            await unitOfWork.Complete();

            _logger.LogInformation("Note with Id: {Id} deleted successfully", id);
        }

        public async Task<List<NoteDto>> GetAllAsync(QueryObject query)
        {
            _logger.LogInformation("Fetching all books with query: {Query}", query);

            var notes = await unitOfWork.NoteRepository.GetAllFilteredAsync(query);

            _logger.LogInformation("Fetched {Count} books", notes.Count);

            return _mapper.Map<List<NoteDto>>(notes);
        }

        public async Task<NoteDto> GetAsync(Guid id)
        {
            _logger.LogInformation("Fetching note with Id: {NoteId}", id);

            var note = await unitOfWork.NoteRepository.GetAsync(id);

            if (note is null)
                throw new ArgumentNullException("No note found with the given Id: {id}", id.ToString());

            _logger.LogInformation("Fetched note with Id: {NoteId}", id);

            return _mapper.Map<NoteDto>(note);
        }

        public async Task ShareNoteAsync(Guid noteId, List<string> users)
        {
            var note = await unitOfWork.NoteRepository.GetAsync(noteId);

            if (note is null)
                throw new ArgumentNullException("No note found with the given Id: {noteId}", noteId.ToString());

            foreach (var userName in users)
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user is null)
                    throw new ArgumentNullException("No user found with the given username: {userName}", userName);

                _emailProducer.PublishEmailRequest(new EmailMessage
                {
                    ToEmail = user.Email,
                    Subject = note.Title,
                    Body = note.Content
                });
            }
            
        }

        public async Task UpdateAsync(Guid id,UpdateNoteDto noteDto)
        {
            _updateValidator.ValidateAndThrow(noteDto);

            var note = await unitOfWork.NoteRepository.GetAsync(id);

            if (note is null)
                throw new ArgumentNullException("No note found with the given Id: {id}", id.ToString());

            _mapper.Map(noteDto,note);

            await unitOfWork.NoteRepository.UpdateAsync(note);
            await unitOfWork.Complete();
        }
    }
}
