using CheckerGame.Models;

namespace CheckerGame.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<Player?> GetPlayerByUsernameAsync(string username);
        Task<Player> RegisterAsync(Player player, string passwordHash);
        Task<bool> ValidateCredentialsAsync(string username, string passwordHash);
    }
}
