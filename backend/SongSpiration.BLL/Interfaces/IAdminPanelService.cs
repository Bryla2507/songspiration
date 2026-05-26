using SongSpiration.BLL.DTOs;
using EntitiesUser = SongSpiration.Models.Entities.User;
using ModelsGenre = SongSpiration.Models.Genre;

namespace SongSpiration.BLL.Interfaces
{
    public interface IAdminPanelService
    {
        Task<EntitiesUser> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<EntitiesUser>> GetAllUsersAsync();
        Task<EntitiesUser> CreateUserAsync(UserCreateDto userCreateDto);
        Task UpdateUserAsync(Guid userId, UserUpdateDto userUpdateDto);
        Task DeleteUserAsync(Guid userId);
        Task<List<ModelsGenre>> GetAllGenresAsync();
        Task<ModelsGenre> GetGenreByIdAsync(Guid genreId);
        Task<ModelsGenre> CreateGenreAsync(GenreCreateDto genreCreateDto);
        Task UpdateGenreAsync(Guid genreId, GenreCreateDto genreCreateDto);
        Task DeleteGenreAsync(Guid genreId);
        Task<List<EntitiesUser>> SearchUsersAsync(string searchTerm);
        Task BanUserAsync(Guid userId);
        Task DeletePinsForUserAsync(Guid userId);
        Task SetEditorChoiceAsync(Guid userId, bool isEditorChoice);
    }
}
