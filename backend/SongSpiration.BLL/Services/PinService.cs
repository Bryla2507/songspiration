using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using EntitiesPin = SongSpiration.Models.Entities.Pin;
using EntitiesPinGenre = SongSpiration.Models.Entities.PinGenre;

namespace SongSpiration.BLL.Services;

public class PinService : IPinService
{
    private readonly IPinRepository _pinRepository;

    public PinService(IPinRepository pinRepository)
    {
        _pinRepository = pinRepository;
    }

    public async Task<PinDto> CreatePinAsync(Guid ownerId, CreatePinDto createDto)
    {
        var pin = new EntitiesPin
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = createDto.Title,
            Description = createDto.Description,
            Visibility = createDto.Visibility,
            Instrument = createDto.Instrument,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PinGenres = new List<EntitiesPinGenre>(),
            // Zapisujemy ścieżkę relatywną bezpośrednio z DTO
            FilePath = createDto.TempFileLocation ?? string.Empty,
            Filename = !string.IsNullOrEmpty(createDto.TempFileLocation)
                ? Path.GetFileName(createDto.TempFileLocation) ?? string.Empty
                : string.Empty,
            MimeType = "application/octet-stream"
        };

        try
        {
            await _pinRepository.AddAsync(pin);
            await _pinRepository.SaveChangesAsync();

if (createDto.GenreIds != null && createDto.GenreIds.Any())
{
    var validGenreIds = await _pinRepository.GetValidGenreIdsAsync(createDto.GenreIds);
    foreach (var gId in validGenreIds)
    {
        pin.PinGenres.Add(new EntitiesPinGenre
        {
            PinId = pin.Id,
            GenreId = gId
        });
    }
    await _pinRepository.SaveChangesAsync();
}

            var pinWithDetails = await _pinRepository.GetByIdWithDetailsAsync(pin.Id);
            return MapToDto(pinWithDetails ?? pin);
        }
        catch (Exception)
        {
            // W razie błędu bazy danych, kontroler zajmie się ewentualnym czyszczeniem plików,
            // jeśli przekażemy błąd dalej.
            throw;
        }
    }

    public async Task<PinDto?> GetPinByIdAsync(Guid pinId)
    {
        var pin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        return pin != null ? MapToDto(pin) : null;
    }

    public async Task<IEnumerable<PinDto>> GetLikedPinsByUserIdAsync(Guid userId, string? sortBy, string? sortOrder)
    {
        // Wywołujemy repozytorium z warstwy DAL
        var pins = await _pinRepository.GetLikedPinsByUserIdAsync(userId, sortBy, sortOrder);
        
        // Mapujemy encje bazy danych na Twoje DTO z BLL
        return pins.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<PinDto>> GetPinsByUserIdAsync(Guid userId)
    {
        var pins = await _pinRepository.GetPinsByUserIdAsync(userId);
        return pins.Select(pin => MapToDto(pin)).ToList();
    }

    public async Task<IEnumerable<PinDto>> GetPinsByUserIdAsync(Guid userId, string? sortBy, string? sortOrder, Guid? currentUserId = null)
    {
        bool showPrivate = currentUserId.HasValue && currentUserId.Value == userId;
        var pins = await _pinRepository.GetPinsByUserIdAsync(userId, sortBy, sortOrder, showPrivate);
        return pins.Select(pin => MapToDto(pin)).ToList();
    }

    public async Task<IEnumerable<PinDto>> GetAllPinsAsync(int start, int limit, string? search, string? instrument, string? genre, string? sortBy, string? sortOrder)
    {
        var pins = await _pinRepository.GetPinsAsync(start, limit, search, instrument, genre, sortBy, sortOrder); 
        return pins.Select(MapToDto);
    }

    public async Task<PinDto> UpdatePinAsync(Guid pinId, UpdatePinDto updateDto)
    {
        var existingPin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        if (existingPin == null) throw new KeyNotFoundException($"Pin o ID {pinId} nie istnieje.");

        if (updateDto.Title != null) existingPin.Title = updateDto.Title;
        if (updateDto.Description != null) existingPin.Description = updateDto.Description;
        if (updateDto.Visibility != null) existingPin.Visibility = updateDto.Visibility.Value;

        existingPin.UpdatedAt = DateTime.UtcNow;
        _pinRepository.Update(existingPin);
        await _pinRepository.SaveChangesAsync();

        return MapToDto(existingPin);
    }

    public async Task<bool> DeletePinAsync(Guid pinId)
    {
        // Używamy wersji WithDetails, żeby załadować Leki i Gatunki
        var existingPin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        if (existingPin == null) return false;
    
        // Używamy nowej metody, która usunie wszystko po kolei (lajki -> gatunki -> pin)
        _pinRepository.RemoveWithRelations(existingPin);
        
        await _pinRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PinDto>> GetPinsByBoardIdAsync(Guid boardId)
    {
        return await Task.FromResult(Enumerable.Empty<PinDto>());
    }
public async Task<(bool IsLiked, int LikeCount)> ToggleLikeAsync(Guid userId, Guid pinId)
{
    var isLiked = await _pinRepository.ToggleLikeAsync(userId, pinId);
    var updatedPin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
    return (isLiked, updatedPin?.Likes.Count ?? 0);
}

    public async Task IncrementDownloadCountAsync(Guid pinId)
    {
        await _pinRepository.IncrementDownloadCountAsync(pinId);
    }

    private PinDto MapToDto(EntitiesPin pin)
    {
        return new PinDto
        {
            Id = pin.Id,
            OwnerId = pin.OwnerId,
            Title = pin.Title,
            Description = pin.Description,
            Instrument = pin.Instrument,
            Visibility = pin.Visibility,
            Filename = pin.Filename,
            FilePath = pin.FilePath,
            Size = pin.Size,
            CreatedAt = pin.CreatedAt,
            LikeCount = pin.Likes?.Count ?? 0,
            DownloadsCount = pin.DownloadsCount,
            Genres = pin.PinGenres?
                .Where(pg => pg.Genre != null)
                .Select(pg => pg.Genre!.Name)
                .ToList() ?? new List<string>()
        };
    }
}
