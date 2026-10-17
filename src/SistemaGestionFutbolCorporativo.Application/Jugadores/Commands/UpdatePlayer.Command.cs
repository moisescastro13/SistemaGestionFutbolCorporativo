using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Application.Jugadores.Commands;

public sealed record UpdatePlayerCommand(
    Guid Id,
    string Nombre,
    int Dorsal,
    string Posicion) : IRequest<Result>;

public sealed class UpdatePlayerCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdatePlayerCommand, Result>
{
    public async Task<Result> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
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

        var jugador = await dbContext.Jugadores.FirstOrDefaultAsync(j => j.Id == request.Id, cancellationToken);
        if (jugador is null)
        {
            return Error.NotFound($"No se encontró ningún jugador con el ID '{request.Id}'.", "Jugador.NotFound");
        }

        var dorsalOcupado = await dbContext.Jugadores.AnyAsync(
            j => j.EquipoId == jugador.EquipoId && j.Id != request.Id && j.Dorsal == request.Dorsal, 
            cancellationToken);

        if (dorsalOcupado)
        {
            return Error.Conflict(
                $"El dorsal {request.Dorsal} ya está asignado a otro jugador en este equipo.", 
                "Jugador.DorsalDuplicado");
        }

        jugador.Update(request.Nombre, request.Dorsal, request.Posicion);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
