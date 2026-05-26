using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using EntitiesUser = SongSpiration.Models.Entities.User;
using ModelsGenre = SongSpiration.Models.Genre;
using EntitiesGenre = SongSpiration.Models.Entities.Genre;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Services
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPinRepository _pinRepository;

        public AdminPanelService(IUserRepository userRepository, IGenreRepository genreRepository, IPinRepository pinRepository)
        {
            _userRepository = userRepository;
            _genreRepository = genreRepository;
            _pinRepository = pinRepository;
        }

        public async Task<EntitiesUser> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<EntitiesUser>> GetAllUsersAsync()
        {
            return await _userRepository.SearchUsersAsync("");
        }

        public async Task<EntitiesUser> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = new EntitiesUser
            {
                Email = userCreateDto.Email,
                PasswordHash = userCreateDto.PasswordHash,
                DisplayName = userCreateDto.Username,
                Roles = userCreateDto.Role,
                IsEmailVerified = true
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

            user.DisplayName = userUpdateDto.Username;
            user.Email = userUpdateDto.Email;
            user.PasswordHash = userUpdateDto.PasswordHash;
            user.Roles = userUpdateDto.Role;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<List<ModelsGenre>> GetAllGenresAsync()
        {
            var entitiesGenres = await _genreRepository.GetAllAsync();
            var genres = new List<ModelsGenre>();
            foreach (var entityGenre in entitiesGenres)
            {
            genres.Add(new ModelsGenre { Id = entityGenre.Id, Name = entityGenre.Name, Slug = entityGenre.Slug });
            }
            return genres;
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

        public async Task<List<EntitiesUser>> SearchUsersAsync(string searchTerm)
        {
            return (List<EntitiesUser>)await _userRepository.SearchUsersAsync(searchTerm);
        }

        public async Task BanUserAsync(Guid userId)
        {
            await _userRepository.BanUserAsync(userId);
        }

        public async Task DeletePinsForUserAsync(Guid userId)
        {
            await _userRepository.DeleteUserPinsAsync(userId);
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
