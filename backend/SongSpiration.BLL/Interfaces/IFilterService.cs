using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.Models;

namespace SongSpiration.BLL.Interfaces;

public interface IFilterService
{
    Task<IEnumerable<PinDto>> FilterPinsAsync(PinFilterDto filterDto);
    Task<IEnumerable<PinDto>> SearchPinsAsync(string searchTerm);
    Task<IEnumerable<PinDto>> GetPopularPinsAsync(int limit = 10);
    Task<IEnumerable<PinDto>> GetRecentPinsAsync(int limit = 10);
    Task<IEnumerable<PinDto>> GetPinsByInstrumentAsync(Instrument instrument);
    Task<IEnumerable<PinDto>> GetPinsByGenreAsync(Guid genreId);
}