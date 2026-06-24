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
       private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] GameState gameState)
        {
            var createdGame = await _gameService.CreateGame(gameState);
            return Ok(createdGame);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _gameService.GetGame(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove(int gameId, int fromX, int fromY, int toX, int toY)
        {
            var updatedGame = await _gameService.ProcessMove(gameId, fromX, fromY, toX, toY);
            if (updatedGame == null) return BadRequest("Invalid Move");
            return Ok(updatedGame);
        }

    }
}
