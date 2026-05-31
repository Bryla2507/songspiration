using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SongSpiration.BLL.Interfaces;
using SongSpiration.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SongSpiration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReportDto>> CreateReport([FromBody] ReportCreateDto dto)
        {
            var reportingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var report = await _reportService.CreateReportAsync(dto, reportingUserId);
            return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, report);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ReportDto>> GetReportById(Guid id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetAllReports()
        {
            var reports = await _reportService.GetAllReportsAsync();
            return Ok(reports);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var result = await _reportService.DeleteReportAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}