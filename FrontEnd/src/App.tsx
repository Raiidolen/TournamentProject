import { useState } from 'react';
import { BracketDashboard } from './components/BracketDashboard';

function App() {
  // Colle l'ID du tournoi récupéré depuis Swagger / la DB
  const [tournamentId, setTournamentId] = useState<string>('');
  const [activeId, setActiveId] = useState<string>('');

  const handleLoad = (e: React.FormEvent) => {
    e.preventDefault();
    if (tournamentId.trim()) {
      setActiveId(tournamentId.trim());
    }
  };

  return (
    <div className="app">
      {!activeId ? (
        <div style={{ 
          display: 'flex', 
          flexDirection: 'column', 
          alignItems: 'center', 
          justifyContent: 'center', 
          height: '100vh' 
        }}>
          <form onSubmit={handleLoad} style={{ textAlign: 'center' }}>
            <h2 style={{ marginBottom: '0.5rem' }}>Live Dashboard TV</h2>
            <p style={{ color: '#94a3b8', marginBottom: '1.5rem' }}>
              Entre le GUID d'un tournoi pour afficher l'arbre en direct
            </p>
            <input
              type="text"
              placeholder="ex: 3fa85f64-5717-4562-b3fc-2c963f66afa6"
              value={tournamentId}
              onChange={(e) => setTournamentId(e.target.value)}
              style={{
                padding: '0.8rem 1rem',
                width: '380px',
                borderRadius: '8px',
                border: '1px solid #334155',
                backgroundColor: '#1e293b',
                color: '#fff',
                fontSize: '0.95rem',
                marginRight: '0.5rem'
              }}
            />
            <button 
              type="submit" 
              style={{
                padding: '0.8rem 1.2rem',
                borderRadius: '8px',
                border: 'none',
                backgroundColor: '#3b82f6',
                color: '#fff',
                fontWeight: 'bold',
                cursor: 'pointer'
              }}
            >
              Afficher
            </button>
          </form>
        </div>
      ) : (
        <BracketDashboard tournamentId={activeId} />
      )}
    </div>
  );
}

export default App;