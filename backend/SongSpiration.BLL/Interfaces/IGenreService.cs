using SongSpiration.BLL.DTOs;
using SongSpiration.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongSpiration.BLL.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        Task<Genre> AddGenreAsync(GenreCreateDto genreDto);
        Task DeleteGenreAsync(Guid id);
    }
}