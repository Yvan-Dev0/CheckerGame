using CheckerGame.Models;

namespace CheckerGame.Repositories.Leaderboard
{
    public interface ILeaderboardRepository
    {
        Task<IEnumerable<Player>> GetTopPlayersAsync(int count);
    }
}
