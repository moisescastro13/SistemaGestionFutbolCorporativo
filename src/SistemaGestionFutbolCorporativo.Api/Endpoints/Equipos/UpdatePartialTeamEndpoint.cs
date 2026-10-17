using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Equipos;

public sealed record UpdatePartialTeamRequest(string? Nombre, string? LogoUrl);

public sealed class UpdatePartialTeamEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/equipos/{id:guid}", async (Guid id, UpdatePartialTeamRequest request, ISender sender) =>
        {
            var command = new UpdatePartialTeamCommand(id, request.Nombre, request.LogoUrl);
            var result = await sender.Send(command);

            return result.ToHttpResult();
        })
        .WithName("UpdatePartialTeam")
        .WithTags("Equipos")
        .RequireIdempotency()
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
