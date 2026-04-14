using System;

namespace SongSpiration.Models.Entities
{
    public class AuthToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; }
        public string TokenType { get; set; } // Access or Refresh
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}