using System;

namespace SongSpiration.Models.Entities
{
    public class Like
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid PinId { get; set; }
        public Pin Pin { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
