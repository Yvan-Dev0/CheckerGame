using CheckerGame.Models;
using CheckerGame.Repositories.Auth;
using CheckerGame.Repositories.UnitWork;

namespace CheckerGame.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Player?> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Login attempt for {Username}", username);
            var valid = await _unitOfWork.Auth.ValidateCredentialsAsync(username, password);

            if (!valid)
            {
                _logger.LogWarning("Invalid login attempt for {Username}", username);
                return null;
            }

            return await _unitOfWork.Auth.GetPlayerByUsernameAsync(username);

        }

        public async Task<Player> RegisterAsync(Player player, string password)
        {
            _logger.LogInformation("Registering new player {Username}", player.Username);
            var regitered = await _unitOfWork.Auth.RegisterAsync(player, password);

            await _unitOfWork.CompleteAsync();
            return regitered;
        }
    }
}
