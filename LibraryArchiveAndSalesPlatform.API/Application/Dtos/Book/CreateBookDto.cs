using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book
{
    public record CreateBookDto
        (
            string Name,
            string Genre,
            string Description,
            Guid ShelfId,
            IFormFile File
        );
    
}
