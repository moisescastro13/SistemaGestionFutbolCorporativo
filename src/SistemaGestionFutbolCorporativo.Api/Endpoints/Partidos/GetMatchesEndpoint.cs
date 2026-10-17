using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionFutbolCorporativo.Api.Common;
using SistemaGestionFutbolCorporativo.Api.Extensions;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Application.Partidos.Queries;

namespace SistemaGestionFutbolCorporativo.Api.Endpoints.Partidos;

public sealed record GetMatchesQueryParams(
    int? Jornada,
    Guid? EquipoId,
    string? Estado,
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    string? SortDirection);

public sealed class GetMatchesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/partidos", async ([AsParameters] GetMatchesQueryParams queryParams, ISender sender) =>
        {
            var pagination = new PaginationParams
            {
                PageNumber = queryParams.PageNumber ?? 1,
                PageSize = queryParams.PageSize ?? 10,
                SortBy = queryParams.SortBy,
                SortDirection = queryParams.SortDirection ?? "asc"
            };

            var query = new GetMatchesQuery(
                queryParams.Jornada,
                queryParams.EquipoId,
                queryParams.Estado,
                pagination);

            var result = await sender.Send(query);

            return result.ToHttpResult();
        })
        .WithName("GetMatches")
        .WithTags("Partidos")
        .Produces<PagedResponse<MatchSummaryDto>>(StatusCodes.Status200OK);
    }
}
