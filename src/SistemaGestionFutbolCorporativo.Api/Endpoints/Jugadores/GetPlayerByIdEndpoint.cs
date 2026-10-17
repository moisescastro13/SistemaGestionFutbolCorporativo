using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Jugadores;

public sealed class GetPlayerByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/jugadores/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPlayerByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithName("GetPlayerById")
        .WithTags("Jugadores")
        .Produces<PlayerDetailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
