using CheckerGame.Models;
using CheckerGame.Repositories;

namespace CheckerGame.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<Player?> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Login attempt for {Username}", username);
            var valid = await _authRepository.ValidateCredentialsAsync(username, password);

            if (!valid)
            {
                _logger.LogWarning("Invalid login attempt for {Username}", username);
                return null;
            }

            return await _authRepository.GetPlayerByUsernameAsync(username);

        }

        public async Task<Player> RegisterAsync(Player player, string password)
        {
            _logger.LogInformation("Registering new player {Username}", player.Username);
            return await _authRepository.RegisterAsync(player, password);
        }
    }
}
