using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Partidos.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Partidos;

public sealed class GetMatchByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/partidos/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMatchByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithName("GetMatchById")
        .WithTags("Partidos")
        .Produces<MatchDetailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
