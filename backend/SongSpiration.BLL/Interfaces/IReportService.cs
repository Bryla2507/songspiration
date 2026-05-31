using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Interfaces;

public interface IReportService
{
    Task<ReportDto> CreateReportAsync(ReportCreateDto dto, Guid reportingUserId);
    Task<IEnumerable<ReportDto>> GetAllReportsAsync();
    Task<ReportDto?> GetReportByIdAsync(Guid reportId);
    Task<bool> DeleteReportAsync(Guid reportId);
}