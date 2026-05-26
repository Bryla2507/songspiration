using System.Collections.Generic;
using System.Threading.Tasks;
using SongSpiration.BLL.DTOs;

namespace SongSpiration.BLL.Interfaces;

public interface IRankingService
{
    Task<IEnumerable<UserRankingDto>> GetEditorsChoiceAsync(int limit = 10);
    Task<IEnumerable<UserRankingDto>> GetTopByLikesAsync(int limit = 10);
    Task<IEnumerable<UserRankingDto>> GetTopByDownloadsAsync(int limit = 10);
}
