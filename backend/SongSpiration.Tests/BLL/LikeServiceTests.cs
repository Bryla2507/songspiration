using Moq;
using AutoMapper;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using SongSpiration.BLL.Services;
using SongSpiration.BLL.DTOs;
using Xunit;

namespace SongSpiration.Tests.BLL
{
    public class LikeServiceTests
    {
        private readonly Mock<ILikeRepository> _mockLikeRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LikeService _likeService;

        public LikeServiceTests()
        {
            _mockLikeRepository = new Mock<ILikeRepository>();
            _mockMapper = new Mock<IMapper>();
            _likeService = new LikeService(_mockLikeRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Like_ShouldAddLikeToRepository()
        {
            // Arrange
            var pinId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            await _likeService.Like(pinId, userId);

            // Assert
            _mockLikeRepository.Verify(repo => repo.Add(It.Is<Like>(l =>
                l.PinId == pinId &&
                l.UserId == userId &&
                l.CreatedAt <= DateTime.UtcNow)), Times.Once);
        }

        [Fact]
        public async Task Unlike_ShouldRemoveLikeFromRepository()
        {
            // Arrange
            var pinId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var like = new Like { PinId = pinId, UserId = userId };

            _mockLikeRepository.Setup(repo => repo.GetByPinIdAndUserId(pinId, userId))
                .ReturnsAsync(like);

            // Act
            await _likeService.Unlike(pinId, userId);

            // Assert
            _mockLikeRepository.Verify(repo => repo.Delete(like), Times.Once);
        }

        [Fact]
        public async Task IsLiked_ShouldReturnTrue_WhenLikeExists()
        {
            // Arrange
            var pinId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var like = new Like { PinId = pinId, UserId = userId };

            _mockLikeRepository.Setup(repo => repo.GetByPinIdAndUserId(pinId, userId))
                .ReturnsAsync(like);

            // Act
            var result = await _likeService.IsLiked(pinId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsLiked_ShouldReturnFalse_WhenLikeDoesNotExist()
        {
            // Arrange
            var pinId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockLikeRepository.Setup(repo => repo.GetByPinIdAndUserId(pinId, userId))
                .ReturnsAsync((Like)null);

            // Act
            var result = await _likeService.IsLiked(pinId, userId);

            // Assert
            Assert.False(result);
        }
    }
}