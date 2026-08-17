using TournamentApp.Core.Entities;
using TournamentApp.Core.Enums;
using TournamentApp.Core.Interfaces;

namespace TournamentApp.BLL.Strategies;

public class SingleEliminationStrategy : IBracketStrategy
{
    public string StrategyName => "SingleElimination";

    public List<Match> GenerateBracket(Tournament tournament, List<Player> players)
    {
        var matches = new List<Match>();
        int playerCount = players.Count;

        if (playerCount < 2)
            throw new InvalidOperationException("Un tournoi nécessite au moins 2 joueurs.");

        int totalRounds = (int)Math.Ceiling(Math.Log2(playerCount));
        int totalSlots = (int)Math.Pow(2, totalRounds);

        var roundMatchesMap = new Dictionary<int, List<Match>>();

        // 1. Instanciation des matchs
        for (int r = 1; r <= totalRounds; r++)
        {
            int matchesInRound = totalSlots / (int)Math.Pow(2, r);
            roundMatchesMap[r] = new List<Match>();

            for (int m = 0; m < matchesInRound; m++)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournament.Id,
                    RoundNumber = r,
                    MatchOrder = m,
                    Status = MatchStatus.Pending,
                    NextMatchId = null,
                    NextMatch = null // Ne pas assigner l'objet navigation
                };
                matches.Add(match);
                roundMatchesMap[r].Add(match);
            }
        }

        // 2. Lier uniquement par l'ID (NextMatchId)
        for (int r = 1; r < totalRounds; r++)
        {
            for (int m = 0; m < roundMatchesMap[r].Count; m++)
            {
                var parentMatch = roundMatchesMap[r + 1][m / 2];
                roundMatchesMap[r][m].NextMatchId = parentMatch.Id;
            }
        }

        // 3. Placement des joueurs au premier tour
        var round1Matches = roundMatchesMap[1];
        for (int i = 0; i < round1Matches.Count; i++)
        {
            var match = round1Matches[i];
            int p1Index = i * 2;
            int p2Index = i * 2 + 1;

            match.Player1Id = p1Index < playerCount ? players[p1Index].Id : null;
            match.Player2Id = p2Index < playerCount ? players[p2Index].Id : null;

            if (match.Player1Id != null && match.Player2Id != null)
            {
                match.Status = MatchStatus.Ready;
            }
            else if (match.Player1Id != null && match.Player2Id == null)
            {
                match.WinnerId = match.Player1Id;
                match.Status = MatchStatus.Completed;
            }
        }

        // 4. Propagation des Byes
        foreach (var byeMatch in round1Matches.Where(m => m.Status == MatchStatus.Completed))
        {
            AdvanceWinner(byeMatch, byeMatch.WinnerId!.Value, matches);
        }

        return matches;
    }

    public void AdvanceWinner(Match currentMatch, Guid winnerId, List<Match> allMatches)
    {
        if (currentMatch.NextMatchId == null) return;

        var nextMatch = allMatches.FirstOrDefault(m => m.Id == currentMatch.NextMatchId);
        if (nextMatch == null) return;

        if (currentMatch.MatchOrder % 2 == 0)
            nextMatch.Player1Id = winnerId;
        else
            nextMatch.Player2Id = winnerId;

        if (nextMatch.Player1Id != null && nextMatch.Player2Id != null)
        {
            nextMatch.Status = MatchStatus.Ready;
        }
    }
}