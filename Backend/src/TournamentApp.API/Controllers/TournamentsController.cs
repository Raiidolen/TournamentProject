using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentApp.API.DTOs;
using TournamentApp.BLL.Services;
using TournamentApp.Core.Entities;
using TournamentApp.DAL.Data;

namespace TournamentApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly TournamentDbContext _context;
    private readonly TournamentService _tournamentService;

    public TournamentsController(TournamentDbContext context, TournamentService tournamentService)
    {
        _context = context;
        _tournamentService = tournamentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTournamentDto dto)
    {
        var tournament = new Tournament
        {
            Name = dto.Name,
            StrategyType = dto.StrategyType,
            JoinCode = Guid.NewGuid().ToString()[..6].ToUpper(),
            Players = dto.PlayerNames.Select((name, index) => new Player
            {
                Name = name,
                Seed = index + 1
            }).ToList()
        };

        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tournament.Id }, tournament.Id);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var t = await _context.Tournaments
            .Include(x => x.Players)
            .Include(x => x.Matches)
                .ThenInclude(m => m.Player1)
            .Include(x => x.Matches)
                .ThenInclude(m => m.Player2)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (t == null) return NotFound();

        var response = new TournamentResponseDto(
            t.Id, t.Name, t.JoinCode, t.Status.ToString(), t.StrategyType,
            t.Players.Select(p => new PlayerDto(p.Id, p.Name, p.Seed)).ToList(),
            t.Matches.Select(m => new MatchDto(
                m.Id, m.RoundNumber, m.MatchOrder,
                m.Player1Id, m.Player1?.Name, m.Score1,
                m.Player2Id, m.Player2?.Name, m.Score2,
                m.WinnerId, m.Status.ToString(), m.NextMatchId
            )).ToList()
        );

        return Ok(response);
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        var tournament = await _tournamentService.StartTournamentAsync(id);
        return Ok(new { message = "Tournoi démarré avec succès", tournamentId = tournament.Id });
    }
}