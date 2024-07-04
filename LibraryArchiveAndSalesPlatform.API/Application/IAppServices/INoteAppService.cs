using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchiveAndSalesPlatform.API.Application.IAppServices
{
    public interface INoteAppService
    {
        Task CreateAsync(CreateNoteDto noteDto);
        Task UpdateAsync(Guid id,UpdateNoteDto noteDto);
        Task DeleteAsync(Guid id);
        Task<NoteDto> GetAsync(Guid id);
        Task<List<NoteDto>> GetAllAsync(QueryObject query);
        Task ShareNoteAsync(Guid noteId, List<string> users);
        Task ChangeNotePrivacyAsync(Guid noteId, PrivacySetting privacySetting);
    }
}
