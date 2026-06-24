using CheckerGame.Data;
using CheckerGame.Models;
using CheckerGame.Repositories;

namespace CheckerGame.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<GameState> CreateGame(GameState gameState)
        {
            return await _gameRepository.AddGameAsync(gameState);
        }

        public async Task<GameState?> GetGame(int id)
        {
            return await _gameRepository.GetGameAsync(id);
        }

        public async Task<GameState?> ProcessMove(int gameId, int fromX, int fromY, int toX, int toY)
        {
            var game = await _gameRepository.GetGameAsync(gameId);
            if (game == null) return null;

            if (!ValidateMove(game, fromX, fromY, toX, toY))
                return null;

            ApplyMove(game, fromX, fromY, toX, toY);
            await _gameRepository.SaveChangesAsync();
            return game;
        }

        private bool ValidateMove(GameState gameState, int fromX, int fromY, int toX, int toY)
        {
            var board = System.Text.Json.JsonSerializer.Deserialize<int[,]>(gameState.Board);

            if (board[toX, toY] != 0) return false;

            int dx = toX - fromX;
            int dy = toY - fromY;

            if (Math.Abs(dx) == 1 && Math.Abs(dy) == 1)
                return true;

            if (Math.Abs(dx) == 2 && Math.Abs(dy) == 2)
            {
                int midX = (fromX + toX) / 2;
                int midY = (fromY + toY) / 2;

                if (board[midX, midY] != 0 && board[midX, midY] != board[fromX, fromY])
                    return true;
            }

            return false;
        }

        private void ApplyMove(GameState game, int fromX, int fromY, int toX, int toY)
        {
            var board = System.Text.Json.JsonSerializer.Deserialize<int[,]>(game.Board);

            board[toX, toY] = board[fromX, fromY];
            board[fromX, fromY] = 0;

            if (Math.Abs(toX - fromX) == 2)
            {
                int midX = (fromX + toX) / 2;
                int midY = (fromY + toY) / 2;
                board[midX, midY] = 0;
            }

            if (toY == 0 || toY == board.GetLength(1) - 1)
                board[toX, toY] = 3;

            game.Board = System.Text.Json.JsonSerializer.Serialize(board);
            game.CurrentTurnPlayerId = (game.CurrentTurnPlayerId == 1) ? 2 : 1;
        }
    }
}
