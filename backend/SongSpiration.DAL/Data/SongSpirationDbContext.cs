using Microsoft.EntityFrameworkCore;
using SongSpiration.Models.Entities;

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
        .WithMany(g => g.PinGenres) 
        .HasForeignKey(pg => pg.GenreId);

    modelBuilder.Entity<Like>()
        .HasOne(l => l.User)
        .WithMany(u => u.Likes)
        .HasForeignKey(l => l.UserId);

    modelBuilder.Entity<Like>()
        .HasOne(l => l.Pin)
        .WithMany(p => p.Likes)
        .HasForeignKey(l => l.PinId);

    // FIX DLA SQLITE: Konwersja Guid na string (małe litery)
    // To zapewnia, że baza zawsze znajdzie ID, niezależnie od formatu
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        foreach (var property in entityType.GetProperties())
        {
            if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
            {
                property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<Guid, string>(
                    v => v.ToString().ToLower(), 
                    v => Guid.Parse(v)));
            }
        }
    }
}
} 