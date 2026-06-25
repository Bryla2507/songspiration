using Moq;
using SongSpiration.BLL.Services;
using SongSpiration.BLL.DTOs;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models;
using SongSpiration.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SongSpiration.Tests.BLL;

// KLASA MUSI BYĆ PUBLICZNA
public class UserProfileAndPinGridTests
{
    private readonly Mock<IPinRepository> _pinRepoMock;
    private readonly PinService _sut; 

    private static readonly Guid TestUserId = new Guid("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = new Guid("22222222-2222-2222-2222-222222222222");
    private static readonly Guid TestPinId = new Guid("33333333-3333-3333-3333-333333333333");

    public UserProfileAndPinGridTests()
    {
        _pinRepoMock = new Mock<IPinRepository>();
        _sut = new PinService(_pinRepoMock.Object); 
    }

    [Fact]
    public async Task GetPinsByUserIdAsync_WhenUserIsOwner_PassesShowPrivateTrueToRepo()
    {
        // Arrange
        var profileUserId = TestUserId;
        var loggedInUserId = TestUserId; 

        _pinRepoMock.Setup(r => r.GetPinsByUserIdAsync(profileUserId, "newest", "desc", true))
            .ReturnsAsync(new List<Pin>());

        // Act
        await _sut.GetPinsByUserIdAsync(profileUserId, "newest", "desc", loggedInUserId);

        // Assert
        _pinRepoMock.Verify(r => r.GetPinsByUserIdAsync(profileUserId, "newest", "desc", true), Times.Once);
    }

    [Fact]
    public async Task GetPinsByUserIdAsync_WhenUserIsNotOwner_PassesShowPrivateFalseToRepo()
    {
        // Arrange
        var profileUserId = TestUserId;
        var loggedInUserId = OtherUserId; 

        _pinRepoMock.Setup(r => r.GetPinsByUserIdAsync(profileUserId, "newest", "desc", false))
            .ReturnsAsync(new List<Pin>());

        // Act
        await _sut.GetPinsByUserIdAsync(profileUserId, "newest", "desc", loggedInUserId);

        // Assert
        _pinRepoMock.Verify(r => r.GetPinsByUserIdAsync(profileUserId, "newest", "desc", false), Times.Once);
    }

    [Fact]
    public async Task UpdatePinAsync_ChangingVisibility_UpdatesAndReturnsTrue()
    {
        // Arrange
        var pinId = TestPinId;
        var existingPin = new Pin 
        { 
            Id = pinId, 
            Title = "My Song", 
            Visibility = PinVisibility.Public 
        };
        
        var updateDto = new UpdatePinDto { Visibility = PinVisibility.Private }; 

        _pinRepoMock.Setup(r => r.GetByIdAsync(pinId)).ReturnsAsync(existingPin);
        _pinRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.UpdatePinAsync(pinId, updateDto);

        // Assert
        Assert.True(result);
        Assert.Equal(PinVisibility.Private, existingPin.Visibility);
        _pinRepoMock.Verify(r => r.Update(existingPin), Times.Once);
    }

    [Fact]
    public async Task ToggleLikeAsync_ValidCall_TriggersRepoAndReturnsResult()
    {
        // Arrange
        var userId = TestUserId;
        var pinId = TestPinId;

        // Repozytorium zwraca Task<bool>
        _pinRepoMock.Setup(r => r.ToggleLikeAsync(userId, pinId)).ReturnsAsync(false);

        // Act
        var result = await _sut.ToggleLikeAsync(userId, pinId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsLiked); 
        _pinRepoMock.Verify(r => r.ToggleLikeAsync(userId, pinId), Times.Once);
    }
}