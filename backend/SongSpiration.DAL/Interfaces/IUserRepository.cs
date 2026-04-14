using System;
using System.Threading.Tasks;
using SongSpiration.Models;

namespace SongSpiration.DAL.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    void Update(User user);
    Task<int> SaveChangesAsync();
}