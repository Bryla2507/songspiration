using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.Models.Entities;

namespace SongSpiration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<ActionResult<Genre>> AddGenre(GenreCreateDto genreDto)
        {
            var createdGenre = await _genreService.AddGenreAsync(genreDto);
            return CreatedAtAction(nameof(GetAllGenres), new { id = createdGenre.Id }, createdGenre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
    }
}
