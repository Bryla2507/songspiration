using Microsoft.AspNetCore.Mvc;
using SongSpiration.BLL.Services;
using SongSpiration.Models;
using SongSpiration.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongSpiration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPanelController : ControllerBase
    {
        private readonly AdminPanelService _adminPanelService;

        public AdminPanelController(AdminPanelService adminPanelService)
        {
            _adminPanelService = adminPanelService;
        }

        [HttpGet("genres")]
        public async Task<ActionResult<List<Genre>>> GetAllGenresAsync()
        {
            var genres = await _adminPanelService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("genres/{id}")]
        public async Task<ActionResult<Genre>> GetGenreByIdAsync(Guid id)
        {
            var genre = await _adminPanelService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return Ok(genre);
        }

        [HttpPost("genres")]
        public async Task<ActionResult<Genre>> CreateGenreAsync([FromBody] GenreCreateDto genreCreateDto)
        {
            var genre = await _adminPanelService.CreateGenreAsync(genreCreateDto);
            return CreatedAtAction(nameof(GetGenreByIdAsync), new { id = genre.Id }, genre);
        }

        [HttpPut("genres/{id}")]
        public async Task<ActionResult<Genre>> UpdateGenreAsync(Guid id, [FromBody] GenreCreateDto genreCreateDto)
        {
            await _adminPanelService.UpdateGenreAsync(id, genreCreateDto);
            return NoContent();
        }

        [HttpDelete("genres/{id}")]
        public async Task<ActionResult> DeleteGenreAsync(Guid id)
        {
            await _adminPanelService.DeleteGenreAsync(id);
            return NoContent();
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<User>>> SearchUsersAsync([FromQuery] string criteria)
        {
            var users = await _adminPanelService.SearchUsersAsync(criteria);
            return Ok(users);
        }

        [HttpDelete("users/{userId}")]
        public async Task<ActionResult> DeleteUserAsync(Guid userId)
        {
            await _adminPanelService.DeleteUserAsync(userId);
            return NoContent();
        }

        [HttpPut("users/{userId}/ban")]
        public async Task<ActionResult> BanUserAsync(Guid userId)
        {
            await _adminPanelService.BanUserAsync(userId);
            return NoContent();
        }

        [HttpDelete("users/{userId}/pins")]
        public async Task<ActionResult> DeletePinsForUserAsync(Guid userId)
        {
            await _adminPanelService.DeletePinsForUserAsync(userId);
            return NoContent();
        }

        [HttpPut("users/{userId}/editor-choice")]
        public async Task<ActionResult> SetEditorChoiceAsync(Guid userId, [FromBody] bool isEditorChoice)
        {
            await _adminPanelService.SetEditorChoiceAsync(userId, isEditorChoice);
            return NoContent();
        }
    }
}
