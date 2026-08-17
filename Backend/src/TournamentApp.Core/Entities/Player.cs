namespace TournamentApp.Core.Entities;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Seed { get; set; } // Position / Tête de série (1, 2, 3...)

    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;
}