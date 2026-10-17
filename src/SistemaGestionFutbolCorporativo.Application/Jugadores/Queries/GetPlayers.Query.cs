using MediatR;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;

public sealed record GetPlayersQuery(
    Guid? EquipoId,
    string? Posicion,
    PaginationParams Pagination) : IRequest<Result<PagedResponse<PlayerSummaryDto>>>;

public sealed class GetPlayersQueryHandler(IPlayerQueries playerQueries)
    : IRequestHandler<GetPlayersQuery, Result<PagedResponse<PlayerSummaryDto>>>
{
    public async Task<Result<PagedResponse<PlayerSummaryDto>>> Handle(
        GetPlayersQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await playerQueries.GetAllAsync(
            request.EquipoId, 
            request.Posicion, 
            request.Pagination, 
            cancellationToken);

        return result;
    }
}
