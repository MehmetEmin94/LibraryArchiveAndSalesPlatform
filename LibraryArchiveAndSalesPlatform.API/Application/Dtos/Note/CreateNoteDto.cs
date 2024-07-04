using LibraryArchiveAndSalesPlatform.API.Domain.Enums;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note
{
    public record CreateNoteDto
        (
            Guid BookId, 
            string Title, 
            string Content, 
            bool IsPrivate, 
            PrivacySetting PrivacySetting
        );
}
