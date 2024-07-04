using LibraryArchiveAndSalesPlatform.API.Domain.Models;

namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices
{
    public interface ITokenService
    {
        string CreateToken(User user,IList<string> userRoles);
    }
}
