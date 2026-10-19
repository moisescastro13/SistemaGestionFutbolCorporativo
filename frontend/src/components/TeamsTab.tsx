import React, { useEffect, useState } from 'react';
import { Shield, Plus, Users, Search, X, Calendar, UserPlus, Edit2, Trash2 } from 'lucide-react';
import { api } from '../services/api';
import type { TeamSummary, TeamDetail, PagedResponse } from '../types';

interface TeamsTabProps {
  onNotify: (type: 'success' | 'error', message: string) => void;
}

export const TeamsTab: React.FC<TeamsTabProps> = ({ onNotify }) => {
  const [teamsData, setTeamsData] = useState<PagedResponse<TeamSummary> | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);

  // Modals state
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newTeamName, setNewTeamName] = useState('');
  const [newTeamLogo, setNewTeamLogo] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // Edit Team state
  const [teamToEdit, setTeamToEdit] = useState<TeamSummary | null>(null);
  const [editName, setEditName] = useState('');
  const [editLogo, setEditLogo] = useState('');

  // Delete confirmation
  const [teamToDelete, setTeamToDelete] = useState<TeamSummary | null>(null);

  // Team Detail Modal state
  const [selectedTeamDetail, setSelectedTeamDetail] = useState<TeamDetail | null>(null);

  // Quick Add Player Modal
  const [showAddPlayerModal, setShowAddPlayerModal] = useState(false);
  const [playerName, setPlayerName] = useState('');
  const [playerDorsal, setPlayerDorsal] = useState<number>(10);
  const [playerPosition, setPlayerPosition] = useState('Delantero');

  const fetchTeams = async () => {
    try {
      setLoading(true);
      const res = await api.getTeams({ pageNumber: page, pageSize: 8, nombre: searchTerm });
      setTeamsData(res);
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al cargar equipos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTeams();
  }, [page, searchTerm]);

  const handleCreateTeam = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newTeamName.trim()) {
      onNotify('error', 'El nombre del equipo es obligatorio.');
      return;
    }

    try {
      setSubmitting(true);
      await api.createTeam({
        nombre: newTeamName.trim(),
        logoUrl: newTeamLogo.trim() || undefined,
      });
      onNotify('success', `Equipo '${newTeamName}' creado exitosamente.`);
      setShowCreateModal(false);
      setNewTeamName('');
      setNewTeamLogo('');
      fetchTeams();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al registrar equipo');
    } finally {
      setSubmitting(false);
    }
  };

  const handleOpenEdit = (team: TeamSummary) => {
    setTeamToEdit(team);
    setEditName(team.nombre);
    setEditLogo(team.logoUrl || '');
  };

  const handleUpdateTeam = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!teamToEdit) return;
    if (!editName.trim()) {
      onNotify('error', 'El nombre del equipo es obligatorio.');
      return;
    }

    try {
      setSubmitting(true);
      await api.updateTeam(teamToEdit.id, {
        nombre: editName.trim(),
        logoUrl: editLogo.trim() || undefined,
      });
      onNotify('success', `Equipo '${editName}' actualizado.`);
      setTeamToEdit(null);
      fetchTeams();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al actualizar equipo');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteTeam = async () => {
    if (!teamToDelete) return;
    try {
      setSubmitting(true);
      await api.deleteTeam(teamToDelete.id);
      onNotify('success', `Equipo '${teamToDelete.nombre}' eliminado.`);
      setTeamToDelete(null);
      fetchTeams();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al eliminar equipo');
    } finally {
      setSubmitting(false);
    }
  };

  const handleOpenTeamDetail = async (id: string) => {
    try {
      const detail = await api.getTeamById(id);
      setSelectedTeamDetail(detail);
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al obtener detalle del equipo');
    }
  };

  const handleQuickAddPlayer = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedTeamDetail) return;
    if (!playerName.trim()) {
      onNotify('error', 'El nombre del jugador es obligatorio.');
      return;
    }

    try {
      setSubmitting(true);
      await api.createPlayer(selectedTeamDetail.id, {
        nombre: playerName.trim(),
        dorsal: Number(playerDorsal),
        posicion: playerPosition,
      });
      onNotify('success', `Jugador '${playerName}' agregado a ${selectedTeamDetail.nombre}.`);
      setShowAddPlayerModal(false);
      setPlayerName('');
      handleOpenTeamDetail(selectedTeamDetail.id);
      fetchTeams();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al registrar jugador');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fade-in" style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
      {/* Top action bar */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', flex: '1', maxWidth: '360px' }}>
          <div style={{ position: 'relative', width: '100%' }}>
            <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: '#64748b' }} />
            <input
              type="text"
              placeholder="Buscar equipo por nombre..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setPage(1);
              }}
              style={{ width: '100%', paddingLeft: '38px' }}
            />
          </div>
        </div>

        <button
          onClick={() => setShowCreateModal(true)}
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
          Nuevo Equipo
        </button>
      </div>

      {/* Grid of Teams */}
      {loading && !teamsData ? (
        <div style={{ textAlign: 'center', padding: '50px', color: '#64748b' }}>Cargando equipos...</div>
      ) : teamsData?.data.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '50px', color: '#64748b' }}>No se encontraron equipos.</div>
      ) : (
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
          gap: '1.25rem',
        }}>
          {teamsData?.data.map((team) => (
            <div
              key={team.id}
              style={{
                backgroundColor: '#131b2e',
                border: '1px solid #2a3756',
                borderRadius: '12px',
                padding: '1.25rem',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between',
                gap: '1rem',
                boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
                transition: 'transform 0.15s, border-color 0.15s',
              }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
                {team.logoUrl ? (
                  <img
                    src={team.logoUrl}
                    alt={team.nombre}
                    style={{
                      width: '48px',
                      height: '48px',
                      borderRadius: '50%',
                      objectFit: 'contain',
                      backgroundColor: '#fff',
                      padding: '3px',
                      border: '1px solid #334155',
                    }}
                    onError={(e) => {
                      (e.target as HTMLElement).style.display = 'none';
                    }}
                  />
                ) : (
                  <div style={{
                    width: '48px',
                    height: '48px',
                    borderRadius: '50%',
                    backgroundColor: '#1e293b',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    color: '#94a3b8',
                  }}>
                    <Shield size={24} />
                  </div>
                )}
                <div style={{ flex: 1, minWidth: 0 }}>
                  <h4 style={{ fontSize: '1.1rem', fontWeight: 700, color: '#f8fafc', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    {team.nombre}
                  </h4>
                  <span className="badge badge-blue" style={{ marginTop: '4px' }}>
                    <Users size={12} /> {team.totalJugadores} Jugadores
                  </span>
                </div>
              </div>

              <div style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                paddingTop: '0.75rem',
                borderTop: '1px solid #1e293b',
                fontSize: '0.8rem',
                color: '#64748b',
              }}>
                <span style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                  <Calendar size={13} /> {new Date(team.fechaCreacion).toLocaleDateString()}
                </span>

                <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                  <button
                    onClick={() => handleOpenTeamDetail(team.id)}
                    title="Ver Plantilla"
                    style={{
                      backgroundColor: '#1e293b',
                      color: '#60a5fa',
                      padding: '6px 10px',
                      borderRadius: '6px',
                      fontWeight: 600,
                      border: '1px solid #334155',
                      fontSize: '0.8rem',
                    }}
                  >
                    Plantilla
                  </button>

                  <button
                    onClick={() => handleOpenEdit(team)}
                    title="Editar Equipo"
                    style={{
                      backgroundColor: '#1e293b',
                      color: '#fbbf24',
                      padding: '6px 8px',
                      borderRadius: '6px',
                      border: '1px solid #334155',
                      display: 'flex',
                      alignItems: 'center',
                    }}
                  >
                    <Edit2 size={14} />
                  </button>

                  <button
                    onClick={() => setTeamToDelete(team)}
                    title="Eliminar Equipo"
                    style={{
                      backgroundColor: '#1e293b',
                      color: '#f87171',
                      padding: '6px 8px',
                      borderRadius: '6px',
                      border: '1px solid #334155',
                      display: 'flex',
                      alignItems: 'center',
                    }}
                  >
                    <Trash2 size={14} />
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination controls */}
      {teamsData && teamsData.totalPages > 1 && (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '1rem', marginTop: '1rem' }}>
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
            Página {teamsData.pageNumber} de {teamsData.totalPages} ({teamsData.totalRecords} equipos)
          </span>
          <button
            onClick={() => setPage((p) => Math.min(teamsData.totalPages, p + 1))}
            disabled={page === teamsData.totalPages}
            style={{
              padding: '6px 14px',
              backgroundColor: '#1e293b',
              color: page === teamsData.totalPages ? '#475569' : '#f8fafc',
              borderRadius: '6px',
              border: '1px solid #334155',
            }}
          >
            Siguiente
          </button>
        </div>
      )}

      {/* Modal: Create Team */}
      {showCreateModal && (
        <div className="modal-overlay">
          <div className="modal-content fade-in">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>Registrar Nuevo Equipo</h3>
              <button onClick={() => setShowCreateModal(false)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleCreateTeam} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  Nombre del Equipo *
                </label>
                <input
                  type="text"
                  required
                  placeholder="Ej. Deportivo San Salvador"
                  value={newTeamName}
                  onChange={(e) => setNewTeamName(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  URL del Logo (Opcional)
                </label>
                <input
                  type="url"
                  placeholder="https://ejemplo.com/logo.png"
                  value={newTeamLogo}
                  onChange={(e) => setNewTeamLogo(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '0.5rem' }}>
                <button
                  type="button"
                  onClick={() => setShowCreateModal(false)}
                  style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{ padding: '8px 20px', backgroundColor: '#10b981', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
                >
                  {submitting ? 'Guardando...' : 'Crear Equipo'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal: Edit Team */}
      {teamToEdit && (
        <div className="modal-overlay">
          <div className="modal-content fade-in">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>Editar Equipo</h3>
              <button onClick={() => setTeamToEdit(null)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleUpdateTeam} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  Nombre del Equipo *
                </label>
                <input
                  type="text"
                  required
                  value={editName}
                  onChange={(e) => setEditName(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  URL del Logo
                </label>
                <input
                  type="url"
                  value={editLogo}
                  onChange={(e) => setEditLogo(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '0.5rem' }}>
                <button
                  type="button"
                  onClick={() => setTeamToEdit(null)}
                  style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{ padding: '8px 20px', backgroundColor: '#3b82f6', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
                >
                  {submitting ? 'Guardando...' : 'Guardar Cambios'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal: Confirm Delete Team */}
      {teamToDelete && (
        <div className="modal-overlay">
          <div className="modal-content fade-in" style={{ maxWidth: '420px' }}>
            <h3 style={{ fontSize: '1.2rem', fontWeight: 700, color: '#f87171', marginBottom: '0.75rem' }}>
              ¿Eliminar Equipo?
            </h3>
            <p style={{ color: '#94a3b8', fontSize: '0.9rem', marginBottom: '1.25rem' }}>
              ¿Estás seguro de que deseas eliminar a <strong>{teamToDelete.nombre}</strong>? Esta acción no se puede deshacer.
              (Si el equipo tiene jugadores o partidos asociados, la regla de negocio impedirá su eliminación).
            </p>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
              <button
                type="button"
                onClick={() => setTeamToDelete(null)}
                style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
              >
                Cancelar
              </button>
              <button
                type="button"
                disabled={submitting}
                onClick={handleDeleteTeam}
                style={{ padding: '8px 18px', backgroundColor: '#ef4444', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
              >
                {submitting ? 'Eliminando...' : 'Eliminar'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Modal: Team Detail & Roster */}
      {selectedTeamDetail && (
        <div className="modal-overlay">
          <div className="modal-content fade-in" style={{ maxWidth: '640px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                {selectedTeamDetail.logoUrl ? (
                  <img
                    src={selectedTeamDetail.logoUrl}
                    alt={selectedTeamDetail.nombre}
                    style={{ width: '40px', height: '40px', borderRadius: '50%', objectFit: 'contain', backgroundColor: '#fff', padding: '2px' }}
                  />
                ) : (
                  <Shield size={28} color="#60a5fa" />
                )}
                <div>
                  <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>
                    {selectedTeamDetail.nombre}
                  </h3>
                  <span style={{ fontSize: '0.8rem', color: '#94a3b8' }}>
                    Plantilla oficial ({selectedTeamDetail.jugadores.length} inscritos)
                  </span>
                </div>
              </div>

              <button onClick={() => setSelectedTeamDetail(null)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '1rem' }}>
              <button
                onClick={() => setShowAddPlayerModal(true)}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '6px',
                  backgroundColor: '#3b82f6',
                  color: '#fff',
                  padding: '6px 12px',
                  borderRadius: '6px',
                  fontSize: '0.85rem',
                  fontWeight: 600,
                }}
              >
                <UserPlus size={16} />
                Inscribir Jugador
              </button>
            </div>

            <div style={{ maxHeight: '350px', overflowY: 'auto', border: '1px solid #2a3756', borderRadius: '8px' }}>
              {selectedTeamDetail.jugadores.length === 0 ? (
                <div style={{ padding: '30px', textAlign: 'center', color: '#64748b' }}>
                  Este equipo aún no tiene jugadores inscritos.
                </div>
              ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.9rem' }}>
                  <thead>
                    <tr style={{ backgroundColor: '#1c2640', color: '#94a3b8', borderBottom: '1px solid #2a3756' }}>
                      <th style={{ padding: '8px 12px', textAlign: 'center', width: '50px' }}>#</th>
                      <th style={{ padding: '8px 12px', textAlign: 'left' }}>Jugador</th>
                      <th style={{ padding: '8px 12px', textAlign: 'left' }}>Posición</th>
                    </tr>
                  </thead>
                  <tbody>
                    {selectedTeamDetail.jugadores.map((j) => (
                      <tr key={j.id} style={{ borderBottom: '1px solid #1e293b' }}>
                        <td style={{ padding: '8px 12px', textAlign: 'center', fontWeight: 700, color: '#fbbf24' }}>
                          {j.dorsal}
                        </td>
                        <td style={{ padding: '8px 12px', color: '#f8fafc', fontWeight: 500 }}>
                          {j.nombre}
                        </td>
                        <td style={{ padding: '8px 12px' }}>
                          <span className="badge badge-blue">{j.posicion}</span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Modal: Quick Add Player inside Team */}
      {showAddPlayerModal && selectedTeamDetail && (
        <div className="modal-overlay" style={{ zIndex: 1100 }}>
          <div className="modal-content fade-in" style={{ maxWidth: '440px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
              <h4 style={{ fontSize: '1.15rem', fontWeight: 700, color: '#f8fafc' }}>
                Inscribir Jugador en {selectedTeamDetail.nombre}
              </h4>
              <button onClick={() => setShowAddPlayerModal(false)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={18} />
              </button>
            </div>

            <form onSubmit={handleQuickAddPlayer} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '4px', fontSize: '0.85rem', color: '#94a3b8' }}>
                  Nombre Completo *
                </label>
                <input
                  type="text"
                  required
                  placeholder="Ej. Roberto Morales"
                  value={playerName}
                  onChange={(e) => setPlayerName(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '10px' }}>
                <div>
                  <label style={{ display: 'block', marginBottom: '4px', fontSize: '0.85rem', color: '#94a3b8' }}>
                    Dorsal (1 - 99) *
                  </label>
                  <input
                    type="number"
                    min="1"
                    max="99"
                    required
                    value={playerDorsal}
                    onChange={(e) => setPlayerDorsal(parseInt(e.target.value))}
                    style={{ width: '100%' }}
                  />
                </div>

                <div>
                  <label style={{ display: 'block', marginBottom: '4px', fontSize: '0.85rem', color: '#94a3b8' }}>
                    Posición *
                  </label>
                  <select
                    value={playerPosition}
                    onChange={(e) => setPlayerPosition(e.target.value)}
                    style={{ width: '100%' }}
                  >
                    <option value="Portero">Portero</option>
                    <option value="Defensa">Defensa</option>
                    <option value="Medio">Medio</option>
                    <option value="Mediocampista">Mediocampista</option>
                    <option value="Delantero">Delantero</option>
                  </select>
                </div>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '0.5rem' }}>
                <button
                  type="button"
                  onClick={() => setShowAddPlayerModal(false)}
                  style={{ padding: '7px 14px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{ padding: '7px 18px', backgroundColor: '#3b82f6', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
                >
                  {submitting ? 'Inscribiendo...' : 'Inscribir'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
