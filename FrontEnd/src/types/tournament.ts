export type TournamentStatus = 'Draft' | 'InProgress' | 'Completed';
export type MatchStatus = 'Pending' | 'Ready' | 'InProgress' | 'Completed';

export interface Player {
  id: string;
  name: string;
  seed: number;
}

export interface Match {
  id: string;
  roundNumber: number;
  matchOrder: number;
  player1Id: string | null;
  player1Name: string | null;
  score1: number;
  player2Id: string | null;
  player2Name: string | null;
  score2: number;
  winnerId: string | null;
  status: MatchStatus;
  nextMatchId: string | null;
}

export interface Tournament {
  id: string;
  name: string;
  joinCode: string;
  status: TournamentStatus;
  strategyType: string;
  players: Player[];
  matches: Match[];
}

export interface CreateTournamentPayload {
  name: string;
  strategyType: string;
  playerNames: string[];
}