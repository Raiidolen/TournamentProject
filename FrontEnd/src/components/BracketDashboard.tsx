import React, { useEffect, useState } from 'react';
import type { Tournament, Match } from '../types/tournament';
import { api } from '../services/api';
import { signalRService } from '../services/signalr';
import './BracketDashboard.css';

interface Props {
  tournamentId: string;
}

export const BracketDashboard: React.FC<Props> = ({ tournamentId }) => {
  const [tournament, setTournament] = useState<Tournament | null>(null);
  const [isConnected, setIsConnected] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTournamentData = async () => {
    try {
      const data = await api.getTournament(tournamentId);
      setTournament(data);
    } catch {
      setError('Impossible de charger les données du tournoi.');
    }
  };

  useEffect(() => {
    fetchTournamentData();

    let isMounted = true;

    // Connexion SignalR pour le rafraîchissement temps réel
    signalRService.startConnection().then(() => {
      if (!isMounted) return;
      setIsConnected(true);
      signalRService.joinTournamentGroup(tournamentId);

      // Rechargement dynamique de l'arbre lors des mises à jour de scores
      signalRService.onScoreUpdated(() => fetchTournamentData());
      signalRService.onBracketUpdated(() => fetchTournamentData());
    }).catch(() => setIsConnected(false));

    return () => {
      isMounted = false;
      signalRService.leaveTournamentGroup(tournamentId);
      signalRService.removeListeners();
    };
  }, [tournamentId]);

  if (error) return <div className="tv-message error">{error}</div>;
  if (!tournament) return <div className="tv-message">Chargement du Live Dashboard...</div>;

  // Extraction et tri des tours
  const rounds = Array.from(new Set(tournament.matches.map((m) => m.roundNumber))).sort((a, b) => a - b);

  const getRoundLabel = (roundNumber: number, totalRounds: number): string => {
    if (roundNumber === totalRounds) return 'FINALE';
    if (roundNumber === totalRounds - 1) return 'DEMI-FINALES';
    if (roundNumber === totalRounds - 2) return 'QUARTS DE FINALE';
    return `TOUR ${roundNumber}`;
  };

  return (
    <div className="tv-dashboard">
      {/* En-tête Écran Géant */}
      <header className="tv-header">
        <div className="tv-title-area">
          <h1 className="tv-title">{tournament.name}</h1>
          <div className="tv-badge">
            Rejoindre : <span className="code">{tournament.joinCode}</span>
          </div>
        </div>

        <div className="tv-status">
          <span className={`status-indicator ${isConnected ? 'online' : 'offline'}`} />
          {isConnected ? 'EN DIRECT' : 'RECONNEXION...'}
        </div>
      </header>

      {/* Arbre de Tournoi Dynamique */}
      <div className="tv-bracket-wrapper">
        <div className="tv-bracket-grid">
          {rounds.map((round) => {
            const roundMatches = tournament.matches
              .filter((m) => m.roundNumber === round)
              .sort((a, b) => a.matchOrder - b.matchOrder);

            return (
              <div key={round} className="tv-round-column">
                <h2 className="tv-round-title">{getRoundLabel(round, rounds.length)}</h2>
                <div className="tv-matches-container">
                  {roundMatches.map((match) => (
                    <MatchCard key={match.id} match={match} />
                  ))}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
};

const MatchCard: React.FC<{ match: Match }> = ({ match }) => {
  const isP1Winner = match.winnerId && match.winnerId === match.player1Id;
  const isP2Winner = match.winnerId && match.winnerId === match.player2Id;

  return (
    <div className={`tv-match-card status-${match.status.toLowerCase()}`}>
      {/* Joueur 1 */}
      <div className={`tv-player-slot ${isP1Winner ? 'winner' : ''}`}>
        <span className="player-name">
          {match.player1Name || (match.status === 'Pending' ? 'À déterminer' : 'Exempté (BYE)')}
        </span>
        <span className="player-score">{match.score1}</span>
      </div>

      <div className="tv-card-divider" />

      {/* Joueur 2 */}
      <div className={`tv-player-slot ${isP2Winner ? 'winner' : ''}`}>
        <span className="player-name">
          {match.player2Name || (match.status === 'Pending' ? 'À déterminer' : 'Exempté (BYE)')}
        </span>
        <span className="player-score">{match.score2}</span>
      </div>
    </div>
  );
};