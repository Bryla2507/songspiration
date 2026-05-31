using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;

namespace SongSpiration.BLL.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    public ReportService(IReportRepository reportRepository, IUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
    }

    public async Task<ReportDto> CreateReportAsync(ReportCreateDto dto, Guid reportingUserId)
    {
        var report = new Report
        {
            ReportedUserId = dto.ReportedUserId,
            ReportingUserId = reportingUserId,
            Content = dto.Content
        };

        await _reportRepository.AddAsync(report);
        return await MapToReportDto(report);
    }

    public async Task<IEnumerable<ReportDto>> GetAllReportsAsync()
    {
        var reports = await _reportRepository.GetAllAsync();
        return await Task.WhenAll(reports.Select(MapToReportDto));
    }

    public async Task<ReportDto?> GetReportByIdAsync(Guid reportId)
    {
        var report = await _reportRepository.GetByIdAsync(reportId);
        return report != null ? await MapToReportDto(report) : null;
    }

    public async Task<bool> DeleteReportAsync(Guid reportId)
    {
        var report = await _reportRepository.GetByIdAsync(reportId);
        if (report == null) return false;

        _reportRepository.Remove(report);
        await _reportRepository.SaveChangesAsync();
        return true;
    }

    private async Task<ReportDto> MapToReportDto(Report report)
    {
        var reportedUser = await _userRepository.GetByIdAsync(report.ReportedUserId);
        var reportingUser = await _userRepository.GetByIdAsync(report.ReportingUserId);

        return new ReportDto
        {
            Id = report.Id,
            ReportedUserId = report.ReportedUserId,
            ReportingUserId = report.ReportingUserId,
            Content = report.Content,
            ReportedUser = new UserDto
            {
                Id = reportedUser.Id,
                Email = reportedUser.Email,
                DisplayName = reportedUser.DisplayName,
                AvatarUrl = reportedUser.AvatarUrl,
                Bio = reportedUser.Bio,
                Roles = reportedUser.Roles
            },
            ReportingUser = new UserDto
            {
                Id = reportingUser.Id,
                Email = reportingUser.Email,
                DisplayName = reportingUser.DisplayName,
                AvatarUrl = reportingUser.AvatarUrl,
                Bio = reportingUser.Bio,
                Roles = reportingUser.Roles
            }
        };
    }
}