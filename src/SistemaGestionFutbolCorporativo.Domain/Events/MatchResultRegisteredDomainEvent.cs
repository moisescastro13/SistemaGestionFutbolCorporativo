using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Domain.Events;

public sealed record MatchResultRegisteredDomainEvent(
    Guid PartidoId,
    Guid EquipoLocalId,
    Guid EquipoVisitanteId,
    int GolesLocal,
    int GolesVisitante,
    DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
}
