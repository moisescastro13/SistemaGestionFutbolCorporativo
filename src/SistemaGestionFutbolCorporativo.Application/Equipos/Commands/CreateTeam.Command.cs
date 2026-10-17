using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;
using SistemaGestionFutbolCorporativo.Domain.Entities;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Commands;

public record CreateTeamCommand(string Nombre, string? LogoUrl) : IRequest<Result<Guid>>;

public class CreateTeamCommandHandler(
    IApplicationDbContext dbContext,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTeamCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
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

        var equipo = Equipo.Create(request.Nombre, request.LogoUrl);

        await dbContext.Equipos.AddAsync(equipo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return equipo.Id; 
    }
}