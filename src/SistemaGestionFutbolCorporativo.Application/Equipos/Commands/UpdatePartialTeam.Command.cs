using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

public sealed record UpdatePartialTeamCommand(
    Guid Id, 
    string? Nombre, 
    string? LogoUrl) : IRequest<Result>;

public sealed class UpdatePartialTeamCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdatePartialTeamCommand, Result>
{
    public async Task<Result> Handle(UpdatePartialTeamCommand request, CancellationToken cancellationToken)
    {
        var equipo = await dbContext.Equipos.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        if (equipo is null)
        {
            return Error.NotFound($"No se encontró ningún equipo con el ID '{request.Id}'.", "Equipo.NotFound");
        }

        if (request.Nombre is null && request.LogoUrl is null)
        {
            return Result.Success();
        }

        if (request.Nombre is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return Error.Validation("El nombre del equipo no puede estar vacío.", "Equipo.NombreVacio");
            }

            var nombreNormalizado = request.Nombre.Trim();
            var nombreEnUso = await dbContext.Equipos
                .AnyAsync(e => e.Id != request.Id && e.Nombre.ToLower() == nombreNormalizado.ToLower(), cancellationToken);

            if (nombreEnUso)
            {
                return Error.Conflict($"Ya existe otro equipo registrado con el nombre '{request.Nombre}'.", "Equipo.Duplicado");
            }

            equipo.UpdateName(request.Nombre);
        }

        if (request.LogoUrl is not null)
        {
            equipo.UpdateLogo(request.LogoUrl);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
