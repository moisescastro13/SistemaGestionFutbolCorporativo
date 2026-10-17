using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Application.Partidos.Commands;

public sealed record RegisterGoalDto(
    Guid JugadorId,
    Guid EquipoId,
    int? Minuto,
    bool EsAutogol = false);

public sealed record RegisterMatchResultCommand(
    Guid PartidoId,
    int GolesLocal,
    int GolesVisitante,
    List<RegisterGoalDto>? Goles = null) : IRequest<Result>;

public sealed class RegisterMatchResultCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterMatchResultCommand, Result>
{
    public async Task<Result> Handle(RegisterMatchResultCommand request, CancellationToken cancellationToken)
    {
        if (request.GolesLocal < 0 || request.GolesVisitante < 0)
        {
            return Error.Validation("Los goles no pueden ser números negativos.", "Partido.GolesNegativos");
        }

        var partido = await dbContext.Partidos
            .Include(p => p.Goles)
            .FirstOrDefaultAsync(p => p.Id == request.PartidoId, cancellationToken);

        if (partido is null)
        {
            return Error.NotFound($"No se encontró ningún partido con el ID '{request.PartidoId}'.", "Partido.NotFound");
        }

        if (partido.Estado == EstadoPartido.Finalizado)
        {
            return Error.Conflict("El partido ya fue finalizado previamente.", "Partido.YaFinalizado");
        }

        List<Gol>? golesEntidades = null;

        if (request.Goles is not null && request.Goles.Count > 0)
        {
            var totalGolesMarcador = request.GolesLocal + request.GolesVisitante;
            if (request.Goles.Count != totalGolesMarcador)
            {
                return Error.Validation(
                    $"La cantidad de goles detallados ({request.Goles.Count}) no coincide con el total del marcador ({totalGolesMarcador}).", 
                    "Partido.GolesDetalleNoCoincide");
            }

            var jugadorIds = request.Goles.Select(g => g.JugadorId).Distinct().ToList();
            var jugadores = await dbContext.Jugadores
                .Where(j => jugadorIds.Contains(j.Id))
                .ToDictionaryAsync(j => j.Id, cancellationToken);

            foreach (var g in request.Goles)
            {
                if (g.EquipoId != partido.EquipoLocalId && g.EquipoId != partido.EquipoVisitanteId)
                {
                    return Error.Validation(
                        $"El equipo con ID '{g.EquipoId}' asignado en el gol no participa en este partido.", 
                        "Partido.EquipoNoParticipa");
                }

                if (!jugadores.TryGetValue(g.JugadorId, out var jugador))
                {
                    return Error.NotFound($"No se encontró ningún jugador con ID '{g.JugadorId}'.", "Jugador.NotFound");
                }

                if (g.Minuto is < 1 or > 130)
                {
                    return Error.Validation(
                        $"El minuto '{g.Minuto}' es inválido (debe estar entre 1 y 130).", 
                        "Partido.MinutoInvalido");
                }

                var equipoEsperadoJugador = g.EsAutogol 
                    ? (g.EquipoId == partido.EquipoLocalId ? partido.EquipoVisitanteId : partido.EquipoLocalId)
                    : g.EquipoId;

                if (jugador.EquipoId != equipoEsperadoJugador)
                {
                    return Error.Validation(
                        $"El jugador '{jugador.Nombre}' no pertenece al equipo correspondiente para registrar este gol.", 
                        "Partido.JugadorEquipoInvalido");
                }
            }

            golesEntidades = request.Goles.Select(g => 
                Gol.Register(partido.Id, g.JugadorId, g.EquipoId, g.Minuto, g.EsAutogol)).ToList();
        }

        partido.RegisterResult(request.GolesLocal, request.GolesVisitante, golesEntidades);

        if (golesEntidades is not null && golesEntidades.Count > 0)
        {
            await dbContext.Goles.AddRangeAsync(golesEntidades, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
