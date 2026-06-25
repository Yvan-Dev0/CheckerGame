using CheckerGame.Models;

namespace CheckerGame.Services.MatchMaking
{
    public interface IMatchMakingService
    {
        MatchResult JoinQueue(string userId, string connectionId);
        void LeaveQueue(string userId);
    }
}
