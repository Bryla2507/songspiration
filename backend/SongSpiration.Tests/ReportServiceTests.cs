using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Services;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using Xunit;

namespace SongSpiration.Tests
{
    public class ReportServiceTests
    {
        private readonly Mock<IReportRepository> _mockReportRepository;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _mockReportRepository = new Mock<IReportRepository>();
            _reportService = new ReportService(_mockReportRepository.Object);
        }

        [Fact]
        public async Task CreateReportAsync_ShouldAddReport()
        {
            // Arrange
            var createReportDto = new CreateReportDto
            {
                ReportedUserId = Guid.NewGuid(),
                Content = "Test content"
            };

            var userId = Guid.NewGuid();

            _mockReportRepository.Setup(repo => repo.AddAsync(It.IsAny<Report>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockReportRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();

            // Act
            await _reportService.CreateReportAsync(createReportDto, userId);

            // Assert
            _mockReportRepository.Verify(repo => repo.AddAsync(It.IsAny<Report>()), Times.Once);
            _mockReportRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllReportsAsync_ShouldReturnAllReports()
        {
            // Arrange
            var reports = new List<Report>
            {
                new Report { Id = Guid.NewGuid(), Content = "Test content 1", ReportedUserId = Guid.NewGuid(), ReportingUserId = Guid.NewGuid() },
                new Report { Id = Guid.NewGuid(), Content = "Test content 2", ReportedUserId = Guid.NewGuid(), ReportingUserId = Guid.NewGuid() }
            };

            _mockReportRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(reports)
                .Verifiable();

            // Act
            var result = await _reportService.GetAllReportsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockReportRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetReportByIdAsync_ShouldReturnReport()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report { Id = reportId, Content = "Test content", ReportedUserId = Guid.NewGuid(), ReportingUserId = Guid.NewGuid() };

            _mockReportRepository.Setup(repo => repo.GetByIdAsync(reportId))
                .ReturnsAsync(report)
                .Verifiable();

            // Act
            var result = await _reportService.GetReportByIdAsync(reportId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reportId, result.Id);
            _mockReportRepository.Verify(repo => repo.GetByIdAsync(reportId), Times.Once);
        }

        [Fact]
        public async Task DeleteReportAsync_ShouldDeleteReport()
        {
            // Arrange
            var reportId = Guid.NewGuid();

            _mockReportRepository.Setup(repo => repo.DeleteAsync(reportId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockReportRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();

            // Act
            await _reportService.DeleteReportAsync(reportId);

            // Assert
            _mockReportRepository.Verify(repo => repo.DeleteAsync(reportId), Times.Once);
            _mockReportRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}