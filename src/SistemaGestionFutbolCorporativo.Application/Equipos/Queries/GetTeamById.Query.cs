using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

public sealed record PlayerDto(
    Guid Id,
    string Nombre,
    int Dorsal,
    string Posicion);

public sealed record TeamDetailResponse(
    Guid Id,
    string Nombre,
    string? LogoUrl,
    DateTime FechaCreacion,
    List<PlayerDto>? Jugadores);

public sealed record GetTeamByIdQuery(Guid Id) : IRequest<Result<TeamDetailResponse>>;

public sealed class GetTeamByIdQueryHandler(ITeamQueries teamQueries) 
    : IRequestHandler<GetTeamByIdQuery, Result<TeamDetailResponse>>
{
    public async Task<Result<TeamDetailResponse>> Handle(
        GetTeamByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var team = await teamQueries.GetByIdAsync(request.Id, cancellationToken);

        if (team is null)
        {
            return Error.NotFound(
                $"No se encontró ningún equipo con el ID '{request.Id}'.", 
                "Equipo.NotFound");
        }

        return team;
    }
}