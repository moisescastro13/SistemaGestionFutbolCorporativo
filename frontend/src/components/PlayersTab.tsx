import React, { useEffect, useState } from 'react';
import { UserPlus, Filter, Shield, Edit2, Trash2, X } from 'lucide-react';
import { api } from '../services/api';
import type { PlayerSummary, TeamSummary, PagedResponse } from '../types';

interface PlayersTabProps {
  onNotify: (type: 'success' | 'error', message: string) => void;
}

export const PlayersTab: React.FC<PlayersTabProps> = ({ onNotify }) => {
  const [playersData, setPlayersData] = useState<PagedResponse<PlayerSummary> | null>(null);
  const [teams, setTeams] = useState<TeamSummary[]>([]);
  const [selectedEquipoId, setSelectedEquipoId] = useState<string>('');
  const [selectedPosicion, setSelectedPosicion] = useState<string>('');
  const [page, setPage] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);

  // Modal Create State
  const [showModal, setShowModal] = useState<boolean>(false);
  const [newTeamId, setNewTeamId] = useState<string>('');
  const [newPlayerName, setNewPlayerName] = useState<string>('');
  const [newDorsal, setNewDorsal] = useState<number>(10);
  const [newPosicion, setNewPosicion] = useState<string>('Delantero');
  const [submitting, setSubmitting] = useState<boolean>(false);

  // Modal Edit State
  const [playerToEdit, setPlayerToEdit] = useState<PlayerSummary | null>(null);
  const [editPlayerName, setEditPlayerName] = useState<string>('');
  const [editPlayerDorsal, setEditPlayerDorsal] = useState<number>(10);
  const [editPlayerPosicion, setEditPlayerPosicion] = useState<string>('Delantero');

  // Modal Delete State
  const [playerToDelete, setPlayerToDelete] = useState<PlayerSummary | null>(null);

  // Load teams for dropdown filter and creation
  useEffect(() => {
    const loadTeams = async () => {
      try {
        const res = await api.getTeams({ pageSize: 100 });
        setTeams(res.data);
        if (res.data.length > 0 && !newTeamId) {
          setNewTeamId(res.data[0].id);
        }
      } catch {
        // Ignored
      }
    };
    loadTeams();
  }, []);

  const fetchPlayers = async () => {
    try {
      setLoading(true);
      const res = await api.getPlayers({
        pageNumber: page,
        pageSize: 10,
        equipoId: selectedEquipoId || undefined,
        posicion: selectedPosicion || undefined,
      });
      setPlayersData(res);
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al cargar jugadores');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlayers();
  }, [page, selectedEquipoId, selectedPosicion]);

  const handleCreatePlayer = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newTeamId) {
      onNotify('error', 'Debes seleccionar un equipo.');
      return;
    }
    if (!newPlayerName.trim()) {
      onNotify('error', 'El nombre del jugador es requerido.');
      return;
    }

    try {
      setSubmitting(true);
      await api.createPlayer(newTeamId, {
        nombre: newPlayerName.trim(),
        dorsal: Number(newDorsal),
        posicion: newPosicion,
      });
      onNotify('success', `Jugador '${newPlayerName}' registrado exitosamente.`);
      setShowModal(false);
      setNewPlayerName('');
      fetchPlayers();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al registrar jugador');
    } finally {
      setSubmitting(false);
    }
  };

  const handleOpenEdit = (player: PlayerSummary) => {
    setPlayerToEdit(player);
    setEditPlayerName(player.nombre);
    setEditPlayerDorsal(player.dorsal);
    setEditPlayerPosicion(player.posicion);
  };

  const handleUpdatePlayer = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!playerToEdit) return;
    if (!editPlayerName.trim()) {
      onNotify('error', 'El nombre es obligatorio.');
      return;
    }

    try {
      setSubmitting(true);
      await api.updatePlayer(playerToEdit.id, {
        nombre: editPlayerName.trim(),
        dorsal: Number(editPlayerDorsal),
        posicion: editPlayerPosicion,
      });
      onNotify('success', `Jugador '${editPlayerName}' actualizado.`);
      setPlayerToEdit(null);
      fetchPlayers();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al actualizar jugador');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeletePlayer = async () => {
    if (!playerToDelete) return;

    try {
      setSubmitting(true);
      await api.deletePlayer(playerToDelete.id);
      onNotify('success', `Jugador '${playerToDelete.nombre}' eliminado.`);
      setPlayerToDelete(null);
      fetchPlayers();
    } catch (err) {
      onNotify('error', err instanceof Error ? err.message : 'Error al eliminar jugador');
    } finally {
      setSubmitting(false);
    }
  };

  const getPositionBadgeClass = (pos: string) => {
    switch (pos.toLowerCase()) {
      case 'portero':
        return 'badge-warning';
      case 'defensa':
        return 'badge-blue';
      case 'medio':
      case 'mediocampista':
        return 'badge-purple';
      case 'delantero':
        return 'badge-red';
      default:
        return 'badge-blue';
    }
  };

  return (
    <div className="fade-in" style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
      {/* Filters and Action Bar */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', flexWrap: 'wrap' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: '#94a3b8', fontSize: '0.9rem' }}>
            <Filter size={16} /> Filtros:
          </div>

          <select
            value={selectedEquipoId}
            onChange={(e) => {
              setSelectedEquipoId(e.target.value);
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

          <select
            value={selectedPosicion}
            onChange={(e) => {
              setSelectedPosicion(e.target.value);
              setPage(1);
            }}
          >
            <option value="">Todas las Posiciones</option>
            <option value="Portero">Portero</option>
            <option value="Defensa">Defensa</option>
            <option value="Medio">Medio</option>
            <option value="Mediocampista">Mediocampista</option>
            <option value="Delantero">Delantero</option>
          </select>
        </div>

        <button
          onClick={() => setShowModal(true)}
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
          <UserPlus size={18} />
          Nuevo Jugador
        </button>
      </div>

      {/* Players Table */}
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
              <th style={{ padding: '14px 16px', width: '70px', textAlign: 'center' }}>Dorsal</th>
              <th style={{ padding: '14px 16px' }}>Nombre</th>
              <th style={{ padding: '14px 16px' }}>Equipo</th>
              <th style={{ padding: '14px 16px' }}>Posición</th>
              <th style={{ padding: '14px 16px', textAlign: 'center' }}>Goles</th>
              <th style={{ padding: '14px 16px', textAlign: 'center', width: '100px' }}>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {loading && !playersData ? (
              <tr>
                <td colSpan={6} style={{ padding: '40px', textAlign: 'center', color: '#64748b' }}>
                  Cargando jugadores...
                </td>
              </tr>
            ) : playersData?.data.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ padding: '40px', textAlign: 'center', color: '#64748b' }}>
                  No se encontraron jugadores con los filtros seleccionados.
                </td>
              </tr>
            ) : (
              playersData?.data.map((player) => (
                <tr key={player.id} style={{ borderBottom: '1px solid #1e293b' }}>
                  <td style={{ padding: '12px 16px', textAlign: 'center' }}>
                    <span style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      width: '32px',
                      height: '32px',
                      borderRadius: '8px',
                      backgroundColor: '#1e293b',
                      color: '#fbbf24',
                      fontWeight: 700,
                      fontSize: '0.95rem',
                      border: '1px solid #334155',
                    }}>
                      {player.dorsal}
                    </span>
                  </td>
                  <td style={{ padding: '12px 16px', fontWeight: 600, color: '#f8fafc' }}>
                    {player.nombre}
                  </td>
                  <td style={{ padding: '12px 16px', color: '#94a3b8' }}>
                    <span style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                      <Shield size={14} color="#60a5fa" />
                      {player.equipoNombre}
                    </span>
                  </td>
                  <td style={{ padding: '12px 16px' }}>
                    <span className={`badge ${getPositionBadgeClass(player.posicion)}`}>
                      {player.posicion}
                    </span>
                  </td>
                  <td style={{ padding: '12px 16px', textAlign: 'center', fontWeight: 600, color: player.totalGoles > 0 ? '#34d399' : '#64748b' }}>
                    {player.totalGoles > 0 ? `⚽ ${player.totalGoles}` : '0'}
                  </td>
                  <td style={{ padding: '12px 16px', textAlign: 'center' }}>
                    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '8px' }}>
                      <button
                        onClick={() => handleOpenEdit(player)}
                        title="Editar Jugador"
                        style={{
                          backgroundColor: '#1e293b',
                          color: '#fbbf24',
                          padding: '6px',
                          borderRadius: '6px',
                          border: '1px solid #334155',
                          display: 'flex',
                          alignItems: 'center',
                        }}
                      >
                        <Edit2 size={14} />
                      </button>

                      <button
                        onClick={() => setPlayerToDelete(player)}
                        title="Eliminar Jugador"
                        style={{
                          backgroundColor: '#1e293b',
                          color: '#f87171',
                          padding: '6px',
                          borderRadius: '6px',
                          border: '1px solid #334155',
                          display: 'flex',
                          alignItems: 'center',
                        }}
                      >
                        <Trash2 size={14} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination controls */}
      {playersData && playersData.totalPages > 1 && (
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
            Página {playersData.pageNumber} de {playersData.totalPages} ({playersData.totalRecords} jugadores)
          </span>
          <button
            onClick={() => setPage((p) => Math.min(playersData.totalPages, p + 1))}
            disabled={page === playersData.totalPages}
            style={{
              padding: '6px 14px',
              backgroundColor: '#1e293b',
              color: page === playersData.totalPages ? '#475569' : '#f8fafc',
              borderRadius: '6px',
              border: '1px solid #334155',
            }}
          >
            Siguiente
          </button>
        </div>
      )}

      {/* Modal: Create Player */}
      {showModal && (
        <div className="modal-overlay">
          <div className="modal-content fade-in">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>
                Registrar Nuevo Jugador
              </h3>
              <button onClick={() => setShowModal(false)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleCreatePlayer} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  Equipo *
                </label>
                <select
                  required
                  value={newTeamId}
                  onChange={(e) => setNewTeamId(e.target.value)}
                  style={{ width: '100%' }}
                >
                  <option value="">Selecciona un equipo...</option>
                  {teams.map((t) => (
                    <option key={t.id} value={t.id}>
                      {t.nombre}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  Nombre del Jugador *
                </label>
                <input
                  type="text"
                  required
                  placeholder="Ej. Andrés Guardado"
                  value={newPlayerName}
                  onChange={(e) => setNewPlayerName(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
                <div>
                  <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                    Dorsal (1 al 99) *
                  </label>
                  <input
                    type="number"
                    min="1"
                    max="99"
                    required
                    value={newDorsal}
                    onChange={(e) => setNewDorsal(parseInt(e.target.value))}
                    style={{ width: '100%' }}
                  />
                </div>

                <div>
                  <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                    Posición *
                  </label>
                  <select
                    value={newPosicion}
                    onChange={(e) => setNewPosicion(e.target.value)}
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
                  onClick={() => setShowModal(false)}
                  style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{ padding: '8px 20px', backgroundColor: '#10b981', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
                >
                  {submitting ? 'Guardando...' : 'Guardar Jugador'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal: Edit Player */}
      {playerToEdit && (
        <div className="modal-overlay">
          <div className="modal-content fade-in">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f8fafc' }}>
                Editar Jugador ({playerToEdit.equipoNombre})
              </h3>
              <button onClick={() => setPlayerToEdit(null)} style={{ background: 'transparent', color: '#94a3b8' }}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleUpdatePlayer} style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                  Nombre del Jugador *
                </label>
                <input
                  type="text"
                  required
                  value={editPlayerName}
                  onChange={(e) => setEditPlayerName(e.target.value)}
                  style={{ width: '100%' }}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
                <div>
                  <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                    Dorsal (1 al 99) *
                  </label>
                  <input
                    type="number"
                    min="1"
                    max="99"
                    required
                    value={editPlayerDorsal}
                    onChange={(e) => setEditPlayerDorsal(parseInt(e.target.value))}
                    style={{ width: '100%' }}
                  />
                </div>

                <div>
                  <label style={{ display: 'block', marginBottom: '6px', fontSize: '0.85rem', color: '#94a3b8', fontWeight: 500 }}>
                    Posición *
                  </label>
                  <select
                    value={editPlayerPosicion}
                    onChange={(e) => setEditPlayerPosicion(e.target.value)}
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
                  onClick={() => setPlayerToEdit(null)}
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

      {/* Modal: Confirm Delete Player */}
      {playerToDelete && (
        <div className="modal-overlay">
          <div className="modal-content fade-in" style={{ maxWidth: '420px' }}>
            <h3 style={{ fontSize: '1.2rem', fontWeight: 700, color: '#f87171', marginBottom: '0.75rem' }}>
              ¿Eliminar Jugador?
            </h3>
            <p style={{ color: '#94a3b8', fontSize: '0.9rem', marginBottom: '1.25rem' }}>
              ¿Estás seguro de eliminar a <strong>{playerToDelete.nombre}</strong> (#{playerToDelete.dorsal} - {playerToDelete.equipoNombre})?
              (Si el jugador tiene goles registrados en el torneo, la regla de negocio impedirá su eliminación).
            </p>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
              <button
                type="button"
                onClick={() => setPlayerToDelete(null)}
                style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: '#94a3b8', borderRadius: '6px' }}
              >
                Cancelar
              </button>
              <button
                type="button"
                disabled={submitting}
                onClick={handleDeletePlayer}
                style={{ padding: '8px 18px', backgroundColor: '#ef4444', color: '#fff', borderRadius: '6px', fontWeight: 600 }}
              >
                {submitting ? 'Eliminando...' : 'Eliminar'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
