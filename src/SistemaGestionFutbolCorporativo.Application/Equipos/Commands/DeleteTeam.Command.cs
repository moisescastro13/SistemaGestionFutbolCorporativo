using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

public sealed record DeleteTeamCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteTeamCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTeamCommand, Result>
{
    public async Task<Result> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var equipo = await dbContext.Equipos.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if (equipo is null)
        {
            return Error.NotFound($"No se encontró ningún equipo con el ID '{request.Id}'.", "Equipo.NotFound");
        }

        var tieneJugadores = await dbContext.Jugadores.AnyAsync(j => j.EquipoId == request.Id, cancellationToken);
        if (tieneJugadores)
        {
            return Error.Conflict("No se puede eliminar el equipo porque tiene jugadores asociados.", "Equipo.TieneJugadores");
        }

        var tienePartidos = await dbContext.Partidos.AnyAsync(
            p => p.EquipoLocalId == request.Id || p.EquipoVisitanteId == request.Id, 
            cancellationToken);

        if (tienePartidos)
        {
            return Error.Conflict("No se puede eliminar el equipo porque tiene partidos registrados.", "Equipo.TienePartidos");
        }

        dbContext.Equipos.Remove(equipo);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
