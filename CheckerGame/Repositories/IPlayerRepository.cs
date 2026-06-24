using CheckerGame.Models;

namespace CheckerGame.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player?> GetPlayerAsync(int id);
        Task<Player?> AddPlayerAsync(Player player);
        Task SaveChangesAsync();
    }
}
