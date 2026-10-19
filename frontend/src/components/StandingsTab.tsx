import React, { useEffect, useState } from 'react';
import { Trophy, Flame, RefreshCw, Shield, AlertTriangle, Activity, Award } from 'lucide-react';
import { api } from '../services/api';
import type { TournamentStanding, TopScorer } from '../types';

export const StandingsTab: React.FC = () => {
  const [standings, setStandings] = useState<TournamentStanding[]>([]);
  const [topScorers, setTopScorers] = useState<TopScorer[]>([]);
  const [topLimit, setTopLimit] = useState<number | undefined>(10);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [posData, scorersData] = await Promise.all([
        api.getStandings(),
        api.getTopScorers(topLimit),
      ]);
      setStandings(posData);
      setTopScorers(scorersData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar datos del torneo.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [topLimit]);

  // Derived metrics
  const totalMatchesPlayed = Math.floor(
    standings.reduce((sum, item) => sum + item.partidosJugados, 0) / 2
  );
  const totalGoals = standings.reduce((sum, item) => sum + item.golesAFavor, 0);
  const currentLeader = standings[0];
  const topGoalScorer = topScorers[0];

  return (
    <div className="fade-in" style={{ display: 'flex', flexDirection: 'column', gap: '2rem' }}>
      {/* Header and Refresh */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
        <div>
          <h2 style={{ fontSize: '1.5rem', fontWeight: 700, color: '#f8fafc', display: 'flex', alignItems: 'center', gap: '10px' }}>
            <Trophy color="#f59e0b" size={26} /> Tabla de Posiciones
          </h2>
          <p style={{ color: '#94a3b8', fontSize: '0.9rem', marginTop: '4px' }}>
            Calculada automáticamente con Dapper. Criterio: Puntos, Diferencia de Gol, Goles a Favor.
          </p>
        </div>
        <button
          onClick={fetchData}
          disabled={loading}
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '8px',
            backgroundColor: '#1e293b',
            color: '#f8fafc',
            padding: '8px 16px',
            borderRadius: '8px',
            border: '1px solid #334155',
            fontWeight: 500,
          }}
        >
          <RefreshCw size={16} className={loading ? 'spin' : ''} />
          Actualizar
        </button>
      </div>

      {error && (
        <div style={{
          padding: '12px 16px',
          borderRadius: '8px',
          backgroundColor: 'rgba(239, 68, 68, 0.1)',
          border: '1px solid rgba(239, 68, 68, 0.3)',
          color: '#f87171',
          display: 'flex',
          alignItems: 'center',
          gap: '10px',
        }}>
          <AlertTriangle size={20} />
          <span>{error}</span>
        </div>
      )}

      {/* Metrics Summary Cards */}
      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
        gap: '1rem',
      }}>
        <div style={{
          backgroundColor: '#131b2e',
          border: '1px solid #2a3756',
          borderRadius: '12px',
          padding: '1.2rem',
          display: 'flex',
          alignItems: 'center',
          gap: '14px',
        }}>
          <div style={{
            width: '42px',
            height: '42px',
            borderRadius: '10px',
            backgroundColor: 'rgba(245, 158, 11, 0.15)',
            color: '#fbbf24',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <Trophy size={22} />
          </div>
          <div>
            <div style={{ fontSize: '0.8rem', color: '#94a3b8' }}>Líder Actual</div>
            <div style={{ fontSize: '1.1rem', fontWeight: 700, color: '#f8fafc' }}>
              {currentLeader ? currentLeader.equipoNombre : 'Por definir'}
            </div>
            {currentLeader && (
              <span style={{ fontSize: '0.75rem', color: '#34d399', fontWeight: 600 }}>
                {currentLeader.puntos} Pts ({currentLeader.diferenciaGoles > 0 ? `+${currentLeader.diferenciaGoles}` : currentLeader.diferenciaGoles} DG)
              </span>
            )}
          </div>
        </div>

        <div style={{
          backgroundColor: '#131b2e',
          border: '1px solid #2a3756',
          borderRadius: '12px',
          padding: '1.2rem',
          display: 'flex',
          alignItems: 'center',
          gap: '14px',
        }}>
          <div style={{
            width: '42px',
            height: '42px',
            borderRadius: '10px',
            backgroundColor: 'rgba(239, 68, 68, 0.15)',
            color: '#f87171',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <Flame size={22} />
          </div>
          <div>
            <div style={{ fontSize: '0.8rem', color: '#94a3b8' }}>Máximo Goleador</div>
            <div style={{ fontSize: '1.1rem', fontWeight: 700, color: '#f8fafc' }}>
              {topGoalScorer ? topGoalScorer.jugadorNombre : 'Sin goles'}
            </div>
            {topGoalScorer && (
              <span style={{ fontSize: '0.75rem', color: '#fbbf24', fontWeight: 600 }}>
                {topGoalScorer.goles} goles ({topGoalScorer.equipoNombre})
              </span>
            )}
          </div>
        </div>

        <div style={{
          backgroundColor: '#131b2e',
          border: '1px solid #2a3756',
          borderRadius: '12px',
          padding: '1.2rem',
          display: 'flex',
          alignItems: 'center',
          gap: '14px',
        }}>
          <div style={{
            width: '42px',
            height: '42px',
            borderRadius: '10px',
            backgroundColor: 'rgba(59, 130, 246, 0.15)',
            color: '#60a5fa',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <Activity size={22} />
          </div>
          <div>
            <div style={{ fontSize: '0.8rem', color: '#94a3b8' }}>Partidos Disputados</div>
            <div style={{ fontSize: '1.3rem', fontWeight: 800, color: '#f8fafc' }}>
              {totalMatchesPlayed}
            </div>
            <span style={{ fontSize: '0.75rem', color: '#94a3b8' }}>
              En el torneo actual
            </span>
          </div>
        </div>

        <div style={{
          backgroundColor: '#131b2e',
          border: '1px solid #2a3756',
          borderRadius: '12px',
          padding: '1.2rem',
          display: 'flex',
          alignItems: 'center',
          gap: '14px',
        }}>
          <div style={{
            width: '42px',
            height: '42px',
            borderRadius: '10px',
            backgroundColor: 'rgba(16, 185, 129, 0.15)',
            color: '#34d399',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <Award size={22} />
          </div>
          <div>
            <div style={{ fontSize: '0.8rem', color: '#94a3b8' }}>Goles Totales</div>
            <div style={{ fontSize: '1.3rem', fontWeight: 800, color: '#34d399' }}>
              {totalGoals}
            </div>
            <span style={{ fontSize: '0.75rem', color: '#94a3b8' }}>
              {totalMatchesPlayed > 0 ? (totalGoals / totalMatchesPlayed).toFixed(2) : '0'} por partido
            </span>
          </div>
        </div>
      </div>

      {/* Standings Table Card */}
      <div style={{
        backgroundColor: '#131b2e',
        borderRadius: '14px',
        border: '1px solid #2a3756',
        overflow: 'hidden',
        boxShadow: '0 4px 16px rgba(0,0,0,0.25)',
      }}>
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '0.95rem' }}>
            <thead>
              <tr style={{ backgroundColor: '#1c2640', color: '#94a3b8', borderBottom: '1px solid #2a3756' }}>
                <th style={{ padding: '14px 16px', width: '60px', textAlign: 'center' }}>#</th>
                <th style={{ padding: '14px 16px' }}>Equipo</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Partidos Jugados">PJ</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Partidos Ganados">PG</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Partidos Empatados">PE</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Partidos Perdidos">PP</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Goles a Favor">GF</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Goles en Contra">GC</th>
                <th style={{ padding: '14px 12px', textAlign: 'center' }} title="Diferencia de Goles">DG</th>
                <th style={{ padding: '14px 16px', textAlign: 'center', fontWeight: 700, color: '#34d399' }} title="Puntos">PTS</th>
              </tr>
            </thead>
            <tbody>
              {loading && standings.length === 0 ? (
                <tr>
                  <td colSpan={10} style={{ padding: '40px', textAlign: 'center', color: '#64748b' }}>
                    Cargando posiciones...
                  </td>
                </tr>
              ) : standings.length === 0 ? (
                <tr>
                  <td colSpan={10} style={{ padding: '40px', textAlign: 'center', color: '#64748b' }}>
                    No hay equipos o partidos registrados todavía.
                  </td>
                </tr>
              ) : (
                standings.map((team, idx) => {
                  const isLeader = team.posicion === 1;
                  return (
                    <tr
                      key={team.equipoId}
                      style={{
                        borderBottom: '1px solid #1e293b',
                        backgroundColor: isLeader ? 'rgba(245, 158, 11, 0.05)' : idx % 2 === 0 ? 'transparent' : 'rgba(255,255,255,0.01)',
                        transition: 'background-color 0.15s',
                      }}
                    >
                      <td style={{ padding: '14px 16px', textAlign: 'center', fontWeight: 700 }}>
                        {isLeader ? (
                          <span style={{
                            display: 'inline-flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            width: '28px',
                            height: '28px',
                            borderRadius: '50%',
                            backgroundColor: 'rgba(245, 158, 11, 0.2)',
                            color: '#fbbf24',
                            border: '1px solid #f59e0b',
                          }}>
                            {team.posicion}
                          </span>
                        ) : (
                          <span style={{ color: '#94a3b8' }}>{team.posicion}</span>
                        )}
                      </td>
                      <td style={{ padding: '14px 16px' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                          {team.logoUrl ? (
                            <img
                              src={team.logoUrl}
                              alt={team.equipoNombre}
                              style={{ width: '32px', height: '32px', borderRadius: '50%', objectFit: 'contain', backgroundColor: '#fff', padding: '2px' }}
                              onError={(e) => {
                                (e.target as HTMLElement).style.display = 'none';
                              }}
                            />
                          ) : (
                            <div style={{
                              width: '32px',
                              height: '32px',
                              borderRadius: '50%',
                              backgroundColor: '#1e293b',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              color: '#94a3b8',
                            }}>
                              <Shield size={16} />
                            </div>
                          )}
                          <span style={{ fontWeight: 600, color: '#f8fafc' }}>{team.equipoNombre}</span>
                        </div>
                      </td>
                      <td style={{ padding: '14px 12px', textAlign: 'center', color: '#94a3b8' }}>{team.partidosJugados}</td>
                      <td style={{ padding: '14px 12px', textAlign: 'center', color: '#f8fafc' }}>{team.partidosGanados}</td>
                      <td style={{ padding: '14px 12px', textAlign: 'center', color: '#94a3b8' }}>{team.partidosEmpatados}</td>
                      <td style={{ padding: '14px 12px', textAlign: 'center', color: '#94a3b8' }}>{team.partidosPerdidos}</td>
                      <td style={{ padding: '14px 12px', textAlign: 'center', color: '#94a3b8' }}>{team.golesAFavor}</td>
                      <td style={{ padding: '14px 12px', textAlign: 'center', color: '#94a3b8' }}>{team.golesEnContra}</td>
                      <td style={{
                        padding: '14px 12px',
                        textAlign: 'center',
                        fontWeight: 600,
                        color: team.diferenciaGoles > 0 ? '#34d399' : team.diferenciaGoles < 0 ? '#f87171' : '#94a3b8',
                      }}>
                        {team.diferenciaGoles > 0 ? `+${team.diferenciaGoles}` : team.diferenciaGoles}
                      </td>
                      <td style={{
                        padding: '14px 16px',
                        textAlign: 'center',
                        fontWeight: 800,
                        fontSize: '1.05rem',
                        color: '#34d399',
                      }}>
                        {team.puntos}
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Top Scorers Section */}
      <div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', flexWrap: 'wrap', gap: '8px' }}>
          <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc', display: 'flex', alignItems: 'center', gap: '8px' }}>
            <Flame color="#ef4444" size={22} /> Tabla de Goleadores
          </h3>

          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <span style={{ fontSize: '0.85rem', color: '#94a3b8' }}>Mostrar:</span>
            <select
              value={topLimit === undefined ? 'all' : topLimit.toString()}
              onChange={(e) => {
                const val = e.target.value;
                setTopLimit(val === 'all' ? undefined : parseInt(val));
              }}
              style={{ padding: '4px 10px', fontSize: '0.85rem' }}
            >
              <option value="5">Top 5</option>
              <option value="10">Top 10</option>
              <option value="all">Todos</option>
            </select>
          </div>
        </div>

        <div style={{
          backgroundColor: '#131b2e',
          borderRadius: '14px',
          border: '1px solid #2a3756',
          overflow: 'hidden',
          boxShadow: '0 4px 16px rgba(0,0,0,0.25)',
        }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '0.95rem' }}>
            <thead>
              <tr style={{ backgroundColor: '#1c2640', color: '#94a3b8', borderBottom: '1px solid #2a3756' }}>
                <th style={{ padding: '12px 16px', width: '60px', textAlign: 'center' }}>#</th>
                <th style={{ padding: '12px 16px' }}>Jugador</th>
                <th style={{ padding: '12px 16px' }}>Equipo</th>
                <th style={{ padding: '12px 16px' }}>Posición</th>
                <th style={{ padding: '12px 16px', textAlign: 'center', color: '#f87171', fontWeight: 700 }}>Goles</th>
              </tr>
            </thead>
            <tbody>
              {topScorers.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ padding: '30px', textAlign: 'center', color: '#64748b' }}>
                    No hay goles registrados en partidos finalizados aún.
                  </td>
                </tr>
              ) : (
                topScorers.map((scorer) => (
                  <tr key={scorer.jugadorId} style={{ borderBottom: '1px solid #1e293b' }}>
                    <td style={{ padding: '12px 16px', textAlign: 'center', fontWeight: 700, color: '#fbbf24' }}>
                      {scorer.posicion}
                    </td>
                    <td style={{ padding: '12px 16px', fontWeight: 600, color: '#f8fafc' }}>
                      {scorer.jugadorNombre}
                    </td>
                    <td style={{ padding: '12px 16px', color: '#94a3b8' }}>
                      {scorer.equipoNombre}
                    </td>
                    <td style={{ padding: '12px 16px' }}>
                      <span className="badge badge-blue">{scorer.posicionJugador}</span>
                    </td>
                    <td style={{ padding: '12px 16px', textAlign: 'center', fontWeight: 800, color: '#f87171', fontSize: '1.1rem' }}>
                      ⚽ {scorer.goles}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
