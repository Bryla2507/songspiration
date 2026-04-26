using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Interfaces;

public interface ICollectionService
{
    Task<CollectionDto> CreateCollectionAsync(Guid userId, CreateCollectionDto createDto);
    Task<CollectionDto?> GetCollectionByIdAsync(Guid collectionId);
    Task<IEnumerable<CollectionDto>> GetCollectionsByUserIdAsync(Guid userId);
    Task<CollectionDto> UpdateCollectionAsync(Guid collectionId, UpdateCollectionDto updateDto);
    Task<bool> DeleteCollectionAsync(Guid collectionId);
    Task<bool> AddPinToCollectionAsync(Guid collectionId, Guid pinId);
    Task<bool> RemovePinFromCollectionAsync(Guid collectionId, Guid pinId);
    Task<bool> IsPinInCollectionAsync(Guid collectionId, Guid pinId);
    Task<IEnumerable<PinDto>> GetPinsInCollectionAsync(Guid collectionId);
}
