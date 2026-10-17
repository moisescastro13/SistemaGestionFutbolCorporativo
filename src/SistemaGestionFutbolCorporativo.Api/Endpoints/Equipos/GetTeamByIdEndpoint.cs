using MediatR;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Equipos;

public sealed class GetTeamByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/equipos/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetTeamByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithName("GetTeamById")
        .WithTags("Equipos")
        .Produces<TeamDetailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}