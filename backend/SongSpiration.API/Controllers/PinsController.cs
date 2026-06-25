using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.Models;
using SongSpiration.Models.Entities;

namespace SongSpiration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PinsController : ControllerBase
    {
        private readonly IPinService _pinService;

        public PinsController(IPinService pinService)
        {
            _pinService = pinService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PinDto>>> GetPins(
            [FromQuery] int start = 0, 
            [FromQuery] int limit = 6,
            [FromQuery] string? search = null,
            [FromQuery] string? instrument = null,
            [FromQuery] string? genre = null,
            [FromQuery] string? sortBy = "newest",
            [FromQuery] string? sortOrder = "desc")
        {
            var pins = await _pinService.GetAllPinsAsync(start, limit, search, instrument, genre, sortBy, sortOrder);
            return Ok(pins);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PinDto>> GetPin(Guid id)
        {
            var pin = await _pinService.GetPinByIdAsync(id);
            if (pin == null) return NotFound();
            return Ok(pin);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<ActionResult<PinDto>> UploadPin(
            [FromForm] string title,
            [FromForm] string? description,
            [FromForm] int instrument,
            [FromForm] int visibility,
            [FromForm(Name = "genreIds")] List<Guid> genreIds,
            IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                             ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var ownerId = Guid.Parse(userIdClaim);

            if (file == null || file.Length == 0) return BadRequest("Plik jest wymagany.");

            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDirectory)) Directory.CreateDirectory(uploadsDirectory);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{fileName}";

            var createPinDto = new CreatePinDto
            {
                Title = title,
                Description = description,
                Instrument = (Instrument)instrument,
                Visibility = (PinVisibility)visibility,
                GenreIds = genreIds?.ToList() ?? new List<Guid>(),
                TempFileLocation = relativePath 
            };

            try 
            {
                var createdPin = await _pinService.CreatePinAsync(ownerId, createPinDto);
                return CreatedAtAction(nameof(GetPin), new { id = createdPin.Id }, createdPin);
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                var innerMessage = ex.InnerException?.Message ?? "Brak szczegółów";
                return StatusCode(500, new { error = "Błąd bazy danych", details = innerMessage });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PinDto>> PutPin(Guid id, [FromBody] UpdatePinDto updateDto)
        {
            try
            {
                var updatedPin = await _pinService.UpdatePinAsync(id, updateDto);
                return Ok(updatedPin);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Pin o ID {id} nie istnieje.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Błąd podczas aktualizacji", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePin(Guid id)
        {
            var pin = await _pinService.GetPinByIdAsync(id);
            if (pin == null) return NotFound();

            var success = await _pinService.DeletePinAsync(id);
            if (!success) return BadRequest("Nie udało się usunąć pina z bazy.");

            if (!string.IsNullOrEmpty(pin.Filename))
            {
                var fileNameOnly = Path.GetFileName(pin.Filename);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileNameOnly);
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            return NoContent();
        }

        [HttpGet("files/{filename}")]
        public IActionResult GetFile(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", filename);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPinFile(Guid id)
        {
            var pin = await _pinService.GetPinByIdAsync(id);
            if (pin == null || string.IsNullOrEmpty(pin.Filename)) return NotFound();

            var fileNameOnly = Path.GetFileName(pin.Filename);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileNameOnly);
            
            if (!System.IO.File.Exists(filePath)) return NotFound();

            await _pinService.IncrementDownloadCountAsync(id);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", pin.Filename);
        }

        [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PinDto>>> GetUserPins(
    Guid userId,
    [FromQuery] string? sortBy = "newest",
    [FromQuery] string? sortOrder = "desc")
    {
        try
        {
            Guid? currentUserId = null;
            var nameIdentifier = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(nameIdentifier, out var parsedId))
            {
                currentUserId = parsedId;
            }

            var pins = await _pinService.GetPinsByUserIdAsync(userId, sortBy, sortOrder, currentUserId);
            return Ok(pins);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Błąd podczas pobierania pinów użytkownika." });
        }
    }

        [HttpPost("{id}/toggle-like")]
        public async Task<IActionResult> ToggleLike(Guid id, [FromQuery] Guid? userId)
        {
            var authUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                             ?? User.FindFirst("sub")?.Value;

            Guid effectiveUserId;

            if (!string.IsNullOrEmpty(authUserIdClaim))
                effectiveUserId = Guid.Parse(authUserIdClaim);
            else if (userId.HasValue)
                effectiveUserId = userId.Value;
            else
                return BadRequest(new { error = "Brak identyfikatora użytkownika lub gościa." });

            try
            {
                var result = await _pinService.ToggleLikeAsync(effectiveUserId, id);
                return Ok(new { isLiked = result.IsLiked, likeCount = result.LikeCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Błąd podczas przełączania polubienia", message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/liked")]
        public async Task<ActionResult<IEnumerable<PinDto>>> GetUserLikedPins(
            Guid userId,
            [FromQuery] string sortBy = "newest",
            [FromQuery] string sortOrder = "desc")
        {
            try
            {
                // Wywołujemy serwis, przekazując filtry sortowania
                var likedPins = await _pinService.GetLikedPinsByUserIdAsync(userId, sortBy, sortOrder);
                return Ok(likedPins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd podczas pobierania polubionych pinów użytkownika.", details = ex.Message });
            }
        }
    }
}