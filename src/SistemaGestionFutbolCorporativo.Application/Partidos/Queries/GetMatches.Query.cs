using MediatR;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Partidos.Queries;

public sealed record GetMatchesQuery(
    int? Jornada,
    Guid? EquipoId,
    string? Estado,
    PaginationParams Pagination) : IRequest<Result<PagedResponse<MatchSummaryDto>>>;

public sealed class GetMatchesQueryHandler(IMatchQueries matchQueries)
    : IRequestHandler<GetMatchesQuery, Result<PagedResponse<MatchSummaryDto>>>
{
    public async Task<Result<PagedResponse<MatchSummaryDto>>> Handle(
        GetMatchesQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await matchQueries.GetAllAsync(
            request.Jornada,
            request.EquipoId,
            request.Estado,
            request.Pagination,
            cancellationToken);

        return result;
    }
}
