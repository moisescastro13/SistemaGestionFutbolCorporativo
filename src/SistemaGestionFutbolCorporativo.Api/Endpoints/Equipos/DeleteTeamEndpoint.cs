using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Equipos;

public sealed class DeleteTeamEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/equipos/{id:guid}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteTeamCommand(id);
            var result = await sender.Send(command);

            return result.ToNoContentHttpResult();
        })
        .WithName("DeleteTeam")
        .WithTags("Equipos")
        .RequireIdempotency()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
