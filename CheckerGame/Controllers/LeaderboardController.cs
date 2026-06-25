using CheckerGame.Services.Leaderboard;
using Microsoft.AspNetCore.Mvc;

namespace CheckerGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILogger<LeaderboardController> _logger;

        public LeaderboardController(ILeaderboardService leaderboardService, ILogger<LeaderboardController> logger)
        {
            _leaderboardService = leaderboardService;
            _logger = logger;   
        }

        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTopPlayers(int count)
        {
            _logger.LogInformation("Leaderboard request received to top {Count} players", count);

            var players = await _leaderboardService.GetTopPlayersAsync(count);

            if(!players.Any())
            {
                _logger.LogWarning("No players found for leaderboard request");
                return NotFound("No players found");
            }

            return Ok(players);
        }
    }
}
