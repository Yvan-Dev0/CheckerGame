using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CheckerGame.DTOs;
using CheckerGame.Models;
using CheckerGame.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CheckerGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
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

            var token = GenerateJwtToken(player);

            return Ok(new { player.Id, player.Username, Token = token });
        }

        private object GenerateJwtToken(Player player)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
                new Claim(ClaimTypes.Name, player.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
