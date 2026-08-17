using TournamentApp.Core.Entities;

namespace TournamentApp.Core.Interfaces;

public interface IBracketStrategy
{
    string StrategyName { get; }
    List<Match> GenerateBracket(Tournament tournament, List<Player> players);
    void AdvanceWinner(Match currentMatch, Guid winnerId, List<Match> allMatches);
}