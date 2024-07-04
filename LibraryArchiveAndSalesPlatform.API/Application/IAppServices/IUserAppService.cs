using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account;

namespace LibraryArchiveAndSalesPlatform.API.Application.IAppServices
{
    public interface IUserAppService
    {
        Task<NewUserDto> Login(LoginDto loginDto);
        Task<NewUserDto> Register(RegisterDto registerDto);
        Task AddRole(string role);
        Task AssignRole(UserRole role);
        Task DeleteUser(string UserName);
        Task UpdateDetails(string userName, UserDetailsDto userDto);
    }
}
