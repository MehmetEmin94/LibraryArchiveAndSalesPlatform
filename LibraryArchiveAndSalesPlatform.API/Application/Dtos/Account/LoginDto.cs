using System.ComponentModel.DataAnnotations;

namespace LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account
{
    public record LoginDto([Required] string UserName, [Required] string Password);
}
