using CheckerGame.Models;

namespace CheckerGame.Services
{
    public interface IAuthService
    {
        Task<Player?> LoginAsync(string username, string passwordHash);
        Task<Player> RegisterAsync(Player player, string passwordHash);
    }
}
