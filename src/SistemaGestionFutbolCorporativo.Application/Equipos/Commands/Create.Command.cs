using MediatR;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

public record RegistrarEquipoCommand(string Nombre, string? LogoUrl) : IRequest<Result<Guid>>;

public class RegistrarEquipoCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<RegistrarEquipoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegistrarEquipoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Error.Validation("El nombre del equipo no puede estar vacío.", "Equipo.NombreVacio");
        }

        var existe = await dbContext.Equipos.AnyAsync(e => e.Nombre == request.Nombre, cancellationToken);
        if (existe)
        {
            return Error.Conflict($"Ya existe un equipo registrado con el nombre '{request.Nombre}'.", "Equipo.Duplicado");
        }

        var equipo = Equipo.Crear(request.Nombre, request.LogoUrl);

        await dbContext.Equipos.AddAsync(equipo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return equipo.Id; 
    }
}