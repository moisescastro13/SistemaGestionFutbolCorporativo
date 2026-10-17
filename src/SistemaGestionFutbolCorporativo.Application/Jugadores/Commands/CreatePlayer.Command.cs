using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Application.Jugadores.Commands;

public sealed record CreatePlayerCommand(
    Guid EquipoId,
    string Nombre,
    int Dorsal,
    string Posicion) : IRequest<Result<Guid>>;

public sealed class CreatePlayerCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<CreatePlayerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Error.Validation("El nombre del jugador no puede estar vacío.", "Jugador.NombreVacio");
        }

        if (request.Dorsal is < 1 or > 99)
        {
            return Error.Validation("El dorsal debe estar comprendido entre 1 y 99.", "Jugador.DorsalInvalido");
        }

        if (!Jugador.IsValidPosition(request.Posicion))
        {
            return Error.Validation(
                $"La posición '{request.Posicion}' no es válida. Posiciones permitidas: Portero, Defensa, Medio/Mediocampista, Delantero.", 
                "Jugador.PosicionInvalida");
        }

        var equipoExiste = await dbContext.Equipos.AnyAsync(e => e.Id == request.EquipoId, cancellationToken);
        if (!equipoExiste)
        {
            return Error.NotFound($"No se encontró ningún equipo con el ID '{request.EquipoId}'.", "Equipo.NotFound");
        }

        var dorsalOcupado = await dbContext.Jugadores
            .AnyAsync(j => j.EquipoId == request.EquipoId && j.Dorsal == request.Dorsal, cancellationToken);

        if (dorsalOcupado)
        {
            return Error.Conflict(
                $"El dorsal {request.Dorsal} ya está asignado a otro jugador en este equipo.", 
                "Jugador.DorsalDuplicado");
        }

        var jugador = Jugador.Create(request.EquipoId, request.Nombre, request.Dorsal, request.Posicion);

        await dbContext.Jugadores.AddAsync(jugador, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return jugador.Id;
    }
}
