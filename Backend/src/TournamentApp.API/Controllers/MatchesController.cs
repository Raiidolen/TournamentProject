using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TournamentApp.API.DTOs;
using TournamentApp.API.Hubs;
using TournamentApp.BLL.Services;

namespace TournamentApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly MatchService _matchService;
    private readonly IHubContext<TournamentHub, ITournamentClient> _hubContext;

    public MatchesController(MatchService matchService, IHubContext<TournamentHub, ITournamentClient> hubContext)
    {
        _matchService = matchService;
        _hubContext = hubContext;
    }

    [HttpPut("{id:guid}/score")]
    public async Task<IActionResult> UpdateScore(Guid id, UpdateScoreDto dto)
    {
        var match = await _matchService.UpdateScoreAsync(id, dto.Score1, dto.Score2);

        // Notification temps réel envoyée à tous les clients du tournoi (Live Dashboard TV & Mobiles)
        await _hubContext.Clients
            .Group($"Tournament_{match.TournamentId}")
            .ReceiveScoreUpdated(match.Id, match.Score1, match.Score2, match.Status.ToString());

        await _hubContext.Clients
            .Group($"Tournament_{match.TournamentId}")
            .ReceiveBracketUpdated(match.TournamentId);

        return Ok(new { message = "Score mis à jour", matchId = match.Id });
    }
}