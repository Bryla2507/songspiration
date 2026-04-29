using Moq;
using AutoMapper;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using SongSpiration.BLL.Services;
using SongSpiration.BLL.DTOs;
using Xunit;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongSpiration.Tests.BLL
{
    public class FilterServiceTests
    {
        private readonly Mock<IPinRepository> _mockPinRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FilterService _filterService;

        public FilterServiceTests()
        {
            _mockPinRepository = new Mock<IPinRepository>();
            _mockMapper = new Mock<IMapper>();
            _filterService = new FilterService(_mockPinRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task FilterPins_ShouldReturnFilteredPins()
        {
            // Arrange
            var filterDto = new PinFilterDto
            {
                Genre = "Rock",
                Instrument = Instrument.Guitar,
                Visibility = PinVisibility.Public
            };

            var pins = new List<Pin>
            {
                new Pin
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Pin 1",
                    Instrument = Instrument.Guitar,
                    Visibility = PinVisibility.Public,
                    Genres = new List<Genre> { new Genre { Name = "Rock" } }
                },
                new Pin
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Pin 2",
                    Instrument = Instrument.Bass,
                    Visibility = PinVisibility.Public,
                    Genres = new List<Genre> { new Genre { Name = "Jazz" } }
                }
            };

            var pinDtos = new List<PinDto>
            {
                new PinDto
                {
                    Id = pins[0].Id,
                    Title = "Test Pin 1",
                    Instrument = Instrument.Guitar,
                    Visibility = PinVisibility.Public
                }
            };

            _mockPinRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Pin, bool>>>()))
                .ReturnsAsync(pins.Where(p => p.Instrument == Instrument.Guitar && p.Genres.Any(g => g.Name == "Rock") && p.Visibility == PinVisibility.Public).ToList());

            _mockMapper.Setup(m => m.Map<IEnumerable<PinDto>>(It.IsAny<IEnumerable<Pin>>()))
                .Returns(pinDtos);

            // Act
            var result = await _filterService.FilterPins(filterDto);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Pin 1", result.First().Title);
        }

        [Fact]
        public async Task FilterPins_ShouldReturnAllPins_WhenNoFilterApplied()
        {
            // Arrange
            var filterDto = new PinFilterDto();

            var pins = new List<Pin>
            {
                new Pin
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Pin 1",
                    Instrument = Instrument.Guitar,
                    Visibility = PinVisibility.Public,
                    Genres = new List<Genre> { new Genre { Name = "Rock" } }
                },
                new Pin
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Pin 2",
                    Instrument = Instrument.Bass,
                    Visibility = PinVisibility.Public,
                    Genres = new List<Genre> { new Genre { Name = "Jazz" } }
                }
            };

            var pinDtos = new List<PinDto>
            {
                new PinDto
                {
                    Id = pins[0].Id,
                    Title = "Test Pin 1",
                    Instrument = Instrument.Guitar,
                    Visibility = PinVisibility.Public
                },
                new PinDto
                {
                    Id = pins[1].Id,
                    Title = "Test Pin 2",
                    Instrument = Instrument.Bass,
                    Visibility = PinVisibility.Public
                }
            };

            _mockPinRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Pin, bool>>>()))
                .ReturnsAsync(pins);

            _mockMapper.Setup(m => m.Map<IEnumerable<PinDto>>(It.IsAny<IEnumerable<Pin>>()))
                .Returns(pinDtos);

            // Act
            var result = await _filterService.FilterPins(filterDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}