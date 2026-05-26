using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;

namespace SongSpiration.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SongSpirationDbContext _db;

    public UserRepository(SongSpirationDbContext db) => _db = db;

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
    }

    public void Delete(User user)
    {
        _db.Users.Remove(user);
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

    public async Task<IEnumerable<User>> SearchUsersAsync(string criteria)
    {
        return await _db.Users
            .Where(u => u.DisplayName.Contains(criteria) || u.Email.Contains(criteria))
            .ToListAsync();
    }

    public async Task BanUserAsync(Guid userId)
    {
        var entityUser = await _db.Users.FindAsync(userId);
        if (entityUser != null)
        {
            entityUser.Roles = "Banned";
            _db.Users.Update(entityUser);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteUserPinsAsync(Guid userId)
    {
        var pins = _db.Pins.Where(p => p.OwnerId == userId);
        _db.Pins.RemoveRange(pins);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetEditorChoiceUsersAsync(int limit)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.IsEditorChoice)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByMostLikesAsync(int limit)
    {
        return await _db.Users
            .AsNoTracking()
            .Include(u => u.Pins)
            .ThenInclude(p => p.Likes)
            .OrderByDescending(u => u.Pins.SelectMany(p => p.Likes).Count())
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByMostDownloadsAsync(int limit)
    {
        return await _db.Users
            .AsNoTracking()
            .Include(u => u.Pins)
            .OrderByDescending(u => u.Pins.Sum(p => p.DownloadsCount))
            .Take(limit)
            .ToListAsync();
    }
}
