using LibraryArchiveAndSalesPlatform.API.Domain.Enums;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note
{
    public record NoteDto
        (
            Guid Id,
            Guid BookId,
            string UserId,
            string Title,
            string Content,
            bool IsPrivate,
            PrivacySetting PrivacySetting
        );
}
