using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.Models.Entities;

namespace SongSpiration.DAL.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task<int> SaveChangesAsync();
    Task<IEnumerable<User>> SearchUsersAsync(string criteria);
    Task BanUserAsync(Guid userId);
    Task DeleteUserPinsAsync(Guid userId);
    Task<IEnumerable<User>> GetEditorChoiceUsersAsync(int limit);
    Task<IEnumerable<User>> GetUsersByMostLikesAsync(int limit);
    Task<IEnumerable<User>> GetUsersByMostDownloadsAsync(int limit);
    Task<IEnumerable<User>> GetAllAsync();
}
