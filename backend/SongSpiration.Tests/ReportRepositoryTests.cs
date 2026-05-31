using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using SongSpiration.DAL;
using SongSpiration.DAL.Repositories;
using SongSpiration.Models.Entities;
using Xunit;

namespace SongSpiration.Tests
{
    public class ReportRepositoryTests
    {
        private readonly DbContextOptions<SongSpirationDbContext> _dbContextOptions;
        private readonly SongSpirationDbContext _dbContext;
        private readonly ReportRepository _reportRepository;

        public ReportRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SongSpirationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new SongSpirationDbContext(_dbContextOptions);
            _reportRepository = new ReportRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldAddReport()
        {
            // Arrange
            var report = new Report
            {
                Id = Guid.NewGuid(),
                Content = "Test content",
                ReportedUserId = Guid.NewGuid(),
                ReportingUserId = Guid.NewGuid()
            };

            // Act
            await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();
            var result = await _dbContext.Reports.FindAsync(report.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(report.Content, result.Content);
            Assert.Equal(report.ReportedUserId, result.ReportedUserId);
            Assert.Equal(report.ReportingUserId, result.ReportingUserId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllReports()
        {
            // Arrange
            var report1 = new Report
            {
                Id = Guid.NewGuid(),
                Content = "Test content 1",
                ReportedUserId = Guid.NewGuid(),
                ReportingUserId = Guid.NewGuid()
            };

            var report2 = new Report
            {
                Id = Guid.NewGuid(),
                Content = "Test content 2",
                ReportedUserId = Guid.NewGuid(),
                ReportingUserId = Guid.NewGuid()
            };

            await _dbContext.Reports.AddRangeAsync(report1, report2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _reportRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnReport()
        {
            // Arrange
            var report = new Report
            {
                Id = Guid.NewGuid(),
                Content = "Test content",
                ReportedUserId = Guid.NewGuid(),
                ReportingUserId = Guid.NewGuid()
            };

            await _dbContext.Reports.AddAsync(report);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _reportRepository.GetByIdAsync(report.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(report.Id, result.Id);
            Assert.Equal(report.Content, result.Content);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteReport()
        {
            // Arrange
            var report = new Report
            {
                Id = Guid.NewGuid(),
                Content = "Test content",
                ReportedUserId = Guid.NewGuid(),
                ReportingUserId = Guid.NewGuid()
            };

            await _dbContext.Reports.AddAsync(report);
            await _dbContext.SaveChangesAsync();

            // Act
            await _reportRepository.DeleteAsync(report.Id);
            await _reportRepository.SaveChangesAsync();
            var result = await _dbContext.Reports.FindAsync(report.Id);

            // Assert
            Assert.Null(result);
        }
    }
}