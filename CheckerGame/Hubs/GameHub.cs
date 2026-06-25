using Microsoft.AspNetCore.SignalR;

namespace CheckerGame.Hubs
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Player connected: ConnectionId {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception,
                    "Player disconnected with error: ConnectionId {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Player disconnected: ConnectionId {ConnectionId}", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGame(int gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            _logger.LogInformation("ConnectionId {ConnectionId} joined GameId {GameId}", Context.ConnectionId, gameId);

            await Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", Context.ConnectionId);
        }

        public async Task LeaveGame(int gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
            _logger.LogInformation("ConnectionId {ConnectionId} left GameId {GameId}", Context.ConnectionId, gameId);
        }

        public async Task MakeMove(int gameId, int fromX, int fromY, int toX, int toY)
        {
            _logger.LogInformation(
                "Move broadcast in GameId {GameId} from ConnectionId {ConnectionId}: ({FromX},{FromY}) -> ({ToX},{ToY})",
                gameId, Context.ConnectionId, fromX, fromY, toX, toY);

            await Clients.Group(gameId.ToString()).SendAsync("GameUpdated", new
            {
                fromX,
                fromY,
                toX,
                toY
            });
        }
    }
}
