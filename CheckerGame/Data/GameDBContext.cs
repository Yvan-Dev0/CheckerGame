using CheckerGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckerGame.Data
{
    public class GameDBContext(DbContextOptions<DbContext> options) : DbContext(options)
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<GameState> Games { get; set; }
    }
}
