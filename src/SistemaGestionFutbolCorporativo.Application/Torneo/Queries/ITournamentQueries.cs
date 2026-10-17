namespace SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

public sealed record TournamentStandingDto(
    int Posicion,
    Guid EquipoId,
    string EquipoNombre,
    string? LogoUrl,
    int PartidosJugados,
    int PartidosGanados,
    int PartidosEmpatados,
    int PartidosPerdidos,
    int GolesAFavor,
    int GolesEnContra,
    int DiferenciaGoles,
    int Puntos);

public sealed record TopScorerDto(
    int Posicion,
    Guid JugadorId,
    string JugadorNombre,
    Guid EquipoId,
    string EquipoNombre,
    string PosicionJugador,
    int Goles);

public interface ITournamentQueries
{
    Task<IReadOnlyList<TournamentStandingDto>> GetStandingsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TopScorerDto>> GetTopScorersAsync(int? limit = null, CancellationToken cancellationToken = default);
}
