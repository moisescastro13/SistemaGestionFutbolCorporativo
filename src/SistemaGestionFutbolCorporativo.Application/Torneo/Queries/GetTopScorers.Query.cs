using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

public sealed record GetTopScorersQuery(int? Limit = null) : IRequest<Result<IReadOnlyList<TopScorerDto>>>;

public sealed class GetTopScorersQueryHandler(ITournamentQueries tournamentQueries)
    : IRequestHandler<GetTopScorersQuery, Result<IReadOnlyList<TopScorerDto>>>
{
    public async Task<Result<IReadOnlyList<TopScorerDto>>> Handle(
        GetTopScorersQuery request, 
        CancellationToken cancellationToken)
    {
        var limit = request.Limit is > 0 ? request.Limit : null;
        var topScorers = await tournamentQueries.GetTopScorersAsync(limit, cancellationToken);
        return Result<IReadOnlyList<TopScorerDto>>.Success(topScorers);
    }
}
