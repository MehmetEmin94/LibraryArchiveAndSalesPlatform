using System.ComponentModel.DataAnnotations;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account
{
    public record RegisterDto
    (
        [Required] string? UserName, 
        [Required][EmailAddress]string? Email, 
        [Required] string? Password
    );
}
