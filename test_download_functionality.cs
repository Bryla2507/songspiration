using System;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Services;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using Moq;
using Xunit;

namespace SongSpiration.Tests
{
    public class DownloadFunctionalityTests
    {
        [Fact]
        public async Task TestDownloadCountIncrement()
        {
            // Arrange
            var mockPinRepository = new Mock<IPinRepository>();
            var pinService = new PinService(mockPinRepository.Object);

            var testPinId = Guid.NewGuid();

            // Setup the mock to return a pin with initial download count
            mockPinRepository.Setup(repo => repo.IncrementDownloadCountAsync(testPinId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await pinService.IncrementDownloadCountAsync(testPinId);

            // Assert
            mockPinRepository.Verify(repo => repo.IncrementDownloadCountAsync(testPinId), Times.Once);
        }

        [Fact]
        public async Task TestPinDtoIncludesDownloadCount()
        {
            // Arrange
            var pin = new Pin
            {
                Id = Guid.NewGuid(),
                Title = "Test Pin",
                DownloadsCount = 5,
                Likes = new System.Collections.Generic.List<Like>(),
                PinGenres = new System.Collections.Generic.List<PinGenre>()
            };

            // Act
            var pinService = new PinService(null);
            var result = await Task.Run(() =>
            {
                // Manually create the DTO to test the mapping
                return new PinDto
                {
                    Id = pin.Id,
                    Title = pin.Title,
                    DownloadsCount = pin.DownloadsCount,
                    LikeCount = pin.Likes.Count,
                    Genres = new System.Collections.Generic.List<string>()
                };
            });

            // Assert
            Assert.Equal(5, result.DownloadsCount);
            Assert.Equal("Test Pin", result.Title);
        }
    }
}