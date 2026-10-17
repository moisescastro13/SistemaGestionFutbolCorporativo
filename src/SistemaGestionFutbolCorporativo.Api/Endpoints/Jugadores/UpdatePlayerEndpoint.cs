using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Jugadores;

public sealed record UpdatePlayerRequest(string Nombre, int Dorsal, string Posicion);

public sealed class UpdatePlayerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/jugadores/{id:guid}", async (
            Guid id, 
            UpdatePlayerRequest request, 
            ISender sender) =>
        {
            var command = new UpdatePlayerCommand(
                id, 
                request.Nombre, 
                request.Dorsal, 
                request.Posicion);

            var result = await sender.Send(command);

            return result.ToHttpResult();
        })
        .WithName("UpdatePlayer")
        .WithTags("Jugadores")
        .RequireIdempotency()
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
