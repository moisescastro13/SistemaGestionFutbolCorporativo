using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Partidos.Queries;

public sealed record GetMatchByIdQuery(Guid Id) : IRequest<Result<MatchDetailResponse>>;

public sealed class GetMatchByIdQueryHandler(IMatchQueries matchQueries)
    : IRequestHandler<GetMatchByIdQuery, Result<MatchDetailResponse>>
{
    public async Task<Result<MatchDetailResponse>> Handle(
        GetMatchByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var match = await matchQueries.GetByIdAsync(request.Id, cancellationToken);
        if (match is null)
        {
            return Error.NotFound(
                $"No se encontró ningún partido con el ID '{request.Id}'.", 
                "Partido.NotFound");
        }

        return match;
    }
}
