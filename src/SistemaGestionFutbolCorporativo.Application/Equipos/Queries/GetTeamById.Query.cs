using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

public sealed record PlayerDto(
    Guid Id,
    string Nombre,
    int Dorsal,
    string Posicion);

public sealed record TeamDetailResponse
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public DateTime FechaCreacion { get; init; }
    public List<PlayerDto> Jugadores { get; init; } = [];

    public TeamDetailResponse() { }

    public TeamDetailResponse(
        Guid id,
        string nombre,
        string? logoUrl,
        DateTime fechaCreacion,
        List<PlayerDto>? jugadores = null)
    {
        Id = id;
        Nombre = nombre;
        LogoUrl = logoUrl;
        FechaCreacion = fechaCreacion;
        Jugadores = jugadores ?? [];
    }
}

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