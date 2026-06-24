using CheckerGame.Data;
using CheckerGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckerGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController(GameDBContext context) : ControllerBase
    {
        private readonly GameDBContext _context = context;

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
    }
}
