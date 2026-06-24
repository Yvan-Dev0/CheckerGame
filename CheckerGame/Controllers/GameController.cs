using CheckerGame.DTOs;
using Mapster;
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
        public async Task<IActionResult> CreateGame([FromBody] GameDto gameDto)
        {
            var gameState = gameDto.Adapt<GameState>();
            var createdGame = await _gameService.CreateGame(gameState);
            return Ok(createdGame.Adapt<GameDto>());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _gameService.GetGame(id);
            if (game == null) return NotFound();
            return Ok(game.Adapt<GameDto>());
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove(MoveRequestDto moveReqDto)
        {
            var updatedGame = await _gameService.ProcessMove(
                moveReqDto.GameId, moveReqDto.FromX, moveReqDto.FromY, moveReqDto.ToX, moveReqDto.ToY);
            if (updatedGame == null) return BadRequest("Invalid Move");

            return Ok(updatedGame.Adapt<GameDto>());
        }

    }
}
