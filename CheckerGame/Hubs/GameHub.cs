using CheckerGame.Models;
using CheckerGame.Services.Game;
using CheckerGame.Services.MatchMaking;
using Microsoft.AspNetCore.SignalR;

namespace CheckerGame.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMatchMakingService _matchmakingService;
        private readonly IGameService _gameService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IMatchMakingService matchmakingService, IGameService gameService, ILogger<GameHub> logger)
        {
            _matchmakingService = matchmakingService;
            _gameService = gameService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Player connected: ConnectionId {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                _matchmakingService.LeaveQueue(userId);
                _logger.LogInformation("Player {UserId} disconnected and removed from queue.", userId);
            }

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

        public async Task JoinMatchmaking()
        {
            var userId = Context.UserIdentifier!;
            var connectionId = Context.ConnectionId;

            var result = _matchmakingService.JoinQueue(userId, connectionId);

            if (result.Waiting)
            {
                await Clients.Caller.SendAsync("WaitingForOpponent");
                _logger.LogInformation("Player {UserId} is waiting for opponent.", userId);
            }
            else
            {
                var gameState = new GameState
                {
                    Board = InitializeBoard(),
                    CurrentTurnPlayerId = 1,
                    Player1Id = int.Parse(result.Player1!.UserId),
                    Player2Id = int.Parse(result.Player2!.UserId)
                };

                var createdGame = await _gameService.CreateGame(gameState);

                await Clients.Client(result.Player1.ConnectionId).SendAsync("MatchFound", createdGame.Id);
                await Clients.Client(result.Player2.ConnectionId).SendAsync("MatchFound", createdGame.Id);

                _logger.LogInformation("Match created: GameId {GameId} between {P1} and {P2}",
                    createdGame.Id, result.Player1.UserId, result.Player2.UserId);
            }
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

        public async Task PlayAgainstAI()
        {
            var userId = Context.UserIdentifier!;
            var connectionId = Context.ConnectionId;

            var gameState = new GameState
            {
                Board = InitializeBoard(),
                CurrentTurnPlayerId = 1,
                Player1Id = int.Parse(userId),
                Player2Id = -1 
            };

            var createdGame = await _gameService.CreateGame(gameState);

            await Groups.AddToGroupAsync(connectionId, createdGame.Id.ToString());
            await Clients.Caller.SendAsync("MatchFoundAI", createdGame.Id);

            _logger.LogInformation("AI match created: GameId {GameId} for Player {UserId}", createdGame.Id, userId);
        }

        public async Task MakeMove(int gameId, int fromX, int fromY, int toX, int toY)
        {
            var game = await _gameService.ProcessMove(gameId, fromX, fromY, toX, toY);
            if (game == null)
            {
                _logger.LogWarning("Invalid move attempted in GameId {GameId}", gameId);
                return;
            }

            _logger.LogInformation(
                "Move broadcast in GameId {GameId} from ConnectionId {ConnectionId}: ({FromX},{FromY}) -> ({ToX},{ToY})",
                gameId, Context.ConnectionId, fromX, fromY, toX, toY);

            await Clients.Group(gameId.ToString()).SendAsync("GameUpdated", new
            {
                fromX,
                fromY,
                toX,
                toY,
                currentTurnPlayerId = game.CurrentTurnPlayerId
            });
        }

        private string InitializeBoard()
        {
            var board = new int[8, 8];
            return System.Text.Json.JsonSerializer.Serialize(board);
        }
    }
}
