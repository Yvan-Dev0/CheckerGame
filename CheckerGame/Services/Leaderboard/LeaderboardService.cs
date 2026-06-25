using CheckerGame.Models;
using CheckerGame.Repositories.Leaderboard;

namespace CheckerGame.Services.Leaderboard
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly ILogger<LeaderboardService> _logger;

        public LeaderboardService(ILeaderboardRepository leaderboardRepository, ILogger<LeaderboardService> logger)
        {
            _leaderboardRepository = leaderboardRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Player>> GetTopPlayersAsync(int count)
        {
            _logger.LogInformation("Fetching top {Count} players for leaderboard", count);
            return await _leaderboardRepository.GetTopPlayersAsync(count);
        }
    }
}
