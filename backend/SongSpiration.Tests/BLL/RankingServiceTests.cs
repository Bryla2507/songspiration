using Moq;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Services;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SongSpiration.Tests.BLL;

public class RankingServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly RankingService _rankingService;

    public RankingServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _rankingService = new RankingService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetEditorsChoiceAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = userId,
                DisplayName = "Editor Choice User",
                IsEditorChoice = true,
                Pins = new List<Pin>()
            }
        };

        _userRepositoryMock.Setup(repo => repo.GetEditorChoiceUsersAsync(10))
            .ReturnsAsync(users);

        // Act
        var result = await _rankingService.GetEditorsChoiceAsync();

        // Assert
        Assert.NotNull(result);
        var dtos = result.ToList();
        Assert.Single(dtos);
        Assert.Equal(userId, dtos[0].Id);
        Assert.Equal("Editor Choice User", dtos[0].DisplayName);
        Assert.True(dtos[0].IsEditorChoice);
    }

    [Fact]
    public async Task GetTopByDownloadsAsync_ShouldCalculateTotalDownloads()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = userId,
                DisplayName = "Top Download User",
                Pins = new List<Pin>
                {
                    new Pin { DownloadsCount = 5 },
                    new Pin { DownloadsCount = 15 }
                }
            }
        };

        _userRepositoryMock.Setup(repo => repo.GetUsersByMostDownloadsAsync(10))
            .ReturnsAsync(users);

        // Act
        var result = await _rankingService.GetTopByDownloadsAsync();

        // Assert
        Assert.NotNull(result);
        var dtos = result.ToList();
        Assert.Single(dtos);
        Assert.Equal(20, dtos[0].TotalDownloads);
    }

    [Fact]
    public async Task GetTopByLikesAsync_ShouldCalculateTotalLikes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = userId,
                DisplayName = "Top Liked User",
                Pins = new List<Pin>
                {
                    new Pin { Likes = new List<Like> { new Like(), new Like() } },
                    new Pin { Likes = new List<Like> { new Like() } }
                }
            }
        };

        _userRepositoryMock.Setup(repo => repo.GetUsersByMostLikesAsync(10))
            .ReturnsAsync(users);

        // Act
        var result = await _rankingService.GetTopByLikesAsync();

        // Assert
        Assert.NotNull(result);
        var dtos = result.ToList();
        Assert.Single(dtos);
        Assert.Equal(3, dtos[0].TotalLikes);
    }
}
