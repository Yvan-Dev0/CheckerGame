using System.Security.Claims;
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
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogInformation("Player connected: {UserId}, with ConnectionId {ConnectionId}", userId, Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning("Unauthenticated connection attempt from ConnectionId {ConnectionId}", Context.ConnectionId);
                Context.Abort();
                return;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _matchmakingService.LeaveQueue(userId);
                _logger.LogInformation("Player {UserId} disconnected and removed from queue.", userId);
            }
            else
            {
                _logger.LogWarning("Unauthenticated player disconnected: ConnectionId {ConnectionId}", Context.ConnectionId);
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
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthenticated user attempted to join matchmaking");
                await Clients.Caller.SendAsync("Error", "You must be logged in to join matchmaking");
                return;
            }

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
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthenticated user attempted to join game {GameId}", gameId);
                await Clients.Caller.SendAsync("Error", "You must be logged in to join a game");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            _logger.LogInformation("Player {UserId} (ConnectionId {ConnectionId}) joined GameId {GameId}",
                userId, Context.ConnectionId, gameId);

            await Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", userId);
        }

        public async Task LeaveGame(int gameId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthenticated user attempted to leave game {GameId}", gameId);
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
            _logger.LogInformation("Player {UserId} (ConnectionId {ConnectionId}) left GameId {GameId}",
                userId, Context.ConnectionId, gameId);
        }

        public async Task PlayAgainstAI()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthenticated user attempted to play against AI");
                await Clients.Caller.SendAsync("Error", "You must be logged in to play against AI");
                return;
            }

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
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthenticated user attempted to make a move in game {GameId}", gameId);
                await Clients.Caller.SendAsync("Error", "You must be logged in to make a move");
                return;
            }

            var game = await _gameService.ProcessMove(gameId, fromX, fromY, toX, toY);
            if (game == null)
            {
                _logger.LogWarning("Invalid move attempted in GameId {GameId} by Player {UserId}", gameId, userId);
                await Clients.Caller.SendAsync("Error", "Invalid move");
                return;
            }

            _logger.LogInformation(
                "Move broadcast in GameId {GameId} from Player {UserId} ({ConnectionId}): ({FromX},{FromY}) -> ({ToX},{ToY})",
                gameId, userId, Context.ConnectionId, fromX, fromY, toX, toY);

            await Clients.Group(gameId.ToString()).SendAsync("GameUpdated", new
            {
                fromX,
                fromY,
                toX,
                toY,
                currentTurnPlayerId = game.CurrentTurnPlayerId,
                movedBy = userId
            });
        }

        private string InitializeBoard()
        {
            var board = new int[8, 8];
            return System.Text.Json.JsonSerializer.Serialize(board);
        }
    }
}
