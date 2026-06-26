using CheckerGame.DTOs;
using CheckerGame.Models;
using CheckerGame.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckerGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            if (string.IsNullOrWhiteSpace(register.Username) || string.IsNullOrWhiteSpace(register.Password))
            {
                return BadRequest("Username and Password are required");
            }

            var player = new Player { Username = register.Username };
            var registered = await _authService.RegisterAsync(player, register.Password);

            _logger.LogInformation("Player {Username} registererd successfully", register.Username);
            return Ok(new { registered.Id, registered.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var player = await _authService.LoginAsync(login.Username, login.Password);
            if (player == null)
            {
                _logger.LogWarning("Invalid login attempt for {Username}", login.Username);
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { player.Id, player.Username });
        }
    }
}
