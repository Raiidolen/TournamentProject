using TournamentApp.Core.Enums;

namespace TournamentApp.Core.Entities;

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public int RoundNumber { get; set; } // 1 = Premier tour, 2 = Quarts, etc.
    public int MatchOrder { get; set; }  // Position verticale dans le tour

    // Joueur 1
    public Guid? Player1Id { get; set; }
    public Player? Player1 { get; set; }
    public int Score1 { get; set; } = 0;

    // Joueur 2
    public Guid? Player2Id { get; set; }
    public Player? Player2 { get; set; }
    public int Score2 { get; set; } = 0;

    // Résultat
    public Guid? WinnerId { get; set; }
    public Player? Winner { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Pending;

    // Auto-référence : Pointeur vers le match du tour suivant
    public Guid? NextMatchId { get; set; }
    public Match? NextMatch { get; set; }
}