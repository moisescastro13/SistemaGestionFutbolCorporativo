using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Queries;

public sealed record JugadorDto(
    Guid Id,
    string Nombre,
    int Dorsal,
    string Posicion);

public sealed record EquipoDetalleResponse(
    Guid Id,
    string Nombre,
    string? LogoUrl,
    DateTime FechaCreacion,
    List<JugadorDto>? Jugadores);

public sealed record ObtenerEquipoPorIdQuery(Guid Id) : IRequest<Result<EquipoDetalleResponse>>;

public sealed class ObtenerEquipoPorIdQueryHandler(IEquipoQueries equipoQueries) 
    : IRequestHandler<ObtenerEquipoPorIdQuery, Result<EquipoDetalleResponse>>
{
    public async Task<Result<EquipoDetalleResponse>> Handle(
        ObtenerEquipoPorIdQuery request, 
        CancellationToken cancellationToken)
    {
        var equipo = await equipoQueries.ObtenerPorIdAsync(request.Id, cancellationToken);

        if (equipo is null)
        {
            return Error.NotFound(
                $"No se encontró ningún equipo con el ID '{request.Id}'.", 
                "Equipo.NotFound");
        }

        return equipo;
    }
}