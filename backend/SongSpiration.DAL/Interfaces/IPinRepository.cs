using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.Models.Entities;

namespace SongSpiration.DAL.Interfaces;

public interface IPinRepository
{
    Task<Pin?> GetByIdAsync(Guid id);
    Task<Pin?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Pin>> GetPinsAsync(int skip = 0, int take = 50);
    Task AddAsync(Pin pin);
    void Update(Pin pin);
    void Remove(Pin pin);
    /// <summary>
    /// Toggles like for given user and pin. Returns true if liked after operation, false if unliked.
    /// </summary>
    Task<bool> ToggleLikeAsync(Guid userId, Guid pinId);
    Task<int> SaveChangesAsync();
}