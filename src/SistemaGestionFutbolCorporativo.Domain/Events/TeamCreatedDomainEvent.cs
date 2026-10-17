using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Domain.Events;

public sealed record TeamCreatedDomainEvent(
    Guid EquipoId,
    string Nombre,
    string? LogoUrl,
    DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
}
