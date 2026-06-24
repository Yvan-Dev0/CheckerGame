using CheckerGame.Data;
using CheckerGame.Models;

namespace CheckerGame.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly GameDBContex _context;

        public PlayerRepository(GameDBContex context)
        {
            _context = context;
        }

        public async Task<Player?> AddPlayerAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<Player?> GetPlayerAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
