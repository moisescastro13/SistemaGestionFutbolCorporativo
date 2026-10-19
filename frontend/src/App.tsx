import React, { useState, useEffect } from 'react';
import { Trophy, Shield, Users, Calendar, ExternalLink, Activity } from 'lucide-react';
import { StandingsTab } from './components/StandingsTab';
import { TeamsTab } from './components/TeamsTab';
import { PlayersTab } from './components/PlayersTab';
import { MatchesTab } from './components/MatchesTab';
import { Alert } from './components/Alert';

type TabType = 'torneo' | 'equipos' | 'jugadores' | 'partidos';

export const App: React.FC = () => {
  const [activeTab, setActiveTab] = useState<TabType>('torneo');
  const [notification, setNotification] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [backendOnline, setBackendOnline] = useState<boolean | null>(null);

  // Ping backend health
  useEffect(() => {
    const checkHealth = async () => {
      try {
        const res = await fetch('/health');
        setBackendOnline(res.ok);
      } catch {
        setBackendOnline(false);
      }
    };
    checkHealth();
    const interval = setInterval(checkHealth, 30000);
    return () => clearInterval(interval);
  }, []);

  const notify = (type: 'success' | 'error', message: string) => {
    setNotification({ type, message });
    setTimeout(() => {
      setNotification((curr) => (curr?.message === message ? null : curr));
    }, 5000);
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      {/* Top Navbar */}
      <header style={{
        backgroundColor: '#131b2e',
        borderBottom: '1px solid #2a3756',
        position: 'sticky',
        top: 0,
        zIndex: 50,
      }}>
        <div style={{
          maxWidth: '1200px',
          margin: '0 auto',
          padding: '0.85rem 1.5rem',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          flexWrap: 'wrap',
          gap: '1rem',
        }}>
          {/* Logo & Brand */}
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <div style={{
              width: '38px',
              height: '38px',
              borderRadius: '10px',
              backgroundColor: '#10b981',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              boxShadow: '0 0 16px rgba(16, 185, 129, 0.4)',
            }}>
              <span style={{ fontSize: '1.4rem' }}>⚽</span>
            </div>
            <div>
              <h1 style={{ fontSize: '1.15rem', fontWeight: 800, color: '#f8fafc', letterSpacing: '-0.02em' }}>
                Fútbol Corporativo
              </h1>
              <span style={{ fontSize: '0.75rem', color: '#94a3b8' }}>
                Gestión de Torneos & Equipos
              </span>
            </div>
          </div>

          {/* Navigation Tabs */}
          <nav style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
            <button
              onClick={() => setActiveTab('torneo')}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '8px',
                padding: '8px 16px',
                borderRadius: '8px',
                fontWeight: 600,
                fontSize: '0.9rem',
                backgroundColor: activeTab === 'torneo' ? '#1c2640' : 'transparent',
                color: activeTab === 'torneo' ? '#34d399' : '#94a3b8',
                border: `1px solid ${activeTab === 'torneo' ? '#2a3756' : 'transparent'}`,
              }}
            >
              <Trophy size={16} /> Torneo
            </button>

            <button
              onClick={() => setActiveTab('equipos')}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '8px',
                padding: '8px 16px',
                borderRadius: '8px',
                fontWeight: 600,
                fontSize: '0.9rem',
                backgroundColor: activeTab === 'equipos' ? '#1c2640' : 'transparent',
                color: activeTab === 'equipos' ? '#60a5fa' : '#94a3b8',
                border: `1px solid ${activeTab === 'equipos' ? '#2a3756' : 'transparent'}`,
              }}
            >
              <Shield size={16} /> Equipos
            </button>

            <button
              onClick={() => setActiveTab('jugadores')}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '8px',
                padding: '8px 16px',
                borderRadius: '8px',
                fontWeight: 600,
                fontSize: '0.9rem',
                backgroundColor: activeTab === 'jugadores' ? '#1c2640' : 'transparent',
                color: activeTab === 'jugadores' ? '#c084fc' : '#94a3b8',
                border: `1px solid ${activeTab === 'jugadores' ? '#2a3756' : 'transparent'}`,
              }}
            >
              <Users size={16} /> Jugadores
            </button>

            <button
              onClick={() => setActiveTab('partidos')}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '8px',
                padding: '8px 16px',
                borderRadius: '8px',
                fontWeight: 600,
                fontSize: '0.9rem',
                backgroundColor: activeTab === 'partidos' ? '#1c2640' : 'transparent',
                color: activeTab === 'partidos' ? '#fbbf24' : '#94a3b8',
                border: `1px solid ${activeTab === 'partidos' ? '#2a3756' : 'transparent'}`,
              }}
            >
              <Calendar size={16} /> Partidos
            </button>
          </nav>

          {/* Links & API status */}
          <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
            <div
              title={backendOnline ? 'Backend conectado' : 'Backend desconectado'}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '6px',
                fontSize: '0.8rem',
                color: backendOnline ? '#34d399' : '#f87171',
                padding: '4px 10px',
                backgroundColor: '#1c2640',
                borderRadius: '999px',
                border: '1px solid #2a3756',
              }}
            >
              <Activity size={13} />
              <span>{backendOnline ? 'API Conectada' : 'Sin Conexión'}</span>
            </div>

            <a
              href="http://localhost:5193/swagger"
              target="_blank"
              rel="noreferrer"
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '5px',
                fontSize: '0.85rem',
                color: '#60a5fa',
                textDecoration: 'none',
                fontWeight: 500,
              }}
            >
              <span>Swagger</span>
              <ExternalLink size={13} />
            </a>
          </div>
        </div>
      </header>

      {/* Main Container */}
      <main style={{ flex: 1, maxWidth: '1200px', width: '100%', margin: '0 auto', padding: '2rem 1.5rem' }}>
        {activeTab === 'torneo' && <StandingsTab />}
        {activeTab === 'equipos' && <TeamsTab onNotify={notify} />}
        {activeTab === 'jugadores' && <PlayersTab onNotify={notify} />}
        {activeTab === 'partidos' && <MatchesTab onNotify={notify} />}
      </main>

      {/* Footer */}
      <footer style={{
        backgroundColor: '#131b2e',
        borderTop: '1px solid #2a3756',
        padding: '1.25rem',
        textAlign: 'center',
        fontSize: '0.8rem',
        color: '#64748b',
      }}>
        Sistema de Gestión de Fútbol Corporativo &bull; .NET 9 Clean Architecture + React Frontend
      </footer>

      {/* Notification Toast */}
      {notification && (
        <Alert
          type={notification.type}
          message={notification.message}
          onClose={() => setNotification(null)}
        />
      )}
    </div>
  );
};

export default App;
