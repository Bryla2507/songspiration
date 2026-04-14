using System;

namespace SongSpiration.Models.Entities
{
    public class Like
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int PinId { get; set; }
        public Pin Pin { get; set; }
        public DateTime LikedAt { get; set; }
    }
}