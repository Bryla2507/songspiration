using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using PinVisibility = SongSpiration.Models.PinVisibility;

namespace SongSpiration.DAL.Repositories;

public class PinRepository : IPinRepository
{
    private readonly SongSpirationDbContext _db;

    public PinRepository(SongSpirationDbContext db) => _db = db;

    public async Task AddAsync(Pin pin) => await _db.Pins.AddAsync(pin);

    public void Remove(Pin pin) => _db.Pins.Remove(pin);

    public void Update(Pin pin) => _db.Pins.Update(pin);

    async Task<IEnumerable<Guid>> IPinRepository.GetValidGenreIdsAsync(IEnumerable<Guid> genreIds)
    {
        return await _db.Genres
            .Where(g => genreIds.Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync();
    }

    public async Task<Pin?> GetByIdAsync(Guid id)
    {
        return await _db.Pins
            .AsNoTracking() 
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pin?> GetByIdWithDetailsAsync(Guid id)
        => await _db.Pins
            .Include(p => p.Owner)
            .Include(p => p.PinGenres)
                .ThenInclude(pg => pg.Genre)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Pin>> GetPinsAsync(int skip, int take, string? search, string? instrument, string? genre, string? sortBy, string? sortOrder)
    {
        var query = _db.Pins
            .Include(p => p.Owner)
            .Include(p => p.PinGenres)
                .ThenInclude(pg => pg.Genre)
            .Include(p => p.Likes)
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Title.Contains(search) || (p.Description != null && p.Description.Contains(search)));

        if (!string.IsNullOrEmpty(instrument) && instrument != "all")
            if (int.TryParse(instrument, out int instVal))
                query = query.Where(p => (int)p.Instrument == instVal);

        if (!string.IsNullOrEmpty(genre) && genre != "all")
            query = query.Where(p => p.PinGenres.Any(pg => pg.Genre.Name == genre));

        // Sorting
        bool isDesc = sortOrder?.ToLower() != "asc";
        query = sortBy?.ToLower() switch
        {
            "alpha" => isDesc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "likes" => isDesc ? query.OrderByDescending(p => p.Likes.Count) : query.OrderBy(p => p.Likes.Count),
            _ => isDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        return await query.AsNoTracking().Skip(skip).Take(take).ToListAsync();
    }

    public async Task<bool> ToggleLikeAsync(Guid userId, Guid pinId)
    {
        var existing = await _db.Likes.FindAsync(userId, pinId);
        if (existing is null)
        {
            var like = new Like { UserId = userId, PinId = pinId, CreatedAt = DateTime.UtcNow };
            await _db.Likes.AddAsync(like);
            await _db.SaveChangesAsync();
            return true;
        }
        else
        {
            _db.Likes.Remove(existing);
            await _db.SaveChangesAsync();
            return false;
        }
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId) 
        => await _db.Pins.CountAsync(p => p.OwnerId == userId);

    public async Task<int> GetTotalLikesReceivedByUserIdAsync(Guid userId)
        => await _db.Likes.CountAsync(l => l.Pin.OwnerId == userId);

    public async Task<IEnumerable<Pin>> GetPinsByUserIdAsync(Guid userId) 
    {
        return await _db.Pins
            .Where(p => p.OwnerId == userId) 
            .Include(p => p.PinGenres)
                .ThenInclude(pg => pg.Genre)
            .ToListAsync();
    }

    public async Task IncrementDownloadCountAsync(Guid pinId)
    {
        await _db.Pins
            .Where(p => p.Id == pinId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.DownloadsCount, p => p.DownloadsCount + 1));
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

    public async Task<IEnumerable<Pin>> GetLikedPinsByUserIdAsync(Guid userId, string? sortBy, string? sortOrder)
    {
    // Budujemy zapytanie na encjach bazy danych
    var query = _db.Pins
        .Where(p => p.Likes.Any(l => l.UserId == userId));

    // Filtrowanie po dacie dodania pinu (wymóg dla polubionych)
    if (sortOrder != null && sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
    {
        query = query.OrderBy(p => p.CreatedAt);
    }
    else
    {
        query = query.OrderByDescending(p => p.CreatedAt);
    }

    // Wyciągamy dane z bazy razem z relacjami gatunków oraz lajków (żeby MapToDto policzył LikeCount)
    return await query
        .Include(p => p.PinGenres)
            .ThenInclude(pg => pg.Genre)
        .Include(p => p.Likes) 
        .ToListAsync();
    }
    public async Task<IEnumerable<Pin>> GetPinsByUserIdAsync(Guid userId, string? sortBy = null, string? sortOrder = null, bool showPrivate = false)
    {
        var query = _db.Pins.Where(p => p.OwnerId == userId);

        if (!showPrivate)
        {
            query = query.Where(p => p.Visibility != PinVisibility.Private);
        }

        bool isAsc = sortOrder != null && sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

        if (sortBy != null && sortBy.Equals("likes", StringComparison.OrdinalIgnoreCase))
        {
            query = isAsc ? query.OrderBy(p => p.Likes.Count) : query.OrderByDescending(p => p.Likes.Count);
        }
        else
        {
            query = isAsc ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
        }

        return await query
            .Include(p => p.PinGenres)
                .ThenInclude(pg => pg.Genre)
            .Include(p => p.Likes)
            .ToListAsync();
    }
}