using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchiveAndSalesPlatform.API.Controllers
{
    [Route("api/notes")]
    [ApiController]
    [Authorize]
    public class NoteController
        (INoteAppService noteAppService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateNote(CreateNoteDto noteDto)
        {
            try
            {
                await noteAppService.CreateAsync(noteDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            try
            {
                await noteAppService.DeleteAsync(id);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(Guid id,UpdateNoteDto noteDto)
        {
            try
            {
                await noteAppService.UpdateAsync(id,noteDto);
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

        [HttpGet("notes-with-filtering")]
        public async Task<IActionResult> GetAllNotes([FromQuery] QueryObject query)
        {
            var notes = await noteAppService.GetAllAsync(query);
            return Ok(notes);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote([FromRoute] Guid id)
        {
            try
            {
                var note = await noteAppService.GetAsync(id);
                return Ok(note);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
        }


        [HttpGet("change-note-privacy/{noteId}")]
        public async Task<IActionResult> ChangeNotePrivacy([FromRoute] Guid noteId, [FromQuery] PrivacySetting privacySetting)
        {
            try
            {
                await noteAppService.ChangeNotePrivacyAsync(noteId, privacySetting);
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
    }
}
