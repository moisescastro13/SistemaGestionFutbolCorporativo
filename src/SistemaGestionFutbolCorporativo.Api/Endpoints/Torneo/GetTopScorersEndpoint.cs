using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Torneo.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Torneo;

public sealed class GetTopScorersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/torneo/goleadores", async (int? top, ISender sender) =>
        {
            var result = await sender.Send(new GetTopScorersQuery(top));
            return result.ToHttpResult();
        })
        .WithName("GetTopScorers")
        .WithTags("Torneo")
        .Produces<IReadOnlyList<TopScorerDto>>(StatusCodes.Status200OK);
    }
}
