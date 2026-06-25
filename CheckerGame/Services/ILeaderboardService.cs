using CheckerGame.Models;

namespace CheckerGame.Services
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<Player>> GetTopPlayersAsync(int count);
    }
}
