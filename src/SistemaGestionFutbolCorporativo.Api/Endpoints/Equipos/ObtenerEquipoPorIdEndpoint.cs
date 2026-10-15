using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Equipos;

public sealed class ObtenerEquipoPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/equipos/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ObtenerEquipoPorIdQuery(id));
            return result.ToHttpResult();
        })
        .WithName("ObtenerEquipoPorId")
        .WithTags("Equipos")
        .Produces<EquipoDetalleResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet("/api/equipos/test", async (ISender sender) =>
        {
            var result = await sender.Send(new ObtenerEquipoPorIdQuery(Guid.Empty));
            return result.ToHttpResult();
        })
        .WithTags("EquiposTest")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}