using CheckerGame.Data;
using CheckerGame.Models;

namespace CheckerGame.Services
{
    public class GameService
    {
        private readonly GameDBContex _context;

        public GameService(GameDBContex contex)
        {
            _context = contex;
        }

        public async Task<bool> ValidateMove(GameState gameState,
                int fromX, int fromY, int toX, int toY)
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
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<GameState> ApplyMove(GameState gameState,
                int fromX, int fromY, int toX, int toY)
        {
            var board = System.Text.Json.JsonSerializer.Deserialize<int[,]>(gameState.Board);

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

            gameState.Board = System.Text.Json.JsonSerializer.Serialize(board);
            gameState.CurrentTurnPlayerId = (gameState.CurrentTurnPlayerId == 1) ? 2 : 1;

            await _context.SaveChangesAsync();
            return gameState;
        }

    }
}
