using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.Models.Entities;

namespace SongSpiration.DAL.Interfaces;

public interface IPinRepository
{
    Task<Pin?> GetByIdAsync(Guid id);
    Task<Pin?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Pin>> GetPinsAsync(int skip, int take, string? search, string? instrument, string? genre, string? sortBy, string? sortOrder);
    Task AddAsync(Pin pin);
    void Update(Pin pin);
    void Remove(Pin pin);
    void RemoveWithRelations(Pin pin);
    Task<bool> ToggleLikeAsync(Guid userId, Guid pinId);
    Task<int> GetCountByUserIdAsync(Guid userId);
    Task<int> GetTotalLikesReceivedByUserIdAsync(Guid userId);
    Task<IEnumerable<Pin>> GetPinsByUserIdAsync(Guid userId);  
    Task<IEnumerable<Guid>> GetValidGenreIdsAsync(IEnumerable<Guid> genreIds);
    Task IncrementDownloadCountAsync(Guid pinId);
    Task<int> SaveChangesAsync();
    Task<IEnumerable<SongSpiration.Models.Entities.Pin>> GetLikedPinsByUserIdAsync(Guid userId, string? sortBy, string? sortOrder);
    Task<IEnumerable<SongSpiration.Models.Entities.Pin>> GetPinsByUserIdAsync(Guid userId, string? sortBy = null, string? sortOrder = null, bool showPrivate = false);

}