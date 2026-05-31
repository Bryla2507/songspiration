using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Services;
using SongSpiration.DAL;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using Xunit;
using SongSpiration.BLL.Interfaces;

namespace SongSpiration.Tests.BLL;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IPinRepository> _mockPinRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockPinRepository = new Mock<IPinRepository>();

        // Setup mock configuration for JWT Secret
        _mockConfiguration.Setup(x => x["JwtSettings:Secret"])
            .Returns("TEST_BARDZO_DLUGI_I_TAJNY_KLUCZ_MIN_32_ZNAKI");

        // Pass null for DbContext since Register/Login don't use it directly (they use repository)
        _userService = new UserService(
            _mockUserRepository.Object,
            _mockPinRepository.Object,
            _mockEmailSender.Object,
            null!, // Assuming DbContext is not used in Register/Login
            _mockConfiguration.Object
        );
    }

    private SongSpirationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<SongSpirationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new SongSpirationDbContext(options);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldGenerateTokenAndSendEmail_WhenUserExists()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var user = new User { Id = Guid.NewGuid(), Email = "reset@example.com" };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var service = new UserService(
            _mockUserRepository.Object,
            _mockPinRepository.Object,
            _mockEmailSender.Object,
            dbContext,
            _mockConfiguration.Object
        );

        var dto = new ForgotPasswordDto { Email = "reset@example.com" };

        // Act
        await service.ForgotPasswordAsync(dto);

        // Assert
        var token = await dbContext.AuthTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
        Assert.NotNull(token);
        Assert.Equal(SongSpiration.Models.TokenType.PasswordReset, token.TokenType);
        _mockEmailSender.Verify(x => x.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldUpdatePasswordAndRevokeToken_WhenTokenIsValid()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var user = new User { Id = Guid.NewGuid(), Email = "reset@example.com", PasswordHash = "oldHash" };
        dbContext.Users.Add(user);
        
        var authToken = new AuthToken 
        { 
            Id = Guid.NewGuid(), 
            UserId = user.Id, 
            User = user,
            TokenHash = "valid-token", 
            TokenType = SongSpiration.Models.TokenType.PasswordReset, 
            ExpiryDate = DateTime.UtcNow.AddHours(1),
            IsRevoked = false 
        };
        dbContext.AuthTokens.Add(authToken);
        await dbContext.SaveChangesAsync();

        var service = new UserService(
            _mockUserRepository.Object,
            _mockPinRepository.Object,
            _mockEmailSender.Object,
            dbContext,
            _mockConfiguration.Object
        );

        var dto = new ResetPasswordDto { Token = "valid-token", NewPassword = "NewPassword123!" };

        // Act
        await service.ResetPasswordAsync(dto);

        // Assert
        var updatedUser = await dbContext.Users.FindAsync(user.Id);
        var updatedToken = await dbContext.AuthTokens.FindAsync(authToken.Id);
        
        Assert.NotEqual("oldHash", updatedUser!.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(dto.NewPassword, updatedUser.PasswordHash));
        Assert.True(updatedToken!.IsRevoked);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrowException_WhenTokenIsInvalidOrExpired()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        
        var service = new UserService(
            _mockUserRepository.Object,
            _mockPinRepository.Object,
            _mockEmailSender.Object,
            dbContext,
            _mockConfiguration.Object
        );

        var dto = new ResetPasswordDto { Token = "invalid-token", NewPassword = "NewPassword123!" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ResetPasswordAsync(dto));
        Assert.Equal("Invalid or expired reset token.", exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenEmailExists()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            DisplayName = "Test User"
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(new User { Email = registerDto.Email });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterAsync(registerDto));
        Assert.Equal("Użytkownik o tym adresie e-mail już istnieje.", exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnToken_WhenDataIsValid()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            DisplayName = "New User"
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.Equal("newuser@example.com", result.User.Email);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(loginDto));
        Assert.Equal("Nieprawidłowy e-mail lub hasło.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@example.com",
            Password = "WrongPassword!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = loginDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!")
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(loginDto));
        Assert.Equal("Nieprawidłowy e-mail lub hasło.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@example.com",
            Password = "CorrectPassword!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = loginDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password),
            Roles = "User"
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.Equal(user.Email, result.User.Email);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
