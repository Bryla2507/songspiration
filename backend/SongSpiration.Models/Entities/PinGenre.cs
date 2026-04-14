namespace SongSpiration.Models.Entities
{
    public class PinGenre
    {
        public int PinId { get; set; }
        public Pin Pin { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}