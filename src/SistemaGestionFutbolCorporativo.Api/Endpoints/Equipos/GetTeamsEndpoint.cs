using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Equipos;

public sealed record GetTeamsQueryParams(
    string? Nombre,
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    string? SortDirection);

public sealed class GetTeamsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/equipos", async ([AsParameters] GetTeamsQueryParams queryParams, ISender sender) =>
        {
            var pagination = new PaginationParams
            {
                PageNumber = queryParams.PageNumber ?? 1,
                PageSize = queryParams.PageSize ?? 10,
                SortBy = queryParams.SortBy,
                SortDirection = queryParams.SortDirection ?? "asc"
            };

            var query = new GetTeamsQuery(queryParams.Nombre, pagination);
            var result = await sender.Send(query);

            return result.ToHttpResult();
        })
        .WithName("GetTeams")
        .WithTags("Equipos")
        .Produces<PagedResponse<TeamSummaryDto>>(StatusCodes.Status200OK);
    }
}
