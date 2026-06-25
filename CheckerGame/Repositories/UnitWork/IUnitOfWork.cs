using CheckerGame.Repositories.Auth;
using CheckerGame.Repositories.Game;
using CheckerGame.Repositories.Leaderboard;
using CheckerGame.Repositories.PlayerRepo;

namespace CheckerGame.Repositories.UnitWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGameRepository Games { get; }
        IPlayerRepository Players { get; }
        ILeaderboardRepository Leaderboard { get; }
        IAuthRepository Auth { get; }

        Task<int> CompleteAsync();
    }
}
