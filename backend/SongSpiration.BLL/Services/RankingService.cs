using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;

namespace SongSpiration.BLL.Services;

public class RankingService : IRankingService
{
    private readonly IUserRepository _userRepository;

    public RankingService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserRankingDto>> GetEditorsChoiceAsync(int limit = 10)
    {
        var users = await _userRepository.GetEditorChoiceUsersAsync(limit);
        return MapToDto(users);
    }

    public async Task<IEnumerable<UserRankingDto>> GetTopByDownloadsAsync(int limit = 10)
    {
        var users = await _userRepository.GetUsersByMostDownloadsAsync(limit);
        return MapToDto(users);
    }

    public async Task<IEnumerable<UserRankingDto>> GetTopByLikesAsync(int limit = 10)
    {
        var users = await _userRepository.GetUsersByMostLikesAsync(limit);
        return MapToDto(users);
    }

    private static IEnumerable<UserRankingDto> MapToDto(IEnumerable<User> users)
    {
        return users.Select(u => new UserRankingDto
        {
            Id = u.Id,
            DisplayName = u.DisplayName,
            AvatarUrl = u.AvatarUrl,
            IsEditorChoice = u.IsEditorChoice,
            TotalLikes = u.Pins?.SelectMany(p => p.Likes)?.Count() ?? 0,
            TotalDownloads = u.Pins?.Sum(p => p.DownloadsCount) ?? 0
        });
    }
}
