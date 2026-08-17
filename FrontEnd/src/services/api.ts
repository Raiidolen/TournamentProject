import type { CreateTournamentPayload, Tournament } from '../types/tournament';

// Adapte le port selon l'URL de ton API .NET (ex: https://localhost:7189 ou http://localhost:5000)
const API_BASE_URL = 'http://localhost:5245/api';

export const api = {
  // Créer un tournoi
  createTournament: async (payload: CreateTournamentPayload): Promise<string> => {
    const response = await fetch(`${API_BASE_URL}/tournaments`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    if (!response.ok) throw new Error('Erreur lors de la création du tournoi');
    return await response.json(); // Retourne l'Id du tournoi
  },

  // Récupérer les détails complets d'un tournoi (avec matchs et joueurs)
  getTournament: async (id: string): Promise<Tournament> => {
    const response = await fetch(`${API_BASE_URL}/tournaments/${id}`);
    if (!response.ok) throw new Error('Tournoi introuvable');
    return await response.json();
  },

  // Démarrer le tournoi (génère l'arbre)
  startTournament: async (id: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/tournaments/${id}/start`, {
      method: 'POST',
    });
    if (!response.ok) throw new Error('Erreur lors du démarrage du tournoi');
  },

  // Mettre à jour un score depuis un mobile / tableau d'affichage
  updateScore: async (matchId: string, score1: number, score2: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/matches/${matchId}/score`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ score1, score2 }),
    });
    if (!response.ok) throw new Error('Erreur lors de la mise à jour du score');
  },
};