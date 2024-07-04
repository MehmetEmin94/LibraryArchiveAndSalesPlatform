using LibraryArchiveAndSalesPlatform.API.Application.Dtos;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryArchiveAndSalesPlatform.API.Controllers
{
    [Route("api/shelfs")]
    [ApiController]
    [Authorize]
    public class ShelfController(IShelfAppService shelfAppService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShelf(CreateShelfDto shelfDto)
        {
            try
            {
                await shelfAppService.CreateAsync(shelfDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateShelf([FromRoute] Guid id,UpdateShelfDto shelfDto)
        {
            try
            {
                await shelfAppService.UpdateAsync(id,shelfDto);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShelf([FromRoute] Guid id)
        {
            try
            {
                await shelfAppService.DeleteAsync(id);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetShelfList()
        {
            var shelfs = await shelfAppService.GetAllAsync();
            return Ok(shelfs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShelf([FromRoute] Guid id)
        {
            
            try
            {
                var shelf = await shelfAppService.GetAsync(id);
                return Ok(shelf);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
            
        }
    }
}
