using System;
using System.Linq;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models;
using SongSpiration.Models.Entities;

namespace SongSpiration.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            DisplayName = registerDto.DisplayName,
            PasswordHash = "hashed_" + registerDto.Password, // Simple placeholder hashing
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = "mock_access_token", // In real app, this would be a JWT token
            RefreshToken = "mock_refresh_token",
            User = MapToDto(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // In a real app, we would verify the password hash here
        // For now, we'll just return a mock token

        return new AuthResponseDto
        {
            AccessToken = "mock_access_token_" + user.Id, // Include user ID in token for demo
            RefreshToken = "mock_refresh_token_" + user.Id,
            User = MapToDto(user)
        };
    }

    public async Task<UserDto?> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UserDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        if (updateDto.DisplayName != null)
        {
            user.DisplayName = updateDto.DisplayName;
        }

        if (updateDto.AvatarUrl != null)
        {
            user.AvatarUrl = updateDto.AvatarUrl;
        }

        if (updateDto.Bio != null)
        {
            user.Bio = updateDto.Bio;
        }

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<bool> DeleteAccountAsync(Guid userId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null) return false;

    _userRepository.Delete(user);
    await _userRepository.SaveChangesAsync();
    return true;
}

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Roles = user.Roles,
            CreatedAt = user.CreatedAt
        };
    }
}
