using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace LibraryArchiveAndSalesPlatform.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    [Authorize]
    public class AccountController(IUserAppService userService) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await userService.Login(loginDto);

                return Ok( user);
            }
            catch (Exception ex) 
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await userService.Register(registerDto);
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("add-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole([FromBody]string Role)
        {
            try
            {
                await userService.AddRole(Role);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
            
            return Ok();
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole role)
        {
            try
            {
                await userService.AssignRole(role);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

            return Ok();
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            try
            {
                await userService.DeleteUser(username);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

            return Ok();
        }

        [HttpPut("user-details/{userName}")]
        public async Task<IActionResult> UpdateUserDetails([FromRoute] string userName,[FromBody] UserDetailsDto user)
        {
            try
            {
                await userService.UpdateDetails(userName,user);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

            return Ok();
        }
    }
}
