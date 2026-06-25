using CheckerGame.Models;

namespace CheckerGame.Repositories
{
    public interface ILeaderboardRepository
    {
        Task<IEnumerable<Player>> GetTopPlayersAsync(int count);
    }
}
