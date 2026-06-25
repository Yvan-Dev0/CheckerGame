using CheckerGame.Data;
using CheckerGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckerGame.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly GameDBContex _context;

        public LeaderboardRepository(GameDBContex context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetTopPlayersAsync(int count)
        {
            return await _context.Players
                .OrderByDescending(p => p.Wins)
                .Take(count)
                .Select(p => new Player
                {
                    Id = p.Id,
                    Username = p.Username,
                    Wins = p.Wins,
                    Losses = p.Losses
                })
                .ToListAsync();
        }
    }
}
