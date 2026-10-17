using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Equipos;

public sealed record CreateTeamRequest(string Nombre, string? LogoUrl);

public sealed class CreateTeamEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/equipos", async (CreateTeamRequest request, ISender sender) =>
        {
            var command = new CreateTeamCommand(request.Nombre, request.LogoUrl);
            var result = await sender.Send(command);

            return result.ToCreatedHttpResult(id => $"/api/equipos/{id}");
        })
        .WithName("CreateTeam")
        .WithTags("Equipos")
        .RequireIdempotency()
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
