using CheckerGame.Data;
using CheckerGame.Models;

namespace CheckerGame.Repositories.Game
{
    public class GameRepository : IGameRepository
    {
        private readonly GameDBContex _context;

        public GameRepository(GameDBContex contex)
        {
            _context = contex;
        }

        public async Task<GameState> AddGameAsync(GameState game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<GameState?> GetGameAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
