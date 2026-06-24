using CheckerGame.Models;

namespace CheckerGame.Services
{
    public interface IGameService
    {
        Task<GameState> CreateGame(GameState gameState);
        Task<GameState?> GetGame(int id);
        Task<GameState?> ProcessMove(int gameId, int fromX, int fromY, int toX, int toY);
    }
}
