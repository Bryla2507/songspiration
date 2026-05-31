using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using SongSpiration.DAL;
using EntitiesUser = SongSpiration.Models.Entities.User;
using ModelsGenre = SongSpiration.Models.Genre;
using EntitiesGenre = SongSpiration.Models.Entities.Genre;
using EntitiesLike = SongSpiration.Models.Entities.Like;
using EntitiesReport = SongSpiration.Models.Entities.Report;
using SongSpiration.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace SongSpiration.BLL.Services
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPinRepository _pinRepository;
        private readonly ILogger<AdminPanelService> _logger;

        private readonly SongSpirationDbContext _dbContext;

        public AdminPanelService(
            IUserRepository userRepository,
            IGenreRepository genreRepository,
            IPinRepository pinRepository,
            ILogger<AdminPanelService> logger,
            SongSpirationDbContext dbContext)
        {
            _userRepository = userRepository;
            _genreRepository = genreRepository;
            _pinRepository = pinRepository;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<List<ModelsGenre>> GetAllGenresAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie wszystkich gatunków z bazy danych");
                var entitiesGenres = await _dbContext.Genres.AsNoTracking().ToListAsync();

                _logger.LogInformation("Liczba pobranych gatunków: {Count}", entitiesGenres.Count);

                var genres = new List<ModelsGenre>();
                foreach (var entityGenre in entitiesGenres)
                {
                    genres.Add(new ModelsGenre {
                        Id = entityGenre.Id,
                        Name = entityGenre.Name,
                        Slug = entityGenre.Slug
                    });
                }

                _logger.LogInformation("Zwracanie {Count} gatunków", genres.Count);
                return genres;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania gatunków");
                throw;
            }
        }

        public async Task<IEnumerable<EntitiesUser>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie wszystkich użytkowników");

                var users = await _dbContext.Users.AsNoTracking().ToListAsync();

                _logger.LogInformation("Liczba pobranych użytkowników: {Count}", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania użytkowników");
                throw;
            }
        }

        // Pozostałe metody bez zmian...
        public async Task<EntitiesUser> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<List<EntitiesUser>> SearchUsersAsync(string searchTerm)
        {
            return (await _userRepository.SearchUsersAsync(searchTerm)).ToList();
        }

        public async Task<EntitiesUser> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = new EntitiesUser
            {
                Email = userCreateDto.Email,
                PasswordHash = userCreateDto.PasswordHash,
                Roles = "User",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                IsEmailVerified = false
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(Guid userId, UserUpdateDto userUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Email = userUpdateDto.Email;
            if (!string.IsNullOrEmpty(userUpdateDto.PasswordHash))
            {
                user.PasswordHash = userUpdateDto.PasswordHash;
            }
            user.Roles = userUpdateDto.Role;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<ModelsGenre> GetGenreByIdAsync(Guid genreId)
        {
            var entityGenre = await _genreRepository.GetByIdAsync(genreId);
            if (entityGenre == null)
            {
                throw new Exception("Genre not found");
            }
            return new ModelsGenre { Id = entityGenre.Id, Name = entityGenre.Name, Slug = entityGenre.Slug };
        }

        public async Task<ModelsGenre> CreateGenreAsync(GenreCreateDto genreCreateDto)
        {
            var entityGenre = new EntitiesGenre { Name = genreCreateDto.Name, Slug = genreCreateDto.Slug };
            await _genreRepository.AddAsync(entityGenre);
            await _genreRepository.SaveAsync();
            return new ModelsGenre { Id = entityGenre.Id, Name = entityGenre.Name, Slug = entityGenre.Slug };
        }

        public async Task UpdateGenreAsync(Guid genreId, GenreCreateDto genreCreateDto)
        {
            var entityGenre = await _genreRepository.GetByIdAsync(genreId);
            if (entityGenre == null)
            {
                throw new Exception("Genre not found");
            }

            entityGenre.Name = genreCreateDto.Name;
            entityGenre.Slug = genreCreateDto.Slug;

            _genreRepository.Update(entityGenre);
            await _genreRepository.SaveAsync();
        }

        public async Task DeleteGenreAsync(Guid genreId)
        {
            var entityGenre = await _genreRepository.GetByIdAsync(genreId);
            if (entityGenre == null)
            {
                throw new Exception("Genre not found");
            }

            _genreRepository.Delete(entityGenre);
            await _genreRepository.SaveAsync();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Usuń wszystkie powiązane rekordy w Likes
            var likes = _dbContext.Set<EntitiesLike>().Where(l => l.UserId == userId).ToList();
            _dbContext.Set<EntitiesLike>().RemoveRange(likes);

            // Usuń wszystkie powiązane rekordy w Reports (jako ReportedUser i ReportingUser)
            var reportsAsReported = _dbContext.Set<EntitiesReport>().Where(r => r.ReportedUserId == userId).ToList();
            var reportsAsReporting = _dbContext.Set<EntitiesReport>().Where(r => r.ReportingUserId == userId).ToList();
            _dbContext.Set<EntitiesReport>().RemoveRange(reportsAsReported);
            _dbContext.Set<EntitiesReport>().RemoveRange(reportsAsReporting);

            // Usuń wszystkie piny użytkownika przed usunięciem konta
            await DeletePinsForUserAsync(userId);

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task BanUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Roles = "Banned";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Roles = "User";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task PromoteToAdminAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Roles = "Admin";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DemoteFromAdminAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Roles = "User";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeletePinsForUserAsync(Guid userId)
        {
            var pins = await _pinRepository.GetPinsByUserIdAsync(userId);
            foreach (var pin in pins)
            {
                _pinRepository.Remove(pin);
            }
            await _pinRepository.SaveChangesAsync();
        }

        public async Task SetEditorChoiceAsync(Guid userId, bool isEditorChoice)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.IsEditorChoice = isEditorChoice;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}