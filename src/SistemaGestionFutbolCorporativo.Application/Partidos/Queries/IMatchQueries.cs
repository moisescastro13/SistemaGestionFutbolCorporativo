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

public sealed record MatchDetailResponse
{
    public Guid Id { get; init; }
    public int Jornada { get; init; }
    public DateTime FechaHora { get; init; }
    public string Estado { get; init; } = string.Empty;
    public Guid EquipoLocalId { get; init; }
    public string EquipoLocalNombre { get; init; } = string.Empty;
    public string? EquipoLocalLogoUrl { get; init; }
    public Guid EquipoVisitanteId { get; init; }
    public string EquipoVisitanteNombre { get; init; } = string.Empty;
    public string? EquipoVisitanteLogoUrl { get; init; }
    public int? GolesLocal { get; init; }
    public int? GolesVisitante { get; init; }
    public List<GoalDetailDto> Goles { get; init; } = [];

    public MatchDetailResponse() { }

    public MatchDetailResponse(
        Guid id,
        int jornada,
        DateTime fechaHora,
        string estado,
        Guid equipoLocalId,
        string equipoLocalNombre,
        string? equipoLocalLogoUrl,
        Guid equipoVisitanteId,
        string equipoVisitanteNombre,
        string? equipoVisitanteLogoUrl,
        int? golesLocal,
        int? golesVisitante,
        List<GoalDetailDto>? goles = null)
    {
        Id = id;
        Jornada = jornada;
        FechaHora = fechaHora;
        Estado = estado;
        EquipoLocalId = equipoLocalId;
        EquipoLocalNombre = equipoLocalNombre;
        EquipoLocalLogoUrl = equipoLocalLogoUrl;
        EquipoVisitanteId = equipoVisitanteId;
        EquipoVisitanteNombre = equipoVisitanteNombre;
        EquipoVisitanteLogoUrl = equipoVisitanteLogoUrl;
        GolesLocal = golesLocal;
        GolesVisitante = golesVisitante;
        Goles = goles ?? [];
    }
}

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
