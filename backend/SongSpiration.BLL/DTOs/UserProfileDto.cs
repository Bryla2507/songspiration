namespace SongSpiration.BLL.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    
    public int AddedPinsCount { get; set; }
    public int TotalLikesReceived { get; set; }
    public string Roles { get; set; } = string.Empty;
    public bool IsEditorChoice { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}
