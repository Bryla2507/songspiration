using Microsoft.AspNetCore.Mvc;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;

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
    }
}
