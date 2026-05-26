using System;

namespace SongSpiration.BLL.DTOs;

public class UserRankingDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsEditorChoice { get; set; }
    public int TotalLikes { get; set; }
    public int TotalDownloads { get; set; }
}
