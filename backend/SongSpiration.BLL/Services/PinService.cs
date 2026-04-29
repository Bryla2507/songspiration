using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models;
using SongSpiration.Models.Entities;

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
    var pin = new Pin
    {
        Id = Guid.NewGuid(),
        OwnerId = ownerId,
        Title = createDto.Title,
        Description = createDto.Description,
        Visibility = createDto.Visibility,
        Instrument = createDto.Instrument,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        PinGenres = new List<PinGenre>()
    };

    if (!string.IsNullOrEmpty(createDto.TempFileLocation) && System.IO.File.Exists(createDto.TempFileLocation))
    {
        var fileInfo = new FileInfo(createDto.TempFileLocation);
        pin.FilePath = createDto.TempFileLocation;
        pin.Filename = fileInfo.Name;
        pin.MimeType = "application/octet-stream";
        pin.Size = fileInfo.Length;
        pin.Checksum = Guid.NewGuid().ToString("N");
    }

    try 
    {
        // KROK 1: Zapis samych danych podstawowych
        await _pinRepository.AddAsync(pin);
        await _pinRepository.SaveChangesAsync();
        Console.WriteLine($">>> SUKCES: Pin {pin.Id} zapisany dla OwnerId: {ownerId}");

        // KROK 2: Dodanie gatunków
        if (createDto.GenreIds != null && createDto.GenreIds.Any())
        {
            foreach (var gId in createDto.GenreIds)
            {
                pin.PinGenres.Add(new PinGenre 
                { 
                    PinId = pin.Id, 
                    GenreId = gId 
                });
            }
            
            await _pinRepository.SaveChangesAsync();
            Console.WriteLine($">>> SUKCES: Powiązano {createDto.GenreIds.Count} gatunków.");
        }

        var pinWithDetails = await _pinRepository.GetByIdWithDetailsAsync(pin.Id);
        return MapToDto(pinWithDetails ?? pin);
    }
    catch (Exception ex)
    {
        // Szczegółowy log w terminalu
        Console.WriteLine("!!!!!!!!!! BŁĄD ZAPISU DO BAZY !!!!!!!!!!");
        Console.WriteLine(ex.ToString()); 
        
        if (System.IO.File.Exists(createDto.TempFileLocation))
            System.IO.File.Delete(createDto.TempFileLocation);
            
        throw;
    }
}

    public async Task<PinDto?> GetPinByIdAsync(Guid pinId)
    {
        var pin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        return pin != null ? MapToDto(pin) : null;
    }

    public async Task<IEnumerable<PinDto>> GetAllPinsAsync()
    {
        var pins = await _pinRepository.GetPinsAsync();
        return pins.Select(MapToDto);
    }

    public async Task<PinDto> UpdatePinAsync(Guid pinId, UpdatePinDto updateDto)
    {
        var existingPin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        if (existingPin == null)
        {
            throw new KeyNotFoundException($"Pin o ID {pinId} nie istnieje.");
        }

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
    // 1. Sprawdzamy, czy rekord już jest w pamięci podręcznej (Local)
    // To zapobiega konfliktowi "already being tracked"
    var existingPin = await _pinRepository.GetByIdAsync(pinId);

    if (existingPin == null) return false;

    // 2. Usuwamy
    _pinRepository.Remove(existingPin);
    await _pinRepository.SaveChangesAsync();
    return true;
}

    public async Task<IEnumerable<PinDto>> GetPinsByUserIdAsync(Guid userId)
    {
        var pins = await _pinRepository.GetPinsAsync();
        return pins.Where(p => p.OwnerId == userId).Select(MapToDto);
    }

    public async Task<IEnumerable<PinDto>> GetPinsByBoardIdAsync(Guid boardId)
    {
        return await Task.FromResult(Enumerable.Empty<PinDto>());
    }

    private PinDto MapToDto(Pin pin)
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
            Size = pin.Size,
            CreatedAt = pin.CreatedAt,
            // Pobieramy nazwy gatunków z załadowanych relacji
            Genres = pin.PinGenres?
                .Where(pg => pg.Genre != null)
                .Select(pg => pg.Genre!.Name)
                .ToList() ?? new List<string>()
        };
    }
}