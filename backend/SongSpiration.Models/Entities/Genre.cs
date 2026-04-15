using System;
using System.Collections.Generic;

namespace SongSpiration.Models.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public ICollection<PinGenre> PinGenres { get; set; } = new List<PinGenre>();
    }
}
