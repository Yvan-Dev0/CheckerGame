using CheckerGame.Data;
using CheckerGame.DTOs;
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
            var gameState = new GameState
            {
                Board = gameDto.Board,
                CurrentTurnPlayerId = gameDto.CurrentTurnPlayerId,
                IsFinished = gameDto.IsFinished
            };

            var createdGame = await _gameService.CreateGame(gameState);

            var response = new GameDto
            {
                Id = createdGame.Id,
                Board = createdGame.Board,
                CurrentTurnPlayerId= createdGame.CurrentTurnPlayerId,
                IsFinished = createdGame.IsFinished
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _gameService.GetGame(id);
            if (game == null) return NotFound();

            var response = new GameDto
            {
                Id = game.Id,
                Board = game.Board,
                CurrentTurnPlayerId = game.CurrentTurnPlayerId,
                IsFinished = game.IsFinished
            };

            return Ok(response);
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove(MoveRequestDto moveReqDto)
        {
            var updatedGame = await _gameService.ProcessMove(
                moveReqDto.GameId, moveReqDto.FromX, moveReqDto.FromY, moveReqDto.ToX, moveReqDto.ToY);
            if (updatedGame == null) return BadRequest("Invalid Move");

            var response = new GameDto
            {
                Id = updatedGame.Id,
                Board = updatedGame.Board,
                CurrentTurnPlayerId = updatedGame.CurrentTurnPlayerId,
                IsFinished = updatedGame.IsFinished
            };

            return Ok(updatedGame);
        }

    }
}
