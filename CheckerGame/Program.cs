using CheckerGame.Data;
using CheckerGame.Hubs;
using CheckerGame.Mappings;
using Mapster;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using FluentValidation;
using CheckerGame.Validators;
using Serilog;
using CheckerGame.Repositories.Game;
using CheckerGame.Services.Game;
using CheckerGame.Repositories.PlayerRepo;
using CheckerGame.Repositories.Auth;
using CheckerGame.Services.Leaderboard;
using CheckerGame.Services.Auth;
using CheckerGame.Repositories.UnitWork;
using CheckerGame.Repositories.Leaderboard;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/game-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// FluentValidation Registration
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Register all Validators
builder.Services.AddValidatorsFromAssemblyContaining<GameDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MoveRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PlayerDtoValidator>();

// Register Validators from Assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DB Connection String
builder.Services.AddDbContext<GameDBContex>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register SignalR
builder.Services.AddSignalR();

// Repository DI Registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();

// Services DI Registration
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Mapster
MappingConfig.RegisterMappings();
builder.Services.AddMapster();

var app = builder.Build();

// Custom error handling middleware
app.UseMiddleware<CheckerGame.Middleware.ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("/gamehub");

app.Run();
