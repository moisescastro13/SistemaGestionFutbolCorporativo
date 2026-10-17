using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

public sealed record GetStandingsQuery : IRequest<Result<IReadOnlyList<TournamentStandingDto>>>;

public sealed class GetStandingsQueryHandler(ITournamentQueries tournamentQueries)
    : IRequestHandler<GetStandingsQuery, Result<IReadOnlyList<TournamentStandingDto>>>
{
    public async Task<Result<IReadOnlyList<TournamentStandingDto>>> Handle(
        GetStandingsQuery request, 
        CancellationToken cancellationToken)
    {
        var standings = await tournamentQueries.GetStandingsAsync(cancellationToken);
        return Result<IReadOnlyList<TournamentStandingDto>>.Success(standings);
    }
}
