using System;
using System.Collections.Generic;

namespace SongSpiration.Models.Entities
{
    public class Pin
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public Instrument Instrument { get; set; }
        public PinVisibility Visibility { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<PinGenre> PinGenres { get; set; } = new List<PinGenre>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
