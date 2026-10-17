using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Jugadores;

public sealed record GetPlayersQueryParams(
    Guid? EquipoId,
    string? Posicion,
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    string? SortDirection);

public sealed class GetPlayersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/jugadores", async ([AsParameters] GetPlayersQueryParams queryParams, ISender sender) =>
        {
            var pagination = new PaginationParams
            {
                PageNumber = queryParams.PageNumber ?? 1,
                PageSize = queryParams.PageSize ?? 10,
                SortBy = queryParams.SortBy,
                SortDirection = queryParams.SortDirection ?? "asc"
            };

            var query = new GetPlayersQuery(
                queryParams.EquipoId, 
                queryParams.Posicion, 
                pagination);

            var result = await sender.Send(query);

            return result.ToHttpResult();
        })
        .WithName("GetPlayers")
        .WithTags("Jugadores")
        .Produces<PagedResponse<PlayerSummaryDto>>(StatusCodes.Status200OK);
    }
}
