using System.Collections.Generic;

namespace SongSpiration.Models.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public ICollection<PinGenre> PinGenres { get; set; }
    }
}