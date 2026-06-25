using CheckerGame.Models;
using CheckerGame.Repositories.UnitWork;

namespace CheckerGame.Services.Leaderboard
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LeaderboardService> _logger;

        public LeaderboardService(IUnitOfWork unitOfWork, ILogger<LeaderboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Player>> GetTopPlayersAsync(int count)
        {
            _logger.LogInformation("Fetching top {Count} players for leaderboard", count);
            return await _unitOfWork.Leaderboard.GetTopPlayersAsync(count);
        }
    }
}
