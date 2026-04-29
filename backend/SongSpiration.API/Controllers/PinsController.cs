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
        public async Task<ActionResult<IEnumerable<PinDto>>> GetPins()
        {
            var pins = await _pinService.GetAllPinsAsync();
            return Ok(pins);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PinDto>> GetPin(Guid id)
        {
            var pin = await _pinService.GetPinByIdAsync(id);
            if (pin == null) return NotFound();
            return Ok(pin);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<PinDto>> UploadPin(
            [FromForm] string title,
            [FromForm] string? description,
            [FromForm] int instrument,
            [FromForm] int visibility,
            [FromForm(Name = "genreIds")] List<Guid> genreIds,
            IFormFile file)
        {
            var ownerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");

            // 1. Przygotowanie DTO
            var createPinDto = new CreatePinDto
            {
                Title = title,
                Description = description,
                Instrument = (Instrument)instrument,
                Visibility = (PinVisibility)visibility,
                GenreIds = genreIds?.ToList() ?? new List<Guid>()
            };

            // 2. Walidacja pliku
            if (file == null || file.Length == 0)
            {
                return BadRequest("Plik jest wymagany.");
            }

            // 3. Zapis pliku na dysku (to Ci działa)
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            createPinDto.TempFileLocation = filePath;

            // 4. Próba zapisu do bazy z rozbudowaną diagnostyką
            try 
            {
                var createdPin = await _pinService.CreatePinAsync(ownerId, createPinDto);
                createdPin.Filename = $"/uploads/{fileName}"; 
                
                return CreatedAtAction(nameof(GetPin), new { id = createdPin.Id }, createdPin);
            }
            catch (Exception ex)
            {
                // Wyciągamy najgłębszy błąd (np. z SQLite)
                var innerMessage = ex.InnerException?.Message ?? "Brak szczegółów (InnerException jest null)";
                
                // Logujemy do konsoli (sprawdź okno terminala!)
                Console.WriteLine("========== BŁĄD ZAPISU DO BAZY ==========");
                Console.WriteLine($"Główny błąd: {ex.Message}");
                Console.WriteLine($"Szczegóły SQL: {innerMessage}");
                Console.WriteLine("=========================================");

                // Zwracamy detale do Swaggera
                return StatusCode(500, new {
                    error = "Błąd bazy danych",
                    details = innerMessage,
                    originalMessage = ex.Message
                });
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
            // 1. Pobieramy dane o pinie, żeby znać ścieżkę do pliku
            var pin = await _pinService.GetPinByIdAsync(id);
            if (pin == null) return NotFound();

            // 2. Próba usunięcia z bazy
            var success = await _pinService.DeletePinAsync(id);
            if (!success) return BadRequest("Nie udało się usunąć pina z bazy.");

            // 3. Usuwanie fizycznego pliku z dysku
            if (!string.IsNullOrEmpty(pin.Filename))
            {
                // Musimy wyłuskać samą nazwę pliku, jeśli Filename zawiera ścieżkę "/uploads/..."
                var fileNameOnly = Path.GetFileName(pin.Filename);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileNameOnly);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
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
    }
}