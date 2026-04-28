using Microsoft.AspNetCore.Mvc;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

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
            if (pin == null)
            {
                return NotFound();
            }
            return Ok(pin);
        }

        [HttpPost]
        public async Task<ActionResult<PinDto>> CreatePin([FromBody] CreatePinDto createPinDto)
        {
            // In a real app, ownerId would come from authenticated user
            var ownerId = Guid.NewGuid(); 
            var createdPin = await _pinService.CreatePinAsync(ownerId, createPinDto);
            return CreatedAtAction(nameof(GetPin), new { id = createdPin.Id }, createdPin);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<PinDto>> UploadPin(
            [FromForm] string title,
            [FromForm] string? description,
            [FromForm] int instrument,
            [FromForm] int visibility,
            [FromForm] string[] genreIds,
            IFormFile file)
        {
            // In a real app, ownerId would come from authenticated user
            var ownerId = Guid.NewGuid();

            // Create the DTO
            var createPinDto = new CreatePinDto
            {
                Title = title,
                Description = description,
                Instrument = (Instrument)instrument,
                Visibility = (PinVisibility)visibility,
                GenreIds = new List<string>(genreIds)
            };

            // Handle file upload
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            // Create uploads directory if it doesn't exist
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Set the temp file location for the service
            createPinDto.TempFileLocation = filePath;

            // Create the pin
            var createdPin = await _pinService.CreatePinAsync(ownerId, createPinDto);

            // Return the file URL
            var fileUrl = $"/uploads/{fileName}";
            createdPin.Filename = fileUrl;

            return CreatedAtAction(nameof(GetPin), new { id = createdPin.Id }, createdPin);
        }

        [HttpGet("files/{filename}")]
        public IActionResult GetFile(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new FileInfo(filePath);
            return File(fileStream, "application/octet-stream", fileInfo.Name);
        }
    }
}
