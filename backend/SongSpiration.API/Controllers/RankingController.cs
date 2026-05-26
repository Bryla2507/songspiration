using Microsoft.AspNetCore.Mvc;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongSpiration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingController : ControllerBase
{
    private readonly IRankingService _rankingService;

    public RankingController(IRankingService rankingService)
    {
        _rankingService = rankingService;
    }

    [HttpGet("editors-choice")]
    public async Task<ActionResult<IEnumerable<UserRankingDto>>> GetEditorsChoice([FromQuery] int limit = 10)
    {
        var users = await _rankingService.GetEditorsChoiceAsync(limit);
        return Ok(users);
    }

    [HttpGet("likes")]
    public async Task<ActionResult<IEnumerable<UserRankingDto>>> GetTopByLikes([FromQuery] int limit = 10)
    {
        var users = await _rankingService.GetTopByLikesAsync(limit);
        return Ok(users);
    }

    [HttpGet("downloads")]
    public async Task<ActionResult<IEnumerable<UserRankingDto>>> GetTopByDownloads([FromQuery] int limit = 10)
    {
        var users = await _rankingService.GetTopByDownloadsAsync(limit);
        return Ok(users);
    }
}
