using CheckerGame.Models;

namespace CheckerGame.Repositories.PlayerRepo
{
    public interface IPlayerRepository
    {
        Task<Player?> GetPlayerAsync(int id);
        Task<Player?> AddPlayerAsync(Player player);
        Task SaveChangesAsync();
    }
}
