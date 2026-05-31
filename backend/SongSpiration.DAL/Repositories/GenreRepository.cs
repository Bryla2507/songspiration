using SongSpiration.DAL.Interfaces;
using ModelsGenre = SongSpiration.Models.Genre;
using EntitiesGenre = SongSpiration.Models.Entities.Genre;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongSpiration.DAL.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly SongSpirationDbContext _context;

        public GenreRepository(SongSpirationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EntitiesGenre>> GetAllAsync()
        {
            return await _context.Set<EntitiesGenre>().ToListAsync();
        }

        public async Task<EntitiesGenre> GetByIdAsync(Guid id)
        {
            return await _context.Set<EntitiesGenre>().FindAsync(id);
        }

        public async Task AddAsync(EntitiesGenre genre)
        {
            await _context.Set<EntitiesGenre>().AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public void Update(EntitiesGenre genre)
        {
            _context.Set<EntitiesGenre>().Update(genre);
        }

        public void Delete(EntitiesGenre genre)
        {
            _context.Set<EntitiesGenre>().Remove(genre);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
