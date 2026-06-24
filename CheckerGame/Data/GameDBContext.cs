using CheckerGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckerGame.Data
{
    public class GameDBContex : DbContext
    {
        public GameDBContex(DbContextOptions<GameDBContex> options) : base(options) {}

        public DbSet<Player> Players { get; set; }
        public DbSet<GameState> Games { get; set; }
    }
}
