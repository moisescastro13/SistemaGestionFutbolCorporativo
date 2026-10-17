using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Jugadores;

public sealed record CreatePlayerRequest(string Nombre, int Dorsal, string Posicion);

public sealed class CreatePlayerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/equipos/{equipoId:guid}/jugadores", async (
            Guid equipoId, 
            CreatePlayerRequest request, 
            ISender sender) =>
        {
            var command = new CreatePlayerCommand(
                equipoId, 
                request.Nombre, 
                request.Dorsal, 
                request.Posicion);

            var result = await sender.Send(command);

            return result.ToCreatedHttpResult(id => $"/api/jugadores/{id}");
        })
        .WithName("CreatePlayer")
        .WithTags("Jugadores")
        .RequireIdempotency()
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
