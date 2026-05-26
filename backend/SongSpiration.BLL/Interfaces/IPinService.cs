using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Interfaces;

public interface IPinService
{
    Task<PinDto> CreatePinAsync(Guid ownerId, CreatePinDto createDto);
    Task<PinDto?> GetPinByIdAsync(Guid pinId);
    Task<PinDto> UpdatePinAsync(Guid pinId, UpdatePinDto updateDto);
    Task<bool> DeletePinAsync(Guid pinId);
    Task<IEnumerable<PinDto>> GetAllPinsAsync(int start, int limit, string? search, string? instrument, string? genre, string? sortBy, string? sortOrder);
    Task<IEnumerable<PinDto>> GetPinsByUserIdAsync(Guid userId);
    Task<IEnumerable<PinDto>> GetPinsByBoardIdAsync(Guid boardId);
    Task<(bool IsLiked, int LikeCount)> ToggleLikeAsync(Guid userId, Guid pinId);
    Task IncrementDownloadCountAsync(Guid pinId);
}
