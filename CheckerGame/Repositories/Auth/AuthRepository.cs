using CheckerGame.Data;
using CheckerGame.Helper;
using CheckerGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckerGame.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly GameDBContex _contex;

        public AuthRepository(GameDBContex contex)
        {
            _contex = contex;
        }

        public async Task<Player?> GetPlayerByUsernameAsync(string username)
        {
            return await _contex.Players.FirstOrDefaultAsync(p => p.Username == username);
        }

        public async Task<Player> RegisterAsync(Player player, string password)
        {
            player.PasswordHash = PasswordHelper.HashPassword(password);
            _contex.Players.Add(player);
            await _contex.SaveChangesAsync();
            return player;
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var player = await GetPlayerByUsernameAsync(username);
            if (player == null) return false;

            return PasswordHelper.VerifyPassword(password, player.PasswordHash);
        }
    }
}
