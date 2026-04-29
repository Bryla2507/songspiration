using Microsoft.AspNetCore.Mvc;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;

namespace SongSpiration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto registerDto)
        {
            try
            {
                var response = await _userService.RegisterAsync(registerDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas rejestracji." });
            }
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var response = await _userService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas logowania." });
            }
        }

        // GET: api/users/profile/{id}
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<UserDto>> GetProfile(Guid id)
        {
            var profile = await _userService.GetProfileAsync(id);
            if (profile == null)
            {
                return NotFound(new { message = "Użytkownik nie został znaleziony." });
            }
            return Ok(profile);
        }

        // PUT: api/users/profile/{id}
        [HttpPut("profile/{id}")]
        public async Task<ActionResult<UserDto>> UpdateProfile(Guid id, UserDto updateDto)
        {
            try
            {
                var updatedUser = await _userService.UpdateProfileAsync(id, updateDto);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd podczas aktualizacji profilu." });
            }
        }
        
        [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        // Tutaj był błąd - poprawione na użycie serwisu
        var result = await _userService.DeleteAccountAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    }
}