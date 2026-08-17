using System.Numerics;
using System.Text.RegularExpressions;
using TournamentApp.Core.Enums;

namespace TournamentApp.Core.Entities;

public class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty; // Ex: "X7K9P2" pour accès rapide mobile
    public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
    public string StrategyType { get; set; } = "SingleElimination";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}