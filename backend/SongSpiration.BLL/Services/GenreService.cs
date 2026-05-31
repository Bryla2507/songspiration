using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongSpiration.BLL.Services
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Genre> _genreRepository;

        public GenreService(IRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<Genre> AddGenreAsync(GenreCreateDto genreDto)
        {
            var genre = new Genre
            {
                Id = System.Guid.NewGuid(),
                Name = genreDto.Name,
                Slug = genreDto.Slug
            };

            await _genreRepository.AddAsync(genre);
            await _genreRepository.SaveChangesAsync();
            return genre;
        }

        public async Task DeleteGenreAsync(Guid id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre != null)
            {
                _genreRepository.Remove(genre);
                await _genreRepository.SaveChangesAsync();
            }
        }
    }
}
