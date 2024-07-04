using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book
{
    public record BookDto
        (
           Guid Id, 
           string Name, 
           string Genre, 
           string Description, 
           Guid ShelfId, 
           List<NoteDto> Notes
        );
    
}
