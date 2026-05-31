using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;

namespace SongSpiration.API.Controllers
{
[ApiController]
[Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto registerDto)
        {
            try
            {
                var response = await _userService.RegisterAsync(registerDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch { return StatusCode(500, new { message = "Błąd rejestracji." }); }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var response = await _userService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch { return StatusCode(500, new { message = "Błąd logowania." }); }
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, UpdateUserDto updateDto)
        {
            try
            {
                var success = await _userService.UpdateProfileAsync(id, updateDto);
                if (!success) return NotFound();
                return Ok(new { message = "Profil zaktualizowany." });
            }
            catch { return StatusCode(500, new { message = "Błąd aktualizacji." }); }
        }

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(Guid id)
        {
            var profile = await _userService.GetUserProfileAsync(id);
            if (profile == null) return NotFound(new { message = "Użytkownik nie znaleziony." });
            return Ok(profile);
        }
    
        [Authorize]
        [HttpPost("profile/{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0) return BadRequest("Nie przesłano pliku.");
            
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, fileName);
    
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }
    
            var relativePath = $"/avatars/{fileName}";
            var success = await _userService.UpdateAvatarAsync(id, relativePath);
            if (!success) return NotFound();
    
            return Ok(new { url = relativePath });
        }
            
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var result = await _userService.DeleteAccountAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            try
            {
                await _userService.ForgotPasswordAsync(dto);
                return Ok(new { message = "Jeśli email istnieje, link został wysłany." });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = $"Błąd wysyłania e-maila: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd po stronie serwera." });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            try
            {
                await _userService.ResetPasswordAsync(dto);
                return Ok(new { message = "Hasło zostało zmienione." });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch { return StatusCode(500, new { message = "Błąd resetowania hasła." }); }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAllUsers()
        {
            try
            {
                Console.WriteLine("GetAllUsers endpoint called.");
                var users = await _userService.GetAllUsersAsync();
                Console.WriteLine($"Fetched {users.Count()} users.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania użytkowników." });
            }
        }
    }
}
