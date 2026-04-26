using System;
using System.Collections.Generic;

namespace SongSpiration.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string Roles { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsEmailVerified { get; set; }

        // Navigation properties
        public ICollection<Pin> Pins { get; set; } = new List<Pin>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
