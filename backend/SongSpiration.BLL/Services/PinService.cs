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
            Instrument = Instrument.Guitar, // Default instrument
            Filename = "default.gp", // Default filename
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _pinRepository.AddAsync(pin);
        await _pinRepository.SaveChangesAsync();

        return MapToDto(pin);
    }

    public async Task<PinDto?> GetPinByIdAsync(Guid pinId)
    {
        var pin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        return pin != null ? MapToDto(pin) : null;
    }

    public async Task<PinDto> UpdatePinAsync(Guid pinId, UpdatePinDto updateDto)
    {
        var existingPin = await _pinRepository.GetByIdWithDetailsAsync(pinId);
        if (existingPin == null)
        {
            throw new KeyNotFoundException($"Pin with ID {pinId} not found.");
        }

        if (updateDto.Title != null)
        {
            existingPin.Title = updateDto.Title;
        }

        if (updateDto.Description != null)
        {
            existingPin.Description = updateDto.Description;
        }

        if (updateDto.Visibility != null)
        {
            existingPin.Visibility = updateDto.Visibility.Value;
        }

        existingPin.UpdatedAt = DateTime.UtcNow;
        _pinRepository.Update(existingPin);
        await _pinRepository.SaveChangesAsync();

        return MapToDto(existingPin);
    }

    public async Task<bool> DeletePinAsync(Guid pinId)
    {
        var pin = await _pinRepository.GetByIdAsync(pinId);
        if (pin == null)
        {
            return false;
        }

        _pinRepository.Remove(pin);
        await _pinRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PinDto>> GetAllPinsAsync()
    {
        var pins = await _pinRepository.GetPinsAsync();
        return pins.Select(MapToDto);
    }

    public async Task<IEnumerable<PinDto>> GetPinsByUserIdAsync(Guid userId)
    {
        var pins = await _pinRepository.GetPinsAsync();
        return pins.Where(p => p.OwnerId == userId).Select(MapToDto);
    }

    public async Task<IEnumerable<PinDto>> GetPinsByBoardIdAsync(Guid boardId)
    {
        // For now, return empty list as board functionality is not fully implemented
        // In a real implementation, this would filter pins by board ID
        return new List<PinDto>();
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
            Genres = pin.PinGenres?.Select(pg => pg.Genre?.Name ?? "").ToList() ?? new List<string>()
        };
    }
}
