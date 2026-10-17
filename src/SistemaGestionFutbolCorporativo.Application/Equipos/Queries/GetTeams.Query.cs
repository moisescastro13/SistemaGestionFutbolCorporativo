using MediatR;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

public sealed record GetTeamsQuery(
    string? Nombre, 
    PaginationParams Pagination) : IRequest<Result<PagedResponse<TeamSummaryDto>>>;

public sealed class GetTeamsQueryHandler(ITeamQueries teamQueries) 
    : IRequestHandler<GetTeamsQuery, Result<PagedResponse<TeamSummaryDto>>>
{
    public async Task<Result<PagedResponse<TeamSummaryDto>>> Handle(
        GetTeamsQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await teamQueries.GetAllAsync(request.Nombre, request.Pagination, cancellationToken);
        return result;
    }
}
