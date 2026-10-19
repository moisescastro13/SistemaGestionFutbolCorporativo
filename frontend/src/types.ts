export interface TournamentStanding {
  posicion: number;
  equipoId: string;
  equipoNombre: string;
  logoUrl?: string;
  partidosJugados: number;
  partidosGanados: number;
  partidosEmpatados: number;
  partidosPerdidos: number;
  golesAFavor: number;
  golesEnContra: number;
  diferenciaGoles: number;
  puntos: number;
}

export interface TopScorer {
  posicion: number;
  jugadorId: string;
  jugadorNombre: string;
  equipoId: string;
  equipoNombre: string;
  posicionJugador: string;
  goles: number;
}

export interface TeamSummary {
  id: string;
  nombre: string;
  logoUrl?: string;
  fechaCreacion: string;
  totalJugadores: number;
}

export interface PlayerDto {
  id: string;
  nombre: string;
  dorsal: number;
  posicion: string;
}

export interface TeamDetail {
  id: string;
  nombre: string;
  logoUrl?: string;
  fechaCreacion: string;
  jugadores: PlayerDto[];
}

export interface PlayerSummary {
  id: string;
  equipoId: string;
  equipoNombre: string;
  nombre: string;
  dorsal: number;
  posicion: string;
  totalGoles: number;
}

export interface GoalDetail {
  id: string;
  jugadorId: string;
  jugadorNombre: string;
  equipoId: string;
  equipoNombre: string;
  minuto?: number;
  esAutogol: boolean;
}

export interface MatchSummary {
  id: string;
  jornada: number;
  fechaHora: string;
  estado: string;
  equipoLocalId: string;
  equipoLocalNombre: string;
  equipoLocalLogoUrl?: string;
  equipoVisitanteId: string;
  equipoVisitanteNombre: string;
  equipoVisitanteLogoUrl?: string;
  golesLocal?: number;
  golesVisitante?: number;
}

export interface MatchDetail extends MatchSummary {
  goles: GoalDetail[];
}

export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  errorCode?: string;
  errors?: Record<string, string[]>;
}
