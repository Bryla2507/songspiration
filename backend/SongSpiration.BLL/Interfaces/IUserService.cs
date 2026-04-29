using System;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetProfileAsync(Guid userId);
    Task<UserDto> UpdateProfileAsync(Guid userId, UserDto updateDto);
    
    // TA LINIA JEST KLUCZOWA:
    Task<bool> DeleteAccountAsync(Guid userId);
}