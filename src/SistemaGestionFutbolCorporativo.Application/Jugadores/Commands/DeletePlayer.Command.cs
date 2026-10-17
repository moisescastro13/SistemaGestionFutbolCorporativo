using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Jugadores.Commands;

public sealed record DeletePlayerCommand(Guid Id) : IRequest<Result>;

public sealed class DeletePlayerCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<DeletePlayerCommand, Result>
{
    public async Task<Result> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        var jugador = await dbContext.Jugadores.FirstOrDefaultAsync(j => j.Id == request.Id, cancellationToken);
        if (jugador is null)
        {
            return Error.NotFound($"No se encontró ningún jugador con el ID '{request.Id}'.", "Jugador.NotFound");
        }

        var tieneGoles = await dbContext.Goles.AnyAsync(g => g.JugadorId == request.Id, cancellationToken);
        if (tieneGoles)
        {
            return Error.Conflict(
                "No se puede eliminar el jugador porque tiene goles registrados en el torneo.", 
                "Jugador.TieneGoles");
        }

        dbContext.Jugadores.Remove(jugador);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
