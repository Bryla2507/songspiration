using System;
using SongSpiration.Models;

namespace SongSpiration.BLL.DTOs;

public class PinDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Instrument Instrument { get; set; }
    public PinVisibility Visibility { get; set; }
    public string Filename { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public List<string> Genres { get; set; } = new();
}

public class CreatePinDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Instrument Instrument { get; set; }
    public PinVisibility Visibility { get; set; }
    public List<string> GenreIds { get; set; } = new();

    // File will be handled separately in the controller
    public string? TempFileLocation { get; set; }
}

public class UpdatePinDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public PinVisibility? Visibility { get; set; }
    public List<Guid>? GenreIds { get; set; }
}

public class PinFilterDto
{
    public Instrument? Instrument { get; set; }
    public List<Guid>? GenreIds { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } // "newest", "popular"
}
