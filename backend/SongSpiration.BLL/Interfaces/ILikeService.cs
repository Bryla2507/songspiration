using System;
using System.Threading.Tasks;

namespace SongSpiration.BLL.Interfaces;

public interface ILikeService
{
    Task<bool> LikePinAsync(Guid userId, Guid pinId);
    Task<bool> UnlikePinAsync(Guid userId, Guid pinId);
    Task<bool> IsPinLikedByUserAsync(Guid userId, Guid pinId);
    Task<int> GetLikeCountAsync(Guid pinId);
    Task<bool> ToggleLikeAsync(Guid userId, Guid pinId);
}