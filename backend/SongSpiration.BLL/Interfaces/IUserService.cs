using System;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    Task<bool> UpdateProfileAsync(Guid userId, UpdateUserDto updateDto);
    Task<bool> DeleteAccountAsync(Guid userId);
    Task<bool> UpdateAvatarAsync(Guid userId, string avatarUrl);
    Task ForgotPasswordAsync(ForgotPasswordDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
    Task<IEnumerable<UserProfileDto>> GetAllUsersAsync();
}
