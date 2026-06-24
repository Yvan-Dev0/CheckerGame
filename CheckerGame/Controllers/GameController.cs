using CheckerGame.Data;
using CheckerGame.Models;
using CheckerGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckerGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameDBContex _context;

        public GameController(GameDBContex contex)
        {
            _context = contex;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] GameState gameState)
        {
            _context.Games.Add(gameState);
            await _context.SaveChangesAsync();
            return Ok(gameState);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove(int gameId, int fromX, int fromY, int toX, int toY)
        {
            var game = await _context.Games.FindAsync(gameId);

            if (game == null ) return NotFound();

            var service = new GameService(_context);

            if (!await service.ValidateMove(game, fromX, fromY, toX, toY))
            {
                return BadRequest("Invalid Move");
            }

            var updatedGame = await service.ApplyMove(game, fromX, fromY, toX, toY);

            return Ok(updatedGame);
        }

    }
}
