using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.CustomExceptions;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.AspNetCore.Identity;
using FluentValidation;

namespace LibraryArchiveAndSalesPlatform.API.Application.AppServices
{
    public class UserAppService
        (
          UserManager<User> _userManager,
          RoleManager<IdentityRole> _roleManager,
          SignInManager<User> _signinManager,
          ITokenService _tokenService,
          ILogger<UserAppService> _logger,
          IValidator<LoginDto> _loginValidator,
          IValidator<RegisterDto> _registerValidator,
          IValidator<UserDetailsDto> _userDetailsValidator,
          IValidator<UserRole> _userRoleValidator
        )
        : IUserAppService
    {
        public async Task<NewUserDto> Login(LoginDto loginDto)
        {
            _loginValidator.ValidateAndThrow( loginDto );

            _logger.LogInformation("User Logging in with username: {UserName}", loginDto.UserName);

            var user =await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
                throw new NotFoundException("Invalid username!");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
                throw new NotSucceededException("Username not found and/or password incorrect");
            var userRoles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("User logged in with username: {UserName} successfully", loginDto.UserName);

            return new NewUserDto
            (
                user.UserName,
                user.Email,
                _tokenService.CreateToken(user,userRoles)
            );
        }

        public async Task<NewUserDto> Register(RegisterDto registerDto)
        {
            _registerValidator.ValidateAndThrow(registerDto);

            _logger.LogInformation("Creating a new user with username: {UserName}", registerDto.UserName);

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

            if (!createdUser.Succeeded)
                throw new NotSucceededException("User can not created");

            _logger.LogInformation("User with username: {UserName} created successfully", registerDto.UserName);

            return new NewUserDto
            (
                user.UserName,
                user.Email,
                _tokenService.CreateToken(user,new List<string>())
            );
        }

        public async Task AddRole(string role)
        {
            _logger.LogInformation("Creating a new role with title: {role}", role);
            var isExist = await _roleManager.RoleExistsAsync(role);
            if (isExist)
                throw new IsExistException("Role already exist");

            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded)
                throw new NotSucceededException("User can not created");

            _logger.LogInformation("Role with title: {Title} created successfully", role);
        }

        public async Task AssignRole(UserRole role)
        {
            _logger.LogInformation("Assigning a role with title: {role}", role);
            _userRoleValidator.ValidateAndThrow(role);

            var user = await _userManager.FindByNameAsync(role.UserName);

            if (user == null)
                throw new ArgumentNullException("User not found");

            var result = await _userManager.AddToRoleAsync(user,role.Role);

            if (!result.Succeeded)
                throw new NotSucceededException($"Role does not assigned");

            _logger.LogInformation("Role with title: {Title} assigned successfully", role);
        }

        public async Task DeleteUser(string UserName)
        {
            _logger.LogInformation("Deleting user with UserName: {UserName}", UserName);

            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
                throw new NotFoundException("Invalid user!");

            await _userManager.DeleteAsync(user);

            _logger.LogInformation("User with UserName: {UserName} deleted successfully", UserName);
        }

        public async Task UpdateDetails(string userName, UserDetailsDto userDto)
        {
            
            _userDetailsValidator.ValidateAndThrow(userDto);
            _logger.LogInformation("Updating user with UserName: {userName}", userName);

            var user = await _userManager.FindByNameAsync(userName);

            user.PhoneNumber = userDto.PhoneNumber;
            user.Email = userDto.Email;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User with UserName: {userName} updated successfully", userName);
        }
    }
}
