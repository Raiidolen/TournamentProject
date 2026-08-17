using TournamentApp.Core.Interfaces;

namespace TournamentApp.BLL.Factories;

public interface IBracketStrategyFactory
{
    IBracketStrategy GetStrategy(string strategyName);
}

public class BracketStrategyFactory : IBracketStrategyFactory
{
    private readonly IEnumerable<IBracketStrategy> _strategies;

    public BracketStrategyFactory(IEnumerable<IBracketStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IBracketStrategy GetStrategy(string strategyName)
    {
        var strategy = _strategies.FirstOrDefault(s => s.StrategyName.Equals(strategyName, StringComparison.OrdinalIgnoreCase));
        return strategy ?? throw new ArgumentException($"La stratégie '{strategyName}' n'est pas reconnue.");
    }
}