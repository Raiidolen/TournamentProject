using Microsoft.EntityFrameworkCore;
using TournamentApp.Core.Entities;

namespace TournamentApp.DAL.Data;

public class TournamentDbContext : DbContext
{
    public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options)
    {
    }

    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ----------------------------------------------------
        // Configuration Tournament
        // ----------------------------------------------------
        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.Property(t => t.JoinCode).IsRequired().HasMaxLength(10);
            entity.HasIndex(t => t.JoinCode).IsUnique(); // Index unique pour recherche rapide par QR Code
        });

        // ----------------------------------------------------
        // Configuration Player
        // ----------------------------------------------------
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(50);

            // Un joueur appartient à un tournoi (Suppression en cascade ok)
            entity.HasOne(p => p.Tournament)
                  .WithMany(t => t.Players)
                  .HasForeignKey(p => p.TournamentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ----------------------------------------------------
        // Configuration Match
        // ----------------------------------------------------
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(m => m.Id);

            // Un match appartient à un tournoi
            entity.HasOne(m => m.Tournament)
                  .WithMany(t => t.Matches)
                  .HasForeignKey(m => m.TournamentId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relations vers Player (DeleteBehavior.Restrict pour éviter les cascades multiples)
            entity.HasOne(m => m.Player1)
                  .WithMany()
                  .HasForeignKey(m => m.Player1Id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Player2)
                  .WithMany()
                  .HasForeignKey(m => m.Player2Id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Winner)
                  .WithMany()
                  .HasForeignKey(m => m.WinnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Auto-référence : Match du tour suivant
            entity.HasOne(m => m.NextMatch)
                  .WithMany()
                  .HasForeignKey(m => m.NextMatchId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}