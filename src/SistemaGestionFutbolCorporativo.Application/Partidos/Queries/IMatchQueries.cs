using SistemaGestionFutbolCorporativo.Application.Common;

namespace SistemaGestionFutbolCorporativo.Application.Partidos.Queries;

public sealed record GoalDetailDto(
    Guid Id,
    Guid JugadorId,
    string JugadorNombre,
    Guid EquipoId,
    string EquipoNombre,
    int? Minuto,
    bool EsAutogol);

public sealed record MatchDetailResponse(
    Guid Id,
    int Jornada,
    DateTime FechaHora,
    string Estado,
    Guid EquipoLocalId,
    string EquipoLocalNombre,
    string? EquipoLocalLogoUrl,
    Guid EquipoVisitanteId,
    string EquipoVisitanteNombre,
    string? EquipoVisitanteLogoUrl,
    int? GolesLocal,
    int? GolesVisitante,
    List<GoalDetailDto> Goles);

public sealed record MatchSummaryDto(
    Guid Id,
    int Jornada,
    DateTime FechaHora,
    string Estado,
    Guid EquipoLocalId,
    string EquipoLocalNombre,
    string? EquipoLocalLogoUrl,
    Guid EquipoVisitanteId,
    string EquipoVisitanteNombre,
    string? EquipoVisitanteLogoUrl,
    int? GolesLocal,
    int? GolesVisitante);

public interface IMatchQueries
{
    Task<MatchDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<MatchSummaryDto>> GetAllAsync(
        int? jornada,
        Guid? equipoId,
        string? estado,
        PaginationParams pagination,
        CancellationToken cancellationToken = default);
}
