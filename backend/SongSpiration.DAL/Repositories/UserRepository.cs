using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models;

namespace SongSpiration.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SongSpirationDbContext _db;

    public UserRepository(SongSpirationDbContext db) => _db = db;

    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

    public async Task<User?> GetByEmailAsync(string email)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id)
        => await _db.Users.FindAsync(id);

    public void Update(User user) => _db.Users.Update(user);

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}