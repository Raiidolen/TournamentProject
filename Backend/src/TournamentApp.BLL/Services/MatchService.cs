using TournamentApp.BLL.Factories;
using TournamentApp.Core.Entities;
using TournamentApp.Core.Enums;
using TournamentApp.Core.Interfaces;

namespace TournamentApp.BLL.Services;

public class MatchService
{
    private readonly IRepository<Match> _matchRepo;
    private readonly IRepository<Tournament> _tournamentRepo;
    private readonly IBracketStrategyFactory _strategyFactory;

    public MatchService(
        IRepository<Match> matchRepo,
        IRepository<Tournament> tournamentRepo,
        IBracketStrategyFactory strategyFactory)
    {
        _matchRepo = matchRepo;
        _tournamentRepo = tournamentRepo;
        _strategyFactory = strategyFactory;
    }

    public async Task<Match> UpdateScoreAsync(Guid matchId, int score1, int score2)
    {
        var match = await _matchRepo.GetByIdAsync(matchId);
        if (match == null) throw new KeyNotFoundException("Match introuvable.");

        if (match.Status == MatchStatus.Pending)
            throw new InvalidOperationException("Le match n'est pas encore prêt.");

        match.Score1 = score1;
        match.Score2 = score2;

        // Si le score est égal, le match reste en cours
        if (score1 == score2)
        {
            match.Status = MatchStatus.InProgress;
            _matchRepo.Update(match);
            await _matchRepo.SaveChangesAsync();
            return match;
        }

        // Détermination du gagnant
        match.WinnerId = score1 > score2 ? match.Player1Id : match.Player2Id;
        match.Status = MatchStatus.Completed;

        // Propagation du gagnant vers le match suivant via la stratégie
        var tournament = await _tournamentRepo.GetByIdAsync(match.TournamentId);
        if (tournament != null)
        {
            var strategy = _strategyFactory.GetStrategy(tournament.StrategyType);
            var allMatches = await _matchRepo.FindAsync(m => m.TournamentId == match.TournamentId);

            strategy.AdvanceWinner(match, match.WinnerId!.Value, allMatches);
        }

        _matchRepo.Update(match);
        await _matchRepo.SaveChangesAsync();

        return match;
    }
}