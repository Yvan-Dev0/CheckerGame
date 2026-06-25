using System.Collections.Concurrent;
using CheckerGame.Models;

namespace CheckerGame.Services.MatchMaking
{
    public class MatchMakingService : IMatchMakingService
    {
        private static readonly ConcurrentQueue<WaitingPlayer> _queue = new();
        private static readonly HashSet<string> _queuedUsers = new();
        private static readonly object _lock = new();

        public MatchResult JoinQueue(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (_queuedUsers.Contains(userId))
                {
                    return new MatchResult { Waiting = true };
                }

                if (_queue.TryDequeue(out var opponent))
                {
                    _queuedUsers.Remove(opponent.UserId);

                    return new MatchResult
                    {
                        Waiting = false,
                        Player1 = opponent,
                        Player2 = new WaitingPlayer { UserId = userId, ConnectionId = connectionId }
                    };
                }
                else
                {
                    var player = new WaitingPlayer { UserId = userId, ConnectionId = connectionId };
                    _queue.Enqueue(player);
                    _queuedUsers.Add(userId);

                    return new MatchResult { Waiting = true };
                }
            }
        }

        public void LeaveQueue(string userId)
        {
            lock (_lock)
            {
                _queuedUsers.Remove(userId);
            }
        }
    }
}
