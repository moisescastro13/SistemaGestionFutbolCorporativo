using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Jugadores;

public sealed class DeletePlayerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/jugadores/{id:guid}", async (Guid id, ISender sender) =>
        {
            var command = new DeletePlayerCommand(id);
            var result = await sender.Send(command);

            return result.ToNoContentHttpResult();
        })
        .WithName("DeletePlayer")
        .WithTags("Jugadores")
        .RequireIdempotency()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
