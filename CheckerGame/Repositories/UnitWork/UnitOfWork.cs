using CheckerGame.Data;
using CheckerGame.Repositories.Auth;
using CheckerGame.Repositories.Game;
using CheckerGame.Repositories.Leaderboard;
using CheckerGame.Repositories.PlayerRepo;

namespace CheckerGame.Repositories.UnitWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameDBContex _context;

        public UnitOfWork(GameDBContex contex,
                          IGameRepository gameRepository,
                          IPlayerRepository playerRepository,
                          ILeaderboardRepository leaderboardRepository,
                          IAuthRepository authRepository)
        {
            _context = contex;
            Games = gameRepository;
            Players = playerRepository;
            Leaderboard = leaderboardRepository;
            Auth = authRepository;
        }

        public IGameRepository Games { get; }
        public IPlayerRepository Players { get; }
        public ILeaderboardRepository Leaderboard { get; }
        public IAuthRepository Auth { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
