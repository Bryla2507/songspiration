using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SongSpiration.BLL.Services;
using SongSpiration.BLL.Interfaces;
using SongSpiration.Models;
using SongSpiration.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntitiesUser = SongSpiration.Models.Entities.User;
using ModelsGenre = SongSpiration.Models.Genre;

namespace SongSpiration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : ControllerBase
    {
        private readonly IAdminPanelService _adminPanelService;

        public AdminPanelController(IAdminPanelService adminPanelService)
        {
            _adminPanelService = adminPanelService;
        }

        // Tymczasowy endpoint bez autoryzacji do testów
        [HttpGet("test/genres")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ModelsGenre>>> TestGetAllGenresAsync()
        {
            var genres = await _adminPanelService.GetAllGenresAsync();
            return Ok(genres);
        }

        // Tymczasowy endpoint bez autoryzacji do testów
        [HttpGet("all-users")]
        public async Task<ActionResult<List<EntitiesUser>>> GetUsersAsync([FromQuery] string criteria)
        {
            var users = await _adminPanelService.SearchUsersAsync(criteria);
            return Ok(users);
        }

        [HttpGet("genres")]
        public async Task<ActionResult<List<ModelsGenre>>> GetAllGenresAsync()
        {
            var genres = await _adminPanelService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("genres/{id}")]
        public async Task<ActionResult<ModelsGenre>> GetGenreByIdAsync(Guid id)
        {
            var genre = await _adminPanelService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return Ok(genre);
        }

        [HttpPost("genres")]
        public async Task<ActionResult<ModelsGenre>> CreateGenreAsync([FromBody] GenreCreateDto genreCreateDto)
        {
            var genre = await _adminPanelService.CreateGenreAsync(genreCreateDto);
            return CreatedAtAction(nameof(GetGenreByIdAsync), new { id = genre.Id }, genre);
        }

        [HttpPut("genres/{id}")]
        public async Task<ActionResult> UpdateGenreAsync(Guid id, [FromBody] GenreCreateDto genreCreateDto)
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
        public async Task<ActionResult<IEnumerable<EntitiesUser>>> GetAllUsersAsync()
        {
            var users = await _adminPanelService.GetAllUsersAsync();
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

        [HttpPut("users/{userId}/unban")]
        public async Task<ActionResult> UnbanUserAsync(Guid userId)
        {
            await _adminPanelService.UnbanUserAsync(userId);
            return NoContent();
        }

        [HttpPut("users/{userId}/promote")]
        public async Task<ActionResult> PromoteToAdminAsync(Guid userId)
        {
            await _adminPanelService.PromoteToAdminAsync(userId);
            return NoContent();
        }

        [HttpPut("users/{userId}/demote")]
        public async Task<ActionResult> DemoteFromAdminAsync(Guid userId)
        {
            await _adminPanelService.DemoteFromAdminAsync(userId);
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