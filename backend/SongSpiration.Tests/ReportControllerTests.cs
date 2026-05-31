using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SongSpiration.API.Controllers;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.Models.Entities;
using Xunit;

namespace SongSpiration.Tests
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly ReportController _reportController;

        public ReportControllerTests()
        {
            _mockReportService = new Mock<IReportService>();
            _reportController = new ReportController(_mockReportService.Object);
        }

        [Fact]
        public async Task CreateReport_ShouldReturnOk()
        {
            // Arrange
            var createReportDto = new CreateReportDto
            {
                ReportedUserId = Guid.NewGuid(),
                Content = "Test content"
            };

            var userId = Guid.NewGuid();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _reportController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockReportService.Setup(service => service.CreateReportAsync(It.IsAny<CreateReportDto>(), userId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _reportController.CreateReport(createReportDto);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockReportService.Verify(service => service.CreateReportAsync(It.IsAny<CreateReportDto>(), userId), Times.Once);
        }

        [Fact]
        public async Task GetAllReports_ShouldReturnOkWithReports()
        {
            // Arrange
            var reports = new List<ReportDto>
            {
                new ReportDto { Id = Guid.NewGuid(), Content = "Test content 1", ReportedUserId = Guid.NewGuid(), ReportingUserId = Guid.NewGuid() },
                new ReportDto { Id = Guid.NewGuid(), Content = "Test content 2", ReportedUserId = Guid.NewGuid(), ReportingUserId = Guid.NewGuid() }
            };

            _mockReportService.Setup(service => service.GetAllReportsAsync())
                .ReturnsAsync(reports)
                .Verifiable();

            // Act
            var result = await _reportController.GetAllReports();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ReportDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            _mockReportService.Verify(service => service.GetAllReportsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetReportById_ShouldReturnOkWithReport()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new ReportDto { Id = reportId, Content = "Test content", ReportedUserId = Guid.NewGuid(), ReportingUserId = Guid.NewGuid() };

            _mockReportService.Setup(service => service.GetReportByIdAsync(reportId))
                .ReturnsAsync(report)
                .Verifiable();

            // Act
            var result = await _reportController.GetReportById(reportId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ReportDto>(okResult.Value);
            Assert.Equal(reportId, returnValue.Id);
            _mockReportService.Verify(service => service.GetReportByIdAsync(reportId), Times.Once);
        }

        [Fact]
        public async Task DeleteReport_ShouldReturnOk()
        {
            // Arrange
            var reportId = Guid.NewGuid();

            _mockReportService.Setup(service => service.DeleteReportAsync(reportId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _reportController.DeleteReport(reportId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockReportService.Verify(service => service.DeleteReportAsync(reportId), Times.Once);
        }
    }
}