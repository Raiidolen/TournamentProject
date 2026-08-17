using Microsoft.AspNetCore.SignalR;

namespace TournamentApp.API.Hubs;

public interface ITournamentClient
{
    Task ReceiveScoreUpdated(Guid matchId, int score1, int score2, string status);
    Task ReceiveBracketUpdated(Guid tournamentId);
}

public class TournamentHub : Hub<ITournamentClient>
{
    public async Task JoinTournament(string tournamentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Tournament_{tournamentId}");
    }

    public async Task LeaveTournament(string tournamentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Tournament_{tournamentId}");
    }
}