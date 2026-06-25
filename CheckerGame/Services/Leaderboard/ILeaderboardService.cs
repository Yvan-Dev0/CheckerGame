using CheckerGame.Models;

namespace CheckerGame.Services.Leaderboard
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<Player>> GetTopPlayersAsync(int count);
    }
}
