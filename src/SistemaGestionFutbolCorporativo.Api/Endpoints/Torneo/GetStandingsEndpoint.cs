using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Torneo;

public sealed class GetStandingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/torneo/posiciones", async (ISender sender) =>
        {
            var result = await sender.Send(new GetStandingsQuery());
            return result.ToHttpResult();
        })
        .WithName("GetStandings")
        .WithTags("Torneo")
        .Produces<IReadOnlyList<TournamentStandingDto>>(StatusCodes.Status200OK);
    }
}
