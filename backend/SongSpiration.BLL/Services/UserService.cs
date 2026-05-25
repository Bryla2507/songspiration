using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL;
using SongSpiration.DAL.Interfaces;
using EntitiesUser = SongSpiration.Models.Entities.User;
using EnumsTokenType = SongSpiration.Models.TokenType;

namespace SongSpiration.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPinRepository _pinRepository;
    private readonly IEmailSender _emailSender;
    private readonly SongSpirationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IPinRepository pinRepository, IEmailSender emailSender, SongSpirationDbContext dbContext, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _pinRepository = pinRepository;
        _emailSender = emailSender;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserProfileDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            Email = user.Email,
            Bio = user.Bio,
            AddedPinsCount = await _pinRepository.GetCountByUserIdAsync(userId),
            TotalLikesReceived = await _pinRepository.GetTotalLikesReceivedByUserIdAsync(userId)
        };
    }

    public async Task<bool> UpdateProfileAsync(Guid id, UpdateUserDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;
    
        user.DisplayName = updateDto.DisplayName;
        user.Email = updateDto.Email; 
        user.Bio = updateDto.Bio;
        user.AvatarUrl = updateDto.AvatarUrl;
    
        _userRepository.Update(user);
        return await _userRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAvatarAsync(Guid userId, string avatarUrl)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.AvatarUrl = avatarUrl;
        _userRepository.Update(user);
        return await _userRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAccountAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        _userRepository.Delete(user);
        return await _userRepository.SaveChangesAsync() > 0;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null) throw new InvalidOperationException("Użytkownik o tym adresie e-mail już istnieje.");

        var user = new EntitiesUser
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            DisplayName = registerDto.DisplayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            CreatedAt = DateTime.UtcNow,
            Roles = "User"
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            RefreshToken = "mock_refresh_token",
            User = MapToDto(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Nieprawidłowy e-mail lub hasło.");
        }

        user.LastLogin = DateTime.UtcNow;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            RefreshToken = "mock_refresh_token_" + user.Id,
            User = MapToDto(user)
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) return;

        var resetToken = Guid.NewGuid().ToString("N");

        var authToken = new SongSpiration.Models.Entities.AuthToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = resetToken,
            TokenType = EnumsTokenType.PasswordReset,
            ExpiryDate = DateTime.UtcNow.AddHours(1),
            IsRevoked = false
        };

        _dbContext.AuthTokens.Add(authToken);
        await _dbContext.SaveChangesAsync();

        var frontendUrl = _configuration["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:5173";
        var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";
        var message = $"<p>Reset Password: <a href='{resetLink}'>Link</a></p>";

        if (_emailSender != null) await _emailSender.SendEmailAsync(user.Email, "SongSpiration - Password Reset", message);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var tokenRecord = await _dbContext.AuthTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.TokenHash == dto.Token &&
                t.TokenType == EnumsTokenType.PasswordReset &&
                !t.IsRevoked &&
                t.ExpiryDate > DateTime.UtcNow);

        if (tokenRecord == null)
        {
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        tokenRecord.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        tokenRecord.IsRevoked = true;

        _dbContext.Users.Update(tokenRecord.User);
        _dbContext.AuthTokens.Update(tokenRecord);
        await _dbContext.SaveChangesAsync();
    }

    private string GenerateJwtToken(EntitiesUser user)
    {
        var jwtSecret = _configuration["JwtSettings:Secret"] ?? "TWOJ_BARDZO_DLUGI_I_TAJNY_KLUCZ_MIN_32_ZNAKI";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Roles ?? "User")
        };

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserDto MapToDto(EntitiesUser user)
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