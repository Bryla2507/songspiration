using Moq;
using AutoMapper;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using SongSpiration.BLL.Services;
using SongSpiration.BLL.DTOs;
using Xunit;

namespace SongSpiration.Tests.BLL
{
    public class CollectionServiceTests
    {
        private readonly Mock<ICollectionRepository> _mockCollectionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CollectionService _collectionService;

        public CollectionServiceTests()
        {
            _mockCollectionRepository = new Mock<ICollectionRepository>();
            _mockMapper = new Mock<IMapper>();
            _collectionService = new CollectionService(_mockCollectionRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateCollection_ShouldAddCollectionToRepository()
        {
            // Arrange
            var collectionDto = new CollectionCreateDto
            {
                Title = "Test Collection",
                Description = "Test Description",
                Visibility = PinVisibility.Public
            };

            var collection = new Collection
            {
                Id = Guid.NewGuid(),
                Title = "Test Collection",
                Description = "Test Description",
                Visibility = PinVisibility.Public,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var collectionResultDto = new CollectionDto
            {
                Id = collection.Id,
                Title = "Test Collection",
                Description = "Test Description",
                Visibility = PinVisibility.Public,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockMapper.Setup(m => m.Map<Collection>(collectionDto)).Returns(collection);
            _mockCollectionRepository.Setup(repo => repo.Add(collection)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<CollectionDto>(collection)).Returns(collectionResultDto);

            // Act
            var result = await _collectionService.CreateCollection(collectionDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(collectionDto.Title, result.Title);
            Assert.Equal(collectionDto.Description, result.Description);
            Assert.Equal(collectionDto.Visibility, result.Visibility);
            _mockCollectionRepository.Verify(repo => repo.Add(collection), Times.Once);
        }

        [Fact]
        public async Task GetCollectionById_ShouldReturnCollectionDto()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var collection = new Collection
            {
                Id = collectionId,
                Title = "Test Collection",
                Description = "Test Description",
                Visibility = PinVisibility.Public,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var collectionDto = new CollectionDto
            {
                Id = collectionId,
                Title = "Test Collection",
                Description = "Test Description",
                Visibility = PinVisibility.Public,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCollectionRepository.Setup(repo => repo.GetById(collectionId)).ReturnsAsync(collection);
            _mockMapper.Setup(m => m.Map<CollectionDto>(collection)).Returns(collectionDto);

            // Act
            var result = await _collectionService.GetCollectionById(collectionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(collectionId, result.Id);
            Assert.Equal(collection.Title, result.Title);
            _mockCollectionRepository.Verify(repo => repo.GetById(collectionId), Times.Once);
        }

        [Fact]
        public async Task AddPinToCollection_ShouldCallRepository()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var pinId = Guid.NewGuid();

            _mockCollectionRepository.Setup(repo => repo.AddPinToCollection(collectionId, pinId)).Returns(Task.CompletedTask);

            // Act
            await _collectionService.AddPinToCollection(collectionId, pinId);

            // Assert
            _mockCollectionRepository.Verify(repo => repo.AddPinToCollection(collectionId, pinId), Times.Once);
        }

        [Fact]
        public async Task RemovePinFromCollection_ShouldCallRepository()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var pinId = Guid.NewGuid();

            _mockCollectionRepository.Setup(repo => repo.RemovePinFromCollection(collectionId, pinId)).Returns(Task.CompletedTask);

            // Act
            await _collectionService.RemovePinFromCollection(collectionId, pinId);

            // Assert
            _mockCollectionRepository.Verify(repo => repo.RemovePinFromCollection(collectionId, pinId), Times.Once);
        }
    }
}