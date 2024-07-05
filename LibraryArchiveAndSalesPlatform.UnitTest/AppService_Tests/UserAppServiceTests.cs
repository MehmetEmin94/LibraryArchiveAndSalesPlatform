using Xunit;
using Moq;
using FluentValidation;
using Microsoft.Extensions.Logging;
using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.AspNetCore.Identity;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.CustomExceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace LibraryArchiveAndSalesPlatform.UnitTests.AppService_Tests
{
    public class UserAppServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<UserAppService>> _loggerMock;
        private readonly Mock<IValidator<LoginDto>> _loginValidatorMock;
        private readonly Mock<IValidator<RegisterDto>> _registerValidatorMock;
        private readonly Mock<IValidator<UserDetailsDto>> _userDetailsValidatorMock;
        private readonly Mock<IValidator<UserRole>> _userRoleValidatorMock;
        private readonly UserAppService _userAppService;

        public UserAppServiceTests()
        {
            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();
            _signInManagerMock = MockSignInManager();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<UserAppService>>();
            _loginValidatorMock = new Mock<IValidator<LoginDto>>();
            _registerValidatorMock = new Mock<IValidator<RegisterDto>>();
            _userDetailsValidatorMock = new Mock<IValidator<UserDetailsDto>>();
            _userRoleValidatorMock = new Mock<IValidator<UserRole>>();

            _userAppService = new UserAppService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object,
                _loginValidatorMock.Object,
                _registerValidatorMock.Object,
                _userDetailsValidatorMock.Object,
                _userRoleValidatorMock.Object
            );
        }

        // Mocking UserManager<User>
        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        // Mocking RoleManager<IdentityRole>
        private static Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        }

        // Mocking SignInManager<User>
        private static Mock<SignInManager<User>> MockSignInManager()
        {
            var userManager = MockUserManager();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            return new Mock<SignInManager<User>>(userManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }

        [Fact]
        public async Task Login_Should_Return_NewUserDto_When_Valid_Credentials()
        {
            // Arrange
            var loginDto = new LoginDto ("testuser", "Password_123");
            var user = new User { UserName = "testuser",PasswordHash= "Password_123", Email = "testuser@example.com" };

            _loginValidatorMock.Setup(v => v.ValidateAndThrow(loginDto));
            _userManagerMock.Setup(um => um.FindByNameAsync("testuser")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "Password_123")).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _tokenServiceMock.Setup(ts => ts.CreateToken(user, It.IsAny<IList<string>>())).Returns("token");

            // Act
            var result = await _userAppService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("testuser");
            result.Email.Should().Be("testuser@example.com");
            result.Token.Should().Be("token");
        }

        [Fact]
        public async Task Register_Should_Throw_NotSucceededException_When_User_Creation_Fails()
        {
            // Arrange
            var registerDto = new RegisterDto("testuser", "testuser@example.com", "Password_123");
            var user = new User { UserName = "testuser", PasswordHash= "Password_123", Email = "testuser@example.com" };

            _registerValidatorMock.Setup(v => v.ValidateAndThrow(registerDto));
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), "Password_123")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act & Assert
            await Assert.ThrowsAsync<NotSucceededException>(() => _userAppService.Register(registerDto));
        }

        [Fact]
        public async Task AddRole_Should_Throw_IsExistException_When_Role_Already_Exists()
        {
            // Arrange
            var role = "Admin";

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<IsExistException>(() => _userAppService.AddRole(role));
        }

        [Fact]
        public async Task AssignRole_Should_Throw_ArgumentNullException_When_User_Not_Found()
        {
            // Arrange
            var userRole = new UserRole ( "nonexistentuser", "User" );

            _userRoleValidatorMock.Setup(v => v.ValidateAndThrow(userRole));
            _userManagerMock.Setup(um => um.FindByNameAsync("nonexistentuser")).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userAppService.AssignRole(userRole));
        }

        [Fact]
        public async Task DeleteUser_Should_Throw_NotFoundException_When_User_Not_Found()
        {
            // Arrange
            var userName = "nonexistentuser";

            _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userAppService.DeleteUser(userName));
        }

        [Fact]
        public async Task UpdateDetails_Should_Update_User_Details()
        {
            // Arrange
            var userName = "testuser";
            var userDetailsDto = new UserDetailsDto (  "newemail@example.com", "1234567890");
            var user = new User { UserName = "testuser", Email = "testuser@example.com",PhoneNumber = "1234567890" };

            _userDetailsValidatorMock.Setup(v => v.ValidateAndThrow(userDetailsDto));
            _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userAppService.UpdateDetails(userName, userDetailsDto);

            // Assert
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }
    }
}
