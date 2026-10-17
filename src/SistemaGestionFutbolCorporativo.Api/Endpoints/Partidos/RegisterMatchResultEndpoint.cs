using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Partidos.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Partidos;

public sealed record RegisterGoalRequest(
    Guid JugadorId,
    Guid EquipoId,
    int? Minuto,
    bool EsAutogol = false);

public sealed record RegisterMatchResultRequest(
    int GolesLocal,
    int GolesVisitante,
    List<RegisterGoalRequest>? Goles = null);

public sealed class RegisterMatchResultEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/partidos/{id:guid}/resultado", async (
            Guid id, 
            RegisterMatchResultRequest request, 
            ISender sender) =>
        {
            var command = new RegisterMatchResultCommand(
                id,
                request.GolesLocal,
                request.GolesVisitante,
                request.Goles?.Select(g => new RegisterGoalDto(
                    g.JugadorId,
                    g.EquipoId,
                    g.Minuto,
                    g.EsAutogol)).ToList());

            var result = await sender.Send(command);

            return result.ToHttpResult();
        })
        .WithName("RegisterMatchResult")
        .WithTags("Partidos")
        .RequireIdempotency()
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
