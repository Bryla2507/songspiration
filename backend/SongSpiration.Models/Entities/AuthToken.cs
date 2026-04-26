using System;

namespace SongSpiration.Models.Entities
{
    public class AuthToken
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public TokenType TokenType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
