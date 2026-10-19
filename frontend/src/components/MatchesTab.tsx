import React, { useEffect, useState } from 'react';
import { Calendar, Plus, CheckCircle2, Clock, X, Shield } from 'lucide-react';
import { api } from '../services/api';
import type { MatchSummary, MatchDetail, TeamSummary, TeamDetail, PagedResponse } from '../types';

interface MatchesTabProps {
  onNotify: (type: 'success' | 'error', message: string) => void;
}

interface NewGoalRow {
  jugadorId: string;
  equipoId: string;
  minuto?: number;
  esAutogol: boolean;
}

export const MatchesTab: React.FC<MatchesTabProps> = ({ onNotify }) => {
  const [matchesData, setMatchesData] = useState<PagedResponse<MatchSummary> | null>(null);
  const [teams, setTeams] = useState<TeamSummary[]>([]);
  const [jornadaFilter, setJornadaFilter] = useState<string>('');
  const [estadoFilter, setEstadoFilter] = useState<string>('');
  const [equipoFilter, setEquipoFilter] = useState<string>('');
  const [page, setPage] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);

  // Modals
  const [showScheduleModal, setShowScheduleModal] = useState<boolean>(false);
  const [equipoLocalId, setEquipoLocalId] = useState<string>('');
  const [equipoVisitanteId, setEquipoVisitanteId] = useState<string>('');
  const [jornada, setJornada] = useState<number>(1);
  const [fechaHora, setFechaHora] = useState<string>(new Date().toISOString().slice(0, 16));
  const [submitting, setSubmitting] = useState<boolean>(false);

  // Detail Modal
  const [selectedMatchDetail, setSelectedMatchDetail] = useState<MatchDetail | null>(null);

  // Result Modal
  const [matchToRegister, setMatchToRegister] = useState<MatchSummary | null>(null);
  const [golesLocal, setGolesLocal] = useState<number>(0);
  const [golesVisitante, setGolesVisitante] = useState<number>(0);
  const [localSquad, setLocalSquad] = useState<TeamDetail | null>(null);
  const [visitorSquad, setVisitorSquad] = useState<TeamDetail | null>(null);
  const [goalRows, setGoalRows] = useState<NewGoalRow[]>([]);

  useEffect(() => {
    const loadTeams = async () => {
      try {
        const res = await api.getTeams({ pageSize: 100 });
        setTeams(res.data);
        if (res.data.length >= 2) {
          setEquipoLocalId(res.data[0].id);
          setEquipoVisitanteId(res.data[1].id);
        }
      } catch {
        // Ignored
      }
    };
    loadTeams();
  }, []);

  const fetchMatches = async () => {
    try {
      setLoading(true);
      const res = await api.getMatches({
        pageNumber: page,
        pageSize: 10,
        jornada: jornadaFilter ? parseInt(jornadaFilter) : undefined,
        equipoId: equipoFilter || undefined,
        estado: estadoFilter || undefined,
      });
      setMatchesData(res);
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al cargar partidos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMatches();
  }, [page, jornadaFilter, estadoFilter, equipoFilter]);

  const handleScheduleMatch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (equipoLocalId === equipoVisitanteId) {
      onNotify('error', 'Un equipo no puede jugar contra sí mismo.');
      return;
    }

    try {
      setSubmitting(true);
      await api.scheduleMatch({
        equipoLocalId,
        equipoVisitanteId,
        jornada: Number(jornada),
        fechaHora: new Date(fechaHora).toISOString(),
      });
      onNotify('success', 'Partido programado exitosamente.');
      setShowScheduleModal(false);
      fetchMatches();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al programar partido');
    } finally {
      setSubmitting(false);
    }
  };

  const handleOpenRegisterResult = async (match: MatchSummary) => {
    setMatchToRegister(match);
    setGolesLocal(0);
    setGolesVisitante(0);
    setGoalRows([]);

    try {
      const [lSquad, vSquad] = await Promise.all([
        api.getTeamById(match.equipoLocalId),
        api.getTeamById(match.equipoVisitanteId),
      ]);
      setLocalSquad(lSquad);
      setVisitorSquad(vSquad);
    } catch {
      onNotify('error', 'Error al cargar las plantillas de los equipos.');
    }
  };

  const handleAddGoalRow = () => {
    if (!localSquad || !visitorSquad) return;
    const defaultTeamId = matchToRegister!.equipoLocalId;
    const defaultPlayerId = localSquad.jugadores[0]?.id || '';
    setGoalRows([
      ...goalRows,
      {
        equipoId: defaultTeamId,
        jugadorId: defaultPlayerId,
        minuto: 45,
        esAutogol: false,
      },
    ]);
  };

  const handleRemoveGoalRow = (index: number) => {
    setGoalRows(goalRows.filter((_, idx) => idx !== index));
  };

  const handleSubmitResult = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!matchToRegister) return;

    const totalMarcador = Number(golesLocal) + Number(golesVisitante);
    if (goalRows.length > 0 && goalRows.length !== totalMarcador) {
      onNotify(
        'error',
        `El desglose de goles (${goalRows.length}) no coincide con el total del marcador (${totalMarcador}). Puedes dejar los detalles vacíos o completarlos todos.`
      );
      return;
    }

    try {
      setSubmitting(true);
      await api.registerMatchResult(matchToRegister.id, {
        golesLocal: Number(golesLocal),
        golesVisitante: Number(golesVisitante),
        goles: goalRows.length > 0 ? goalRows : undefined,
      });

      onNotify('success', 'Resultado del partido registrado y torneo actualizado.');
      setMatchToRegister(null);
      fetchMatches();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al registrar resultado');
    } finally {
      setSubmitting(false);
    }
  };

  const handleViewDetail = async (id: string) => {
    try {
      const detail = await api.getMatchById(id);
      setSelectedMatchDetail(detail);
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al obtener detalle');
    }
  };

  return (
    <div className="fade-in" style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
      {/* Top Filter Bar */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', flexWrap: 'wrap' }}>
          <select
            value={jornadaFilter}
            onChange={(e) => {
              setJornadaFilter(e.target.value);
              setPage(1);
            }}
          >
            <option value="">Todas las Jornadas</option>
            <option value="1">Jornada 1</option>
            <option value="2">Jornada 2</option>
            <option value="3">Jornada 3</option>
            <option value="4">Jornada 4</option>
            <option value="5">Jornada 5</option>
          </select>

          <select
            value={estadoFilter}
            onChange={(e) => {
              setEstadoFilter(e.target.value);
              setPage(1);
            }}
          >
            <option value="">Todos los Estados</option>
            <option value="Pendiente">Pendientes</option>
            <option value="Finalizado">Finalizados</option>
          </select>

          <select
            value={equipoFilter}
            onChange={(e) => {
              setEquipoFilter(e.target.value);
              setPage(1);
            }}
          >
            <option value="">Todos los Equipos</option>
            {teams.map((t) => (
              <option key={t.id} value={t.id}>
                {t.nombre}
              </option>
            ))}
          </select>
        </div>

        <button
          onClick={() => setShowScheduleModal(true)}
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '8px',
            backgroundColor: '#10b981',
            color: '#fff',
            padding: '9px 18px',
            borderRadius: '8px',
            fontWeight: 600,
          }}
        >
          <Plus size={18} />
          Programar Partido
        </button>
      </div>

      {/* Matches Grid */}
      {loading && !matchesData ? (
        <div style={{ textAlign: 'center', padding: '50px', color: '#64748b' }}>Cargando partidos...</div>
      ) : matchesData?.data.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '50px', color: '#64748b' }}>No se encontraron partidos.</div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(340px, 1fr))', gap: '1.25rem' }}>
          {matchesData?.data.map((m) => {
            const isFinalizado = m.estado.toLowerCase() === 'finalizado';
            return (
              <div
                key={m.id}
                style={{
                  backgroundColor: '#131b2e',
                  border: '1px solid #2a3756',
                  borderRadius: '14px',
                  padding: '1.25rem',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '1rem',
                  boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
                }}
              >
                {/* Header: Jornada and Estado */}
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <span className="badge badge-blue">Jornada {m.jornada}</span>
                  <span className={`badge ${isFinalizado ? 'badge-success' : 'badge-warning'}`}>
                    {isFinalizado ? <CheckCircle2 size={12} /> : <Clock size={12} />}
                    {m.estado}
                  </span>
                </div>

                {/* Scoreboard */}
                <div style={{ display: 'grid', gridTemplateColumns: '1fr auto 1fr', alignItems: 'center', gap: '12px', padding: '0.75rem 0' }}>
                  {/* Local */}
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: '6px' }}>
                    {m.equipoLocalLogoUrl ? (
                      <img
                        src={m.equipoLocalLogoUrl}
                        alt={m.equipoLocalNombre}
                        style={{ width: '42px', height: '42px', borderRadius: '50%', objectFit: 'contain', backgroundColor: '#fff', padding: '2px' }}
                      />
                    ) : (
                      <Shield size={36} color="#60a5fa" />
                    )}
                    <span style={{ fontWeight: 600, fontSize: '0.9rem', color: '#f8fafc' }}>
                      {m.equipoLocalNombre}
                    </span>
                  </div>

                  {/* Marker */}
                  <div style={{ textAlign: 'center' }}>
                    {isFinalizado ? (
                      <div style={{
                        backgroundColor: '#1c2640',
                        padding: '6px 14px',
                        borderRadius: '10px',
                        fontWeight: 800,
                        fontSize: '1.4rem',
                        color: '#34d399',
                        letterSpacing: '2px',
                        border: '1px solid #2a3756',
                      }}>
                        {m.golesLocal} - {m.golesVisitante}
                      </div>
                    ) : (
                      <div style={{
                        padding: '4px 12px',
                        borderRadius: '6px',
                        fontWeight: 700,
                        fontSize: '0.9rem',
                        color: '#64748b',
                        backgroundColor: '#1e293b',
                      }}>
                        VS
                      </div>
                    )}
                  </div>

                  {/* Visitante */}
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: '6px' }}>
                    {m.equipoVisitanteLogoUrl ? (
                      <img
                        src={m.equipoVisitanteLogoUrl}
                        alt={m.equipoVisitanteNombre}
                        style={{ width: '42px', height: '42px', borderRadius: '50%', objectFit: 'contain', backgroundColor: '#fff', padding: '2px' }}
                      />
                    ) : (
                      <Shield size={36} color="#f87171" />
                    )}
                    <span style={{ fontWeight: 600, fontSize: '0.9rem', color: '#f8fafc' }}>
                      {m.equipoVisitanteNombre}
                    </span>
                  </div>
                </div>

                {/* Footer and Actions */}
                <div style={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                  paddingTop: '0.75rem',
                  borderTop: '1px solid #1e293b',
                  fontSize: '0.8rem',
                  color: '#64748b',
                }}>
                  <span style={{ display: 'flex', alignItems: 'center', gap: '5px' }}>
                    <Calendar size={13} />
                    {new Date(m.fechaHora).toLocaleString([], { dateStyle: 'short', timeStyle: 'short' })}
                  </span>

                  {isFinalizado ? (
                    <button
                      onClick={() => handleViewDetail(m.id)}
                      style={{
                        padding: '5px 12px',
                        backgroundColor: '#1e293b',
                        color: '#60a5fa',
                        borderRadius: '6px',
                        fontWeight: 600,
                        border: '1px solid #334155',
                      }}
                    >
                      Ver Detalle
                    </button>
                  ) : (
                    <button
                      onClick={() => handleOpenRegisterResult(m)}
                      style={{
                        padding: '5px 12px',
                        backgroundColor: '#059669',
                        color: '#fff',
                        borderRadius: '6px',
                        fontWeight: 600,
                      }}
                    >
                      Registrar Resultado
                    </button>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* Pagination controls */}
      {matchesData && matchesData.totalPages > 1 && (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '1rem', marginTop: '0.5rem' }}>
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            style={{
              padding: '6px 14px',
              backgroundColor: '#1e293b',
              color: page === 1 ? '#475569' : '#f8fafc',
              borderRadius: '6px',
              border: '1px solid #334155',
            }}
          >
            Anterior
          </button>
          <span style={{ color: '#94a3b8', fontSize: '0.9rem' }}>
            Página {matchesData.pageNumber} de {matchesData.totalPages} ({matchesData.totalRecords} partidos)
          </span>
          <button
            onClick={() => setPage((p) => Math.min(matchesData.totalPages, p + 1))}
            disabled={page === matchesData.totalPages}
            style={{
              padding: '6px 14px',
              backgroundColor: '#1e293b',
              color: page === matchesData.totalPages ? '#475569' : '#f8fafc',
              borderRadius: '6px',
              border: '1px solid #334155',
            }}
          >
            Siguiente
          </button>
        </div>
      )}

      {/* Modal: Schedule Match */}
      {showScheduleModal && (
        <div className="modal-overlay">
          <div className="modal-content fade-in">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>Programar Partido</h3>
              <button onClick={() => setShowScheduleModal(false)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleScheduleMatch} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8' }}>
                  Equipo Local *
                </label>
                <select
                  required
                  value={equipoLocalId}
                  onChange={(e) => setEquipoLocalId(e.target.value)}
                  style={{ width: '100%' }}
                >
                  {teams.map((t) => (
                    <option key={t.id} value={t.id}>
                      {t.nombre}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8' }}>
                  Equipo Visitante *
                </label>
                <select
                  required
                  value={equipoVisitanteId}
                  onChange={(e) => setEquipoVisitanteId(e.target.value)}
                  style={{ width: '100%' }}
                >
                  {teams.map((t) => (
                    <option key={t.id} value={t.id}>
                      {t.nombre}
                    </option>
                  ))}
                </select>
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 2fr', gap: '12px' }}>
                <div>
                  <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8' }}>
                    Jornada *
                  </label>
                  <input
                    type="number"
                    min="1"
                    required
                    value={jornada}
                    onChange={(e) => setJornada(parseInt(e.target.value))}
                    style={{ width: '100%' }}
                  />
                </div>

                <div>
                  <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8' }}>
                    Fecha y Hora *
                  </label>
                  <input
                    type="datetime-local"
                    required
                    value={fechaHora}
                    onChange={(e) => setFechaHora(e.target.value)}
                    style={{ width: '100%' }}
                  />
                </div>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '0.5rem' }}>
                <button
                  type="button"
                  onClick={() => setShowScheduleModal(false)}
                  style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{ padding: '8px 20px', backgroundColor: '#10b981', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
                >
                  {submitting ? 'Programando...' : 'Programar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal: View Match Detail (Goles) */}
      {selectedMatchDetail && (
        <div className="modal-overlay">
          <div className="modal-content fade-in">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>Detalle del Partido</h3>
              <button onClick={() => setSelectedMatchDetail(null)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <div style={{
              backgroundColor: '#1c2640',
              borderRadius: '10px',
              padding: '1rem',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              marginBottom: '1.25rem',
            }}>
              <span style={{ fontWeight: 700, color: '#f8fafc' }}>{selectedMatchDetail.equipoLocalNombre}</span>
              <span style={{ fontWeight: 800, fontSize: '1.4rem', color: '#34d399' }}>
                {selectedMatchDetail.golesLocal} - {selectedMatchDetail.golesVisitante}
              </span>
              <span style={{ fontWeight: 700, color: '#f8fafc' }}>{selectedMatchDetail.equipoVisitanteNombre}</span>
            </div>

            <h4 style={{ fontSize: '1rem', fontWeight: 600, color: '#94a3b8', marginBottom: '0.75rem' }}>
              Goles Anotados ({selectedMatchDetail.goles.length})
            </h4>

            {selectedMatchDetail.goles.length === 0 ? (
              <div style={{ textAlign: 'center', padding: '20px', color: '#64748b' }}>
                No se registraron goles individuales (o fue empate 0-0).
              </div>
            ) : (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                {selectedMatchDetail.goles.map((g) => (
                  <div
                    key={g.id}
                    style={{
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      padding: '8px 12px',
                      backgroundColor: '#131b2e',
                      borderRadius: '6px',
                      border: '1px solid #2a3756',
                      fontSize: '0.9rem',
                    }}
                  >
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                      <span>⚽</span>
                      <strong style={{ color: '#f8fafc' }}>{g.jugadorNombre}</strong>
                      <span style={{ color: '#64748b' }}>({g.equipoNombre})</span>
                      {g.esAutogol && <span className="badge badge-red">Autogol</span>}
                    </div>
                    {g.minuto && (
                      <span style={{ color: '#fbbf24', fontWeight: 600 }}>{g.minuto}'</span>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {/* Modal: Register Result */}
      {matchToRegister && (
        <div className="modal-overlay">
          <div className="modal-content fade-in" style={{ maxWidth: '620px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>Registrar Marcador</h3>
              <button onClick={() => setMatchToRegister(null)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSubmitResult} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>
              {/* Score Inputs */}
              <div style={{
                display: 'grid',
                gridTemplateColumns: '1fr auto 1fr',
                gap: '12px',
                alignItems: 'center',
                backgroundColor: '#1c2640',
                padding: '1.25rem',
                borderRadius: '10px',
              }}>
                <div style={{ textAlign: 'center' }}>
                  <label style={{ display: 'block', fontWeight: 600, color: '#f8fafc', marginBottom: '6px', fontSize: '0.9rem' }}>
                    {matchToRegister.equipoLocalNombre}
                  </label>
                  <input
                    type="number"
                    min="0"
                    required
                    value={golesLocal}
                    onChange={(e) => setGolesLocal(parseInt(e.target.value) || 0)}
                    style={{ width: '80px', textAlign: 'center', fontSize: '1.4rem', fontWeight: 700 }}
                  />
                </div>

                <div style={{ color: '#64748b', fontWeight: 800 }}>-</div>

                <div style={{ textAlign: 'center' }}>
                  <label style={{ display: 'block', fontWeight: 600, color: '#f8fafc', marginBottom: '6px', fontSize: '0.9rem' }}>
                    {matchToRegister.equipoVisitanteNombre}
                  </label>
                  <input
                    type="number"
                    min="0"
                    required
                    value={golesVisitante}
                    onChange={(e) => setGolesVisitante(parseInt(e.target.value) || 0)}
                    style={{ width: '80px', textAlign: 'center', fontSize: '1.4rem', fontWeight: 700 }}
                  />
                </div>
              </div>

              {/* Goal Details Section */}
              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <label style={{ fontSize: '0.9rem', fontWeight: 600, color: '#94a3b8' }}>
                    Detalle de Goles (Opcional - Total: {goalRows.length})
                  </label>
                  <button
                    type="button"
                    onClick={handleAddGoalRow}
                    style={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: '4px',
                      padding: '4px 10px',
                      backgroundColor: '#1e293b',
                      color: '#60a5fa',
                      borderRadius: '6px',
                      fontSize: '0.8rem',
                      fontWeight: 600,
                    }}
                  >
                    <Plus size={14} /> Añadir Gol
                  </button>
                </div>

                {goalRows.map((row, idx) => {
                  const currentTeam = row.equipoId === matchToRegister.equipoLocalId ? localSquad : visitorSquad;
                  return (
                    <div
                      key={idx}
                      style={{
                        display: 'grid',
                        gridTemplateColumns: '1.2fr 1.5fr 70px auto auto',
                        gap: '8px',
                        alignItems: 'center',
                        marginBottom: '8px',
                        padding: '8px',
                        backgroundColor: '#1c2640',
                        borderRadius: '6px',
                      }}
                    >
                      {/* Team select */}
                      <select
                        value={row.equipoId}
                        onChange={(e) => {
                          const newTeamId = e.target.value;
                          const squad = newTeamId === matchToRegister.equipoLocalId ? localSquad : visitorSquad;
                          const updated = [...goalRows];
                          updated[idx] = {
                            ...updated[idx],
                            equipoId: newTeamId,
                            jugadorId: squad?.jugadores[0]?.id || '',
                          };
                          setGoalRows(updated);
                        }}
                      >
                        <option value={matchToRegister.equipoLocalId}>{matchToRegister.equipoLocalNombre}</option>
                        <option value={matchToRegister.equipoVisitanteId}>{matchToRegister.equipoVisitanteNombre}</option>
                      </select>

                      {/* Player select */}
                      <select
                        value={row.jugadorId}
                        onChange={(e) => {
                          const updated = [...goalRows];
                          updated[idx] = { ...updated[idx], jugadorId: e.target.value };
                          setGoalRows(updated);
                        }}
                      >
                        {currentTeam?.jugadores.map((j) => (
                          <option key={j.id} value={j.id}>
                            #{j.dorsal} {j.nombre}
                          </option>
                        ))}
                      </select>

                      {/* Minute */}
                      <input
                        type="number"
                        min="1"
                        max="130"
                        placeholder="Min"
                        value={row.minuto ?? ''}
                        onChange={(e) => {
                          const updated = [...goalRows];
                          updated[idx] = { ...updated[idx], minuto: parseInt(e.target.value) || undefined };
                          setGoalRows(updated);
                        }}
                      />

                      {/* Autogol checkbox */}
                      <label style={{ display: 'flex', alignItems: 'center', gap: '4px', fontSize: '0.75rem', color: '#94a3b8' }}>
                        <input
                          type="checkbox"
                          checked={row.esAutogol}
                          onChange={(e) => {
                            const updated = [...goalRows];
                            updated[idx] = { ...updated[idx], esAutogol: e.target.checked };
                            setGoalRows(updated);
                          }}
                        />
                        Auto
                      </label>

                      {/* Remove */}
                      <button
                        type="button"
                        onClick={() => handleRemoveGoalRow(idx)}
                        style={{ background: 'transparent', color: '#f87171', padding: '4px' }}
                      >
                        <X size={16} />
                      </button>
                    </div>
                  );
                })}
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '0.5rem' }}>
                <button
                  type="button"
                  onClick={() => setMatchToRegister(null)}
                  style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{ padding: '8px 20px', backgroundColor: '#10b981', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
                >
                  {submitting ? 'Guardando...' : 'Finalizar Partido'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
