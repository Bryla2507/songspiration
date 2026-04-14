using System;
using System.Collections.Generic;

namespace SongSpiration.Models.Entities
{
    public class Pin
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public string Checksum { get; set; }
        public string Visibility { get; set; } // Public or Private
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<PinGenre> PinGenres { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}