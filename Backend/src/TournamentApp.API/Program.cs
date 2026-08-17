using Microsoft.EntityFrameworkCore;
using TournamentApp.API.Hubs;
using TournamentApp.BLL.Factories;
using TournamentApp.BLL.Services;
using TournamentApp.BLL.Strategies;
using TournamentApp.Core.Interfaces;
using TournamentApp.DAL.Data;
using TournamentApp.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Database
builder.Services.AddDbContext<TournamentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repositories & Services BLL
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBracketStrategy, SingleEliminationStrategy>();
builder.Services.AddScoped<IBracketStrategyFactory, BracketStrategyFactory>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddScoped<MatchService>();

// 3. SignalR
builder.Services.AddSignalR();

// 4. CORS (Autorise le Frontend React Vite)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Obligatoire pour WebSockets / SignalR
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Application de la politique CORS
app.UseCors("ReactAppPolicy");

app.UseAuthorization();

app.MapControllers();

// Mappage de la route SignalR Hub
app.MapHub<TournamentHub>("/hubs/tournament");

app.Run();