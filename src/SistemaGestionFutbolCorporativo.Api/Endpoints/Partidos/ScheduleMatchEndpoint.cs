using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Api.Filters;
using SistemaGestionFutbolCorporativo.Application.Partidos.Commands;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Partidos;

public sealed record ScheduleMatchRequest(
    Guid EquipoLocalId,
    Guid EquipoVisitanteId,
    int Jornada,
    DateTime FechaHora);

public sealed class ScheduleMatchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/partidos", async (ScheduleMatchRequest request, ISender sender) =>
        {
            var command = new ScheduleMatchCommand(
                request.EquipoLocalId, 
                request.EquipoVisitanteId, 
                request.Jornada, 
                request.FechaHora);

            var result = await sender.Send(command);

            return result.ToCreatedHttpResult(id => $"/api/partidos/{id}");
        })
        .WithName("ScheduleMatch")
        .WithTags("Partidos")
        .RequireIdempotency()
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
