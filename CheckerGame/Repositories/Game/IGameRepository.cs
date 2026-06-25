using CheckerGame.Models;

namespace CheckerGame.Repositories.Game
{
    public interface IGameRepository
    {
        Task<GameState?> GetGameAsync(int id);
        Task<GameState> AddGameAsync(GameState game);
        Task SaveChangesAsync();
    }
}
