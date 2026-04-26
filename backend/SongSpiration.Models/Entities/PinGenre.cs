using System;

namespace SongSpiration.Models.Entities
{
    public class PinGenre
    {
        public Guid PinId { get; set; }
        public Pin Pin { get; set; } = null!;
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}
