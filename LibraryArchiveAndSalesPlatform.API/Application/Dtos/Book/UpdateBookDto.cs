namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book
{
    public record UpdateBookDto
        (
            string Name,
            string Genre,
            string Description,
            Guid ShelfId,
            IFormFile File
        );
}
