using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf
{
    public record ShelfDto(Guid Id,string Section, string Row, string Position, List<BookDto> Books);
}
