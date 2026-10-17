namespace SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

public sealed record TournamentStandingDto(
    int Posicion,
    Guid EquipoId,
    string EquipoNombre,
    string? LogoUrl,
    int PJ,
    int PG,
    int PE,
    int PP,
    int GF,
    int GC,
    int DG,
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
