using SistemaGestionFutbolCorporativo.Application.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

public sealed record TeamSummaryDto(
    Guid Id,
    string Nombre,
    string? LogoUrl,
    DateTime FechaCreacion,
    int TotalJugadores);

public interface ITeamQueries
{
    Task<TeamDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<TeamSummaryDto>> GetAllAsync(string? nombre, PaginationParams pagination, CancellationToken cancellationToken = default);
}
