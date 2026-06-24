using CheckerGame.Data;
using CheckerGame.Hubs;
using CheckerGame.Mappings;
using Mapster;
using CheckerGame.Repositories;
using CheckerGame.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using FluentValidation;
using CheckerGame.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// FluentValidation Registration
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<GameDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MoveRequestDtoValidator>();

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

// DI Registration
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

// Register Mapster
MappingConfig.RegisterMappings();
builder.Services.AddMapster();

var app = builder.Build();

app.MapHub<GameHub>("/gamehub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
