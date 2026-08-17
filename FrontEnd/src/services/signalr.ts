import * as signalR from '@microsoft/signalr';

const HUB_URL = 'http://localhost:5245/hubs/tournament';

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  // Démarrer la connexion SignalR
  public async startConnection(): Promise<signalR.HubConnection> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      return this.connection;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    await this.connection.start();
    console.log('SignalR Connecté !');
    return this.connection;
  }

  // Rejoindre le groupe spécifique d'un tournoi
  public async joinTournamentGroup(tournamentId: string): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('JoinTournament', tournamentId);
    }
  }

  // Quitter le groupe d'un tournoi
  public async leaveTournamentGroup(tournamentId: string): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('LeaveTournament', tournamentId);
    }
  }

  // Écouter la mise à jour dynamique d'un score
  public onScoreUpdated(callback: (matchId: string, score1: number, score2: number, status: string) => void): void {
    this.connection?.on('ReceiveScoreUpdated', callback);
  }

  // Écouter le rechargement global de l'arbre
  public onBracketUpdated(callback: (tournamentId: string) => void): void {
    this.connection?.on('ReceiveBracketUpdated', callback);
  }

  // Arrêter proprement les écouteurs lors du démontage du composant
  public removeListeners(): void {
    this.connection?.off('ReceiveScoreUpdated');
    this.connection?.off('ReceiveBracketUpdated');
  }
}

export const signalRService = new SignalRService();