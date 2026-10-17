using SistemaGestionFutbolCorporativo.Application.Common;

namespace SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;

public sealed record PlayerDetailResponse(
    Guid Id,
    Guid EquipoId,
    string EquipoNombre,
    string Nombre,
    int Dorsal,
    string Posicion,
    int TotalGoles);

public sealed record PlayerSummaryDto(
    Guid Id,
    Guid EquipoId,
    string EquipoNombre,
    string Nombre,
    int Dorsal,
    string Posicion,
    int TotalGoles);

public interface IPlayerQueries
{
    Task<PlayerDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<PlayerSummaryDto>> GetAllAsync(
        Guid? equipoId, 
        string? posicion, 
        PaginationParams pagination, 
        CancellationToken cancellationToken = default);
}
