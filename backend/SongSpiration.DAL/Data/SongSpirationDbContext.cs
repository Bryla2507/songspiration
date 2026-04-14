using Microsoft.EntityFrameworkCore;
using SongSpiration.Models;

namespace SongSpiration.DAL;

public class SongSpirationDbContext : DbContext
{
    public SongSpirationDbContext(DbContextOptions<SongSpirationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Pin> Pins { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<PinGenre> PinGenres { get; set; } = null!;
    public DbSet<Like> Likes { get; set; } = null!;
    public DbSet<AuthToken> AuthTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite keys
        modelBuilder.Entity<PinGenre>().HasKey(pg => new { pg.PinId, pg.GenreId });
        modelBuilder.Entity<Like>().HasKey(l => new { l.UserId, l.PinId });

        // Unique constraints / indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Relations
        modelBuilder.Entity<Pin>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.Pins)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PinGenre>()
            .HasOne(pg => pg.Pin)
            .WithMany(p => p.PinGenres)
            .HasForeignKey(pg => pg.PinId);

        modelBuilder.Entity<PinGenre>()
            .HasOne(pg => pg.Genre)
            .WithMany()
            .HasForeignKey(pg => pg.GenreId);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Pin)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PinId);
    }
}