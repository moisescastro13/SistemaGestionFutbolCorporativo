using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Application.Partidos.Commands;

public sealed record ScheduleMatchCommand(
    Guid EquipoLocalId,
    Guid EquipoVisitanteId,
    int Jornada,
    DateTime FechaHora) : IRequest<Result<Guid>>;

public sealed class ScheduleMatchCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<ScheduleMatchCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ScheduleMatchCommand request, CancellationToken cancellationToken)
    {
        if (request.EquipoLocalId == request.EquipoVisitanteId)
        {
            return Error.Validation("Un equipo no puede jugar contra sí mismo.", "Partido.EquiposIguales");
        }

        if (request.Jornada < 1)
        {
            return Error.Validation("La jornada debe ser mayor o igual a 1.", "Partido.JornadaInvalida");
        }

        var localExiste = await dbContext.Equipos.AnyAsync(e => e.Id == request.EquipoLocalId, cancellationToken);
        if (!localExiste)
        {
            return Error.NotFound($"No se encontró el equipo local con ID '{request.EquipoLocalId}'.", "Equipo.NotFound");
        }

        var visitanteExiste = await dbContext.Equipos.AnyAsync(e => e.Id == request.EquipoVisitanteId, cancellationToken);
        if (!visitanteExiste)
        {
            return Error.NotFound($"No se encontró el equipo visitante con ID '{request.EquipoVisitanteId}'.", "Equipo.NotFound");
        }

        var yaProgramado = await dbContext.Partidos.AnyAsync(
            p => p.Jornada == request.Jornada &&
                 ((p.EquipoLocalId == request.EquipoLocalId && p.EquipoVisitanteId == request.EquipoVisitanteId) ||
                  (p.EquipoLocalId == request.EquipoVisitanteId && p.EquipoVisitanteId == request.EquipoLocalId)),
            cancellationToken);

        if (yaProgramado)
        {
            return Error.Conflict(
                $"Ya existe un partido programado entre ambos equipos para la jornada {request.Jornada}.", 
                "Partido.YaProgramado");
        }

        var partido = Partido.Schedule(
            request.EquipoLocalId, 
            request.EquipoVisitanteId, 
            request.Jornada, 
            request.FechaHora);

        await dbContext.Partidos.AddAsync(partido, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return partido.Id;
    }
}
