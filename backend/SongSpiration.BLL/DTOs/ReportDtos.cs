using System;
using SongSpiration.Models.Entities;

namespace SongSpiration.BLL.DTOs;

public class ReportCreateDto
{
    public Guid ReportedUserId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class ReportDto
{
    public int Id { get; set; }
    public Guid ReportedUserId { get; set; }
    public Guid ReportingUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public UserDto ReportedUser { get; set; } = null!;
    public UserDto ReportingUser { get; set; } = null!;
}