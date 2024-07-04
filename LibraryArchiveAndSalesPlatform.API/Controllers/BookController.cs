using Microsoft.AspNetCore.Mvc;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;

namespace LibraryArchiveAndSalesPlatform.API.Controllers
{
    [Route("api/books")]
    [ApiController]
    [Authorize]
    public class BookController(IBookAppService bookAppService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookDto bookDto)
        {
            try
            {
                await bookAppService.CreateAsync(bookDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                await bookAppService.DeleteAsync(id);
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


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(Guid id,[FromForm] UpdateBookDto bookDto)
        {
            try
            {
                await bookAppService.UpdateAsync(id,bookDto);
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


        [HttpGet("books-with-filtering")]
        public async Task<IActionResult> GetAllBooks([FromQuery] BookQueryObject query)
        {
            var books = await bookAppService.GetAllAsync(query);
            return Ok(books);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook([FromRoute] Guid id)
        {

            try
            {
               var book = await bookAppService.GetAsync(id);
                return Ok(book);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
            
        }
        [HttpGet("book-image/{id}")]
        public async Task<IActionResult> GetBookImage([FromRoute] Guid id)
        {
            
            try
            {
                var image = await bookAppService.GetBookImageAsync(id);
                return File(image.Content, image.ContentType, image.Name);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
           
        }
    }
}
