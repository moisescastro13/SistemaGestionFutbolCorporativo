import type {
  TournamentStanding,
  TopScorer,
  TeamSummary,
  TeamDetail,
  PlayerSummary,
  MatchSummary,
  MatchDetail,
  PagedResponse,
  ProblemDetails,
} from '../types';

const API_BASE = import.meta.env.VITE_API_BASE || '/api';

function generateUUID(): string {
  if (typeof crypto !== 'undefined' && crypto.randomUUID) {
    return crypto.randomUUID();
  }
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const url = `${API_BASE}${path}`;
  const method = (options.method || 'GET').toUpperCase();
  const headers = new Headers(options.headers || {});

  headers.set('Accept', 'application/json');
  if (!headers.has('X-Correlation-Id')) {
    headers.set('X-Correlation-Id', generateUUID());
  }

  // Idempotency-Key for mutating HTTP methods
  if (['POST', 'PUT', 'PATCH', 'DELETE'].includes(method)) {
    if (!headers.has('Idempotency-Key')) {
      headers.set('Idempotency-Key', generateUUID());
    }
    if (options.body && !headers.has('Content-Type')) {
      headers.set('Content-Type', 'application/json');
    }
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let errorDetail = `Error ${response.status}: ${response.statusText}`;
    try {
      const errorJson = (await response.json()) as ProblemDetails;
      errorDetail = errorJson.detail || errorJson.title || errorDetail;
    } catch {
      // Fallback to status text
    }
    throw new Error(errorDetail);
  }

  if (response.status === 204) {
    return {} as T;
  }

  const text = await response.text();
  if (!text) {
    return {} as T;
  }

  try {
    return JSON.parse(text) as T;
  } catch {
    return text as unknown as T;
  }
}

export const api = {
  // Torneo
  async getStandings(): Promise<TournamentStanding[]> {
    return request<TournamentStanding[]>('/torneo/posiciones');
  },

  async getTopScorers(top?: number): Promise<TopScorer[]> {
    const query = top ? `?top=${top}` : '';
    return request<TopScorer[]>(`/torneo/goleadores${query}`);
  },

  // Equipos
  async getTeams(params: {
    pageNumber?: number;
    pageSize?: number;
    nombre?: string;
  } = {}): Promise<PagedResponse<TeamSummary>> {
    const searchParams = new URLSearchParams();
    if (params.pageNumber) searchParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize) searchParams.set('pageSize', params.pageSize.toString());
    if (params.nombre) searchParams.set('nombre', params.nombre);
    const qs = searchParams.toString();
    return request<PagedResponse<TeamSummary>>(`/equipos${qs ? `?${qs}` : ''}`);
  },

  async getTeamById(id: string): Promise<TeamDetail> {
    return request<TeamDetail>(`/equipos/${id}`);
  },

  async createTeam(data: { nombre: string; logoUrl?: string }): Promise<string> {
    return request<string>('/equipos', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  async updateTeam(id: string, data: { nombre: string; logoUrl?: string }): Promise<void> {
    return request<void>(`/equipos/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  async deleteTeam(id: string): Promise<void> {
    return request<void>(`/equipos/${id}`, {
      method: 'DELETE',
    });
  },

  // Jugadores
  async getPlayers(params: {
    pageNumber?: number;
    pageSize?: number;
    equipoId?: string;
    posicion?: string;
  } = {}): Promise<PagedResponse<PlayerSummary>> {
    const searchParams = new URLSearchParams();
    if (params.pageNumber) searchParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize) searchParams.set('pageSize', params.pageSize.toString());
    if (params.equipoId) searchParams.set('equipoId', params.equipoId);
    if (params.posicion) searchParams.set('posicion', params.posicion);
    const qs = searchParams.toString();
    return request<PagedResponse<PlayerSummary>>(`/jugadores${qs ? `?${qs}` : ''}`);
  },

  async createPlayer(
    equipoId: string,
    data: { nombre: string; dorsal: number; posicion: string }
  ): Promise<string> {
    return request<string>(`/equipos/${equipoId}/jugadores`, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  async updatePlayer(
    id: string,
    data: { nombre: string; dorsal: number; posicion: string }
  ): Promise<void> {
    return request<void>(`/jugadores/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  async deletePlayer(id: string): Promise<void> {
    return request<void>(`/jugadores/${id}`, {
      method: 'DELETE',
    });
  },

  // Partidos
  async getMatches(params: {
    pageNumber?: number;
    pageSize?: number;
    jornada?: number;
    equipoId?: string;
    estado?: string;
  } = {}): Promise<PagedResponse<MatchSummary>> {
    const searchParams = new URLSearchParams();
    if (params.pageNumber) searchParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize) searchParams.set('pageSize', params.pageSize.toString());
    if (params.jornada) searchParams.set('jornada', params.jornada.toString());
    if (params.equipoId) searchParams.set('equipoId', params.equipoId);
    if (params.estado) searchParams.set('estado', params.estado);
    const qs = searchParams.toString();
    return request<PagedResponse<MatchSummary>>(`/partidos${qs ? `?${qs}` : ''}`);
  },

  async getMatchById(id: string): Promise<MatchDetail> {
    return request<MatchDetail>(`/partidos/${id}`);
  },

  async scheduleMatch(data: {
    equipoLocalId: string;
    equipoVisitanteId: string;
    jornada: number;
    fechaHora: string;
  }): Promise<string> {
    return request<string>('/partidos', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  async registerMatchResult(
    id: string,
    data: {
      golesLocal: number;
      golesVisitante: number;
      goles?: Array<{
        jugadorId: string;
        equipoId: string;
        minuto?: number;
        esAutogol?: boolean;
      }>;
    }
  ): Promise<void> {
    return request<void>(`/partidos/${id}/resultado`, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },
};
