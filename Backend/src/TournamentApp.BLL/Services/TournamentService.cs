using Microsoft.EntityFrameworkCore;
using TournamentApp.BLL.Factories;
using TournamentApp.Core.Entities;
using TournamentApp.Core.Enums;
using TournamentApp.DAL.Data;

namespace TournamentApp.BLL.Services;

public class TournamentService
{
    private readonly TournamentDbContext _context;
    private readonly IBracketStrategyFactory _strategyFactory;

    public TournamentService(
        TournamentDbContext context,
        IBracketStrategyFactory strategyFactory)
    {
        _context = context;
        _strategyFactory = strategyFactory;
    }

    public async Task<Tournament> StartTournamentAsync(Guid tournamentId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
            throw new KeyNotFoundException("Tournoi introuvable.");

        if (tournament.Status != TournamentStatus.Draft)
            throw new InvalidOperationException("Le tournoi est déjà démarré ou terminé.");

        var strategy = _strategyFactory.GetStrategy(tournament.StrategyType);
        var matches = strategy.GenerateBracket(tournament, tournament.Players.ToList());

        tournament.Status = TournamentStatus.InProgress;

        // Forcer explicitement le state 'Added' pour chaque nouveau match
        foreach (var match in matches)
        {
            _context.Entry(match).State = EntityState.Added;
        }

        await _context.SaveChangesAsync();

        return tournament;
    }
}