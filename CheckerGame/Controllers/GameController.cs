using CheckerGame.DTOs;
using Mapster;
using CheckerGame.Models;
using CheckerGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CheckerGame.Hubs;

namespace CheckerGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;
        public readonly IHubContext<GameHub> _hubContext;

        public GameController(
            IGameService gameService, 
            ILogger<GameController> logger, 
            IHubContext<GameHub> hubContext)
        {
            _gameService = gameService;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] GameDto gameDto)
        {
            _logger.LogInformation("CreateGame request received");
            var createdGame = await _gameService.CreateGame(gameDto.Adapt<GameState>());
            return Ok(createdGame.Adapt<GameDto>());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            _logger.LogInformation("GetGame request received for GameId {GameId}", id);
            
            var game = await _gameService.GetGame(id);
            if (game == null)
            {
                _logger.LogWarning("Game {GameId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Game {GameId} successfully retrieved", id);
            return Ok(game.Adapt<GameDto>());
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove(MoveRequestDto moveReqDto)
        {
            _logger.LogInformation(
                "Move request received for GameId {GameId}: ({FromX}, {FromY} -> {ToX}, {ToY})",
                moveReqDto.GameId, moveReqDto.FromX, moveReqDto.FromY, moveReqDto.ToX, moveReqDto.ToY
                );

            var updatedGame = await _gameService.ProcessMove(
                moveReqDto.GameId, moveReqDto.FromX, moveReqDto.FromY, moveReqDto.ToX, moveReqDto.ToY);

            if (updatedGame == null)
            {
                _logger.LogWarning(
                    "Invalid move attempted in GameId {GameId}: ({FromX}, {FromY} -> {ToX}, {ToY})",
                    moveReqDto.GameId, moveReqDto.FromX, moveReqDto.FromY, moveReqDto.ToX, moveReqDto.ToY
                    );
                return BadRequest("Invalid Move");
            }

            _logger.LogInformation(
                "Move applied successfully in GameId {GameId}: ({FromX}, {FromY} -> {ToX}, {ToY})",
                moveReqDto.GameId, moveReqDto.FromX, moveReqDto.FromY, moveReqDto.ToX, moveReqDto.ToY
                );

            await _hubContext.Clients.Group(moveReqDto.GameId.ToString()).SendAsync("GameUpdated", new
            {
                fromX = moveReqDto.FromX,
                fromY = moveReqDto.FromY,
                toX = moveReqDto.ToX,
                toY = moveReqDto.ToY,
                currentTurnPlayerId = updatedGame.CurrentTurnPlayerId
            });

            return Ok(updatedGame.Adapt<GameDto>());
        }

    }
}
